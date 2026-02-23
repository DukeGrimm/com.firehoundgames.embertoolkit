using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmberToolkit.Unity.Services.Models
{
    public class EventSubscription
    {
        private object _eventSource;
        private string _eventName;
        private Delegate _eventHandler;
        private bool _hasArguments;
        private bool isSubscribed = false;
        private bool _ignoreDisabled = false;

        public bool IsSubscribed => isSubscribed;
        public bool IgnoreDisabled => _ignoreDisabled;

        public EventSubscription(object eventSource, string eventName, Delegate eventHandler, bool ignoreDisabled = false)
        {
            _eventSource = eventSource;
            _eventName = eventName;
            _eventHandler = eventHandler;
            _hasArguments = eventHandler.Method.GetParameters().Length > 0;
            _ignoreDisabled = ignoreDisabled;
        }

        public string EventName { get { return _eventName; } }
        public object EventSource { get { return _eventSource; } }

        public void Subscribe()
        {
            if (!isSubscribed && _eventSource != null && !string.IsNullOrEmpty(_eventName) && _eventHandler != null)
            {
                var eventInfo = _eventSource.GetType().GetEvent(_eventName);
                if (eventInfo != null)
                {
                    try
                    {
                        var handlerType = eventInfo.EventHandlerType;
                        // Create a delegate of the exact event handler type from the provided delegate's target/method
                        var handler = Delegate.CreateDelegate(handlerType, _eventHandler.Target, _eventHandler.Method);
                        // Use the event's add method to subscribe the typed delegate
                        eventInfo.GetAddMethod().Invoke(_eventSource, new object[] { handler });
                        isSubscribed = true;
                    }
                    catch (Exception ex)
                    {
                        // Optional: replace with your logging
                        UnityEngine.Debug.LogError($"EventSubscription.Subscribe: Failed to subscribe to {_eventName} on {_eventSource.GetType().Name}: {ex}");
                    }
                }
            }
        }

        public void Unsubscribe()
        {
            if (isSubscribed && _eventSource != null && !string.IsNullOrEmpty(_eventName) && _eventHandler != null)
            {
                var eventInfo = _eventSource.GetType().GetEvent(_eventName);
                if (eventInfo != null)
                {
                    try
                    {
                        var handlerType = eventInfo.EventHandlerType;
                        var handler = Delegate.CreateDelegate(handlerType, _eventHandler.Target, _eventHandler.Method);
                        eventInfo.GetRemoveMethod().Invoke(_eventSource, new object[] { handler });
                        isSubscribed = false;
                    }
                    catch (Exception ex)
                    {
                        // Optional: replace with your logging
                        UnityEngine.Debug.LogError($"EventSubscription.Unsubscribe: Failed to unsubscribe from {_eventName} on {_eventSource.GetType().Name}: {ex}");
                    }
                }
            }
        }
    }
}
