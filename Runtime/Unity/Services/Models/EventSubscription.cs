using EmberToolkit.Common.Enum.Events;
using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;

namespace EmberToolkit.Unity.Services.Models
{

    public class EventSubscription
    {
        private readonly object _eventSource;
        private readonly string _eventName;
        private readonly Delegate _eventHandler; // original delegate
        private Delegate? _typedHandler;         // created delegate used for add/remove
        private readonly bool _ignoreDisabled;
        private readonly eEventKind _kind;
        private MemberInfo? _unityEventMember;
        private bool isSubscribed;

        //accessors
        public object EventSource => _eventSource;
        public string EventName => _eventName;
        public eEventKind Kind => _kind;
        public bool IgnoreDisabled => _ignoreDisabled;


        public EventSubscription(object eventSource, string eventName, Delegate eventHandler, bool ignoreDisabled = false, eEventKind kind = eEventKind.CSharpEvent)
        {
            _eventSource = eventSource ?? throw new ArgumentNullException(nameof(eventSource));
            _eventName = eventName ?? throw new ArgumentNullException(nameof(eventName));
            _eventHandler = eventHandler ?? throw new ArgumentNullException(nameof(eventHandler));
            _ignoreDisabled = ignoreDisabled;
            _kind = kind;
        }

        public void Subscribe()
        {
            if (isSubscribed) return;

            if (_kind == eEventKind.CSharpEvent)
            {
                // C# event path only
                var eventInfo = _eventSource.GetType().GetEvent(_eventName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (eventInfo == null)
                {
                    Debug.LogError($"C# event '{_eventName}' not found on {_eventSource.GetType().Name}");
                    return;
                }
                var handlerType = eventInfo.EventHandlerType!;
                _typedHandler = Delegate.CreateDelegate(handlerType, _eventHandler.Target, _eventHandler.Method);
                eventInfo.AddEventHandler(_eventSource, _typedHandler);
                isSubscribed = true;
                return;
            }

            // UnityEvent path only
            var type = _eventSource.GetType();
            var field = type.GetField(_eventName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            MemberInfo? member = field;
            Type? memberType = field?.FieldType;
            if (memberType == null)
            {
                var prop = type.GetProperty(_eventName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                member = prop;
                memberType = prop?.PropertyType;
            }

            if (memberType == null || !typeof(UnityEventBase).IsAssignableFrom(memberType))
            {
                Debug.LogError($"UnityEvent '{_eventName}' not found (or not a UnityEvent) on {type.Name}");
                return;
            }

            _unityEventMember = member;
            object unityEventInstance = (member is FieldInfo f) ? f.GetValue(_eventSource) : ((PropertyInfo)member).GetValue(_eventSource);
            if (unityEventInstance == null)
            {
                Debug.LogError($"UnityEvent instance '{_eventName}' on {type.Name} is null.");
                return;
            }

            var addMethod = memberType.GetMethod("AddListener", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (addMethod == null) { Debug.LogError($"AddListener not found for UnityEvent '{_eventName}'"); return; }

            // Build typed UnityAction / UnityAction<T> based on memberType generics
            if (memberType == typeof(UnityEvent))
            {
                var unityActionType = typeof(UnityAction);
                _typedHandler = Delegate.CreateDelegate(unityActionType, _eventHandler.Target, _eventHandler.Method);
            }
            else if (memberType.IsGenericType && memberType.GetGenericTypeDefinition() == typeof(UnityEvent<>))
            {
                var arg = memberType.GetGenericArguments()[0];
                var unityActionGeneric = typeof(UnityAction<>).MakeGenericType(arg);
                _typedHandler = Delegate.CreateDelegate(unityActionGeneric, _eventHandler.Target, _eventHandler.Method);
            }
            else
            {
                Debug.LogError($"Unsupported UnityEvent signature for '{_eventName}' on {type.Name}");
                return;
            }

            addMethod.Invoke(unityEventInstance, new object[] { _typedHandler });
            isSubscribed = true;
        }

        public void Unsubscribe()
        {
            if (!isSubscribed) return;

            if (_kind == eEventKind.CSharpEvent)
            {
                var eventInfo = _eventSource.GetType().GetEvent(_eventName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (eventInfo != null && _typedHandler != null)
                {
                    eventInfo.RemoveEventHandler(_eventSource, _typedHandler);
                }
                isSubscribed = false;
                return;
            }

            // UnityEvent removal
            if (_unityEventMember == null) { isSubscribed = false; return; }
            object unityEventInstance = (_unityEventMember is FieldInfo f) ? f.GetValue(_eventSource) : ((PropertyInfo)_unityEventMember).GetValue(_eventSource);
            if (unityEventInstance == null) { isSubscribed = false; return; }
            var removeMethod = unityEventInstance.GetType().GetMethod("RemoveListener", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (removeMethod != null && _typedHandler != null)
            {
                removeMethod.Invoke(unityEventInstance, new object[] { _typedHandler });
            }
            isSubscribed = false;
        }
    }
}