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

        public void Subscribe()
        {
            if (!isSubscribed && _eventSource != null && !string.IsNullOrEmpty(_eventName) && _eventHandler != null)
            {
                var eventInfo = _eventSource.GetType().GetEvent(_eventName);
                if (eventInfo != null)
                {
                    if (!_hasArguments)
                    {
                        eventInfo.AddEventHandler(_eventSource, _eventHandler);
                        isSubscribed = true;
                    }
                    else
                    {
                        var handlerType = eventInfo.EventHandlerType;
                        var handler = Delegate.CreateDelegate(handlerType, _eventHandler.Target, _eventHandler.Method.Name);
                        eventInfo.GetAddMethod().Invoke(_eventSource, new object[] { handler });
                        isSubscribed = true;
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
                    if (!_hasArguments)
                    {
                        eventInfo.RemoveEventHandler(_eventSource, _eventHandler);
                        isSubscribed = false;
                    }
                    else
                    {
                        var handlerType = eventInfo.EventHandlerType;
                        var handler = Delegate.CreateDelegate(handlerType, _eventHandler.Target, _eventHandler.Method.Name);
                        eventInfo.GetRemoveMethod().Invoke(_eventSource, new object[] { handler });
                        isSubscribed = false;
                    }
                }
            }
        }
    }
}
