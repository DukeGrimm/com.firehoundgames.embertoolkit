using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;

namespace EmberToolkit.Unity.Services.Models
{
    /// <summary>
    /// Lightweight holder that subscribes/unsubscribes to UnityEvent and UnityEvent<T>.
    /// Use the static Create overloads to construct parameterless or typed subscriptions.
    /// Internally stores the UnityEventBase and a Delegate (UnityAction or UnityAction<T>)
    /// so AddListener/RemoveListener can be invoked for both non-generic and generic UnityEvents.
    /// </summary>
    public class UnityEventSubscription
    {
        private readonly GameObject _sourceObject;
        private readonly UnityEventBase _uEvent;
        private readonly Delegate _uAction;     // UnityAction or UnityAction<T>
        private readonly Type? _uArgType;       // null for parameterless UnityEvent

        private MethodInfo? _addMethod;
        private MethodInfo? _removeMethod;

        private UnityEventSubscription(GameObject sourceObject, UnityEventBase uEvent, Delegate uAction, Type? argType)
        {
            _sourceObject = sourceObject;
            _uEvent = uEvent;
            _uAction = uAction;
            _uArgType = argType;

            // cache add/remove overloads that accept a single parameter
            var evtType = _uEvent.GetType();
            _addMethod = evtType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                                .FirstOrDefault(m => m.Name == "AddListener" && m.GetParameters().Length == 1);
            _removeMethod = evtType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                                   .FirstOrDefault(m => m.Name == "RemoveListener" && m.GetParameters().Length == 1);
        }

        // Factory for parameterless UnityEvent
        public static UnityEventSubscription Create(GameObject sourceObject, UnityEvent uEvent, UnityAction uAction)
        {
            if (sourceObject == null) throw new ArgumentNullException(nameof(sourceObject));
            if (uEvent == null) throw new ArgumentNullException(nameof(uEvent));
            if (uAction == null) throw new ArgumentNullException(nameof(uAction));
            return new UnityEventSubscription(sourceObject, uEvent, uAction, null);
        }

        // Factory for UnityEvent<T>
        public static UnityEventSubscription Create<T>(GameObject sourceObject, UnityEvent<T> uEvent, UnityAction<T> uAction)
        {
            if (sourceObject == null) throw new ArgumentNullException(nameof(sourceObject));
            if (uEvent == null) throw new ArgumentNullException(nameof(uEvent));
            if (uAction == null) throw new ArgumentNullException(nameof(uAction));
            return new UnityEventSubscription(sourceObject, uEvent, uAction, typeof(T));
        }

        public void Subscribe()
        {
            if (!IsEventValid()) return;
            if (_addMethod == null)
            {
                Debug.LogError($"UnityEventSubscription: AddListener not found on {_uEvent.GetType().Name}");
                return;
            }
            try
            {
                _addMethod.Invoke(_uEvent, new object[] { _uAction });
            }
            catch (Exception ex)
            {
                Debug.LogError($"UnityEventSubscription: failed to AddListener on {_uEvent.GetType().Name}: {ex}");
            }
        }

        public void Unsubscribe()
        {
            if (!IsEventValid()) return;
            if (_removeMethod == null)
            {
                Debug.LogError($"UnityEventSubscription: RemoveListener not found on {_uEvent.GetType().Name}");
                return;
            }
            try
            {
                _removeMethod.Invoke(_uEvent, new object[] { _uAction });
            }
            catch (Exception ex)
            {
                Debug.LogError($"UnityEventSubscription: failed to RemoveListener on {_uEvent.GetType().Name}: {ex}");
            }
        }

        public bool IsEventValid()
        {
            // sourceObject might become null (destroyed) in Unity; check both it and the event/action
            return _sourceObject != null && _uEvent != null && _uAction != null;
        }
    }

}
