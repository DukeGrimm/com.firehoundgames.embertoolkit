using EmberToolkit.Common.DataTypes;
using EmberToolkit.Common.Interfaces.Data;
using EmberToolkit.Common.Interfaces.Repository;
using EmberToolkit.Common.Interfaces.Unity.Behaviours;
using EmberToolkit.Unity.Services;
using EmberToolkit.Unity.Services.Models;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;

namespace EmberToolkit.Unity.Behaviours
{

    public abstract class EmberBehaviour : SerializedMonoBehaviour, IEmberBehaviour
    {
        protected List<EventSubscription> eventSubscriptions = new List<EventSubscription>();
        protected List<EventSubscription> eventSubscriptionsWithArgs = new List<EventSubscription>();
        protected List<UnityEventSubscription> unityEventSubscriptions = new List<UnityEventSubscription>();
        protected List<string> registeredEventNames = new List<string>();
        protected ISaveableBehaviourRepository _repo;
        private ISaveLoadEvents _saveLoadEvents;
        protected bool WasDisabled = false;


        //GUID Work
        [FoldoutGroup("Behaviour Data:")]
        [OdinSerialize, InlineProperty]
        [DisableIf("@this.id != Guid.Empty")]
        protected Guid id = new Guid();
        [FoldoutGroup("Behaviour Data:")]
        [OdinSerialize]
        protected bool SaveObject = false;

        public virtual Type ItemType => GetType();

        public Guid Id
        {
            get => id;
        }

        public string Name => nameof(ItemType);

        public IReadOnlyList<string> GetRegisteredEvents()
        {
            if (registeredEventNames == null || registeredEventNames.Count < 1)
                return new List<string>();

            return registeredEventNames;

        }
        protected virtual void Awake() {
            if (id == Guid.Empty)
            {
                id = Guid.NewGuid();
            }
            if (SaveObject)
            {
                RequestService(out _repo);
                RequestService(out _saveLoadEvents);
                SubscribeEvent(_saveLoadEvents, nameof(_saveLoadEvents.OnSaveEvent), Save);
                SubscribeEvent(_saveLoadEvents, nameof(_saveLoadEvents.OnLoadEvent), Load);
            }
        }

        protected virtual void OnDestroy()
        {
            //Clear Subscriptions to events before the Object is destroyed
            UnsubscribeEvents();


            //TOCONSIDER: We may want to remove this object from the DataRepo on Destroy, Consider if we need to preserve it in an archive.
        }

        protected virtual void OnEnable()
        {
            if(WasDisabled)
                ResubscribeEvents();
        }

        protected virtual void OnDisable()
        {
            UnsubscribeEvents(true);
            WasDisabled = true;
            //Save Data to Repo in case a save event occurs while disabled.
            if (_repo != null) Save();
        }
        protected override void OnBeforeSerialize()
        {

        }
        protected override void OnAfterDeserialize()
        {

        }

        void OnValidate()
        {

        }
        public virtual void Save() => _repo.SaveBehaviour(new SaveableBehaviour(ItemType, this));

        public virtual void Load()
        {
            if(_repo == null) {
                Debug.LogError($"SaveLoad Repository not found for {this}. Cannot Load data.");
                return;
            }
            _repo.LoadBehaviour(id).ApplySavedFields(ItemType, this);
        }

        public void SetEmberId(byte[] array) => id = new Guid(array);

        #region Events

        // Parameterless UnityEvent subscription - returns the stored subscription so caller can unsubscribe later if desired.
        public UnityEventSubscription SubscribeUnityEvent(GameObject gameObject, UnityEvent uEvent, UnityAction uAction)
        {
            if (unityEventSubscriptions == null)
                unityEventSubscriptions = new List<UnityEventSubscription>();

            var ueSub = UnityEventSubscription.Create(gameObject, uEvent, uAction);
            if (!ueSub.IsEventValid())
            {
                Debug.LogError($"{this.name} failed to bind {uAction.Method.Name} to target {gameObject?.name ?? "null"}");
                return ueSub;
            }

            ueSub.Subscribe();
            unityEventSubscriptions.Add(ueSub);
            return ueSub;
        }

        // Generic UnityEvent<T> subscription
        public UnityEventSubscription SubscribeUnityEvent<T>(GameObject gameObject, UnityEvent<T> uEvent, UnityAction<T> uAction)
        {
            if (unityEventSubscriptions == null)
                unityEventSubscriptions = new List<UnityEventSubscription>();

            var ueSub = UnityEventSubscription.Create(gameObject, uEvent, uAction);
            if (!ueSub.IsEventValid())
            {
                Debug.LogError($"{this.name} failed to bind {uAction.Method.Name} to target {gameObject?.name ?? "null"}");
                return ueSub;
            }

            ueSub.Subscribe();
            unityEventSubscriptions.Add(ueSub);
            return ueSub;
        }
        public void SubscribeEvent(object eventSource, string eventName, Action eventHandler, bool ignoreDisabledCleanup = false)
        {
            if (eventSubscriptions == null)
                eventSubscriptions = new List<EventSubscription>();


            if (eventSubscriptions.FirstOrDefault(x => x.EventName == eventName && x.EventSource == eventSource) == null)
            {
                EventSubscription newEvent = new EventSubscription(eventSource, eventName, eventHandler, ignoreDisabledCleanup);
                newEvent.Subscribe();
                eventSubscriptions.Add(newEvent);
                AddRegisteredEventName(eventName);
            }

            // Add event name to the registered events list

        }

        public void SubscribeEvent<TArgs>(object eventSource, string eventName, Action<TArgs> eventHandler, bool ignoreDisabledCleanup = false)
        {
            if (eventSubscriptionsWithArgs == null)
                eventSubscriptionsWithArgs = new List<EventSubscription>();

            if (eventSubscriptionsWithArgs.FirstOrDefault(x => x.EventName == eventName && x.EventSource == eventSource) == null)
            {
                EventSubscription newEvent = new EventSubscription(eventSource, eventName, eventHandler, ignoreDisabledCleanup);
                newEvent.Subscribe();
                eventSubscriptionsWithArgs.Add(newEvent);
                AddRegisteredEventName(eventName);
            }
        }

        public void ResubscribeEvents()
        {
            if (eventSubscriptions != null)
            {
                foreach (EventSubscription thisEvent in eventSubscriptions.Where(x => !x.IgnoreDisabled))
                {
                    thisEvent.Subscribe();
                    AddRegisteredEventName(thisEvent.EventName);
                }
            }
            if (eventSubscriptionsWithArgs != null)
            {
                foreach (EventSubscription thisEvent in eventSubscriptionsWithArgs.Where(x => !x.IgnoreDisabled))
                {
                    thisEvent.Subscribe();
                    AddRegisteredEventName(thisEvent.EventName);
                }
            }
            //UnityEvents
            if (unityEventSubscriptions != null)
            {
                for (int i = unityEventSubscriptions.Count - 1; i >= 0; i--)
                {
                    UnityEventSubscription thisEvent = unityEventSubscriptions[i];
                    if (!thisEvent.IsEventValid())
                        unityEventSubscriptions.Remove(thisEvent);
                    else
                        thisEvent.Subscribe();
                }
            }
        }

        //For Calling in scripts
        // This uses reflection to access the internal fields of UnityEventSubscription instances stored in the list.
        public void UnsubscribeUnityEvent(GameObject gameObject, UnityEvent uEvent, UnityAction action)
        {
            if (unityEventSubscriptions == null || unityEventSubscriptions.Count == 0) return;

            UnityEventSubscription match = null;
            foreach (var sub in unityEventSubscriptions)
            {
                if (UnityEventSubscriptionMatches(sub, gameObject, uEvent, action))
                {
                    match = sub;
                    break;
                }
            }

            if (match != null)
            {
                UnsubscribeUnityEvent(match);
                unityEventSubscriptions.Remove(match);
            }
            else
            {
                Debug.LogWarning($"UnsubscribeUnityEvent: no matching subscription found for {gameObject?.name} / {uEvent?.GetType().Name}");
            }
        }

        // Unsubscribe using the exact subscription instance returned from SubscribeUnityEvent
        public void UnsubscribeUnityEvent(UnityEventSubscription subscription)
        {
            if (subscription == null) return;
            if (unityEventSubscriptions != null && unityEventSubscriptions.Contains(subscription))
                unityEventSubscriptions.Remove(subscription);
            UnsubscribeUnityEventInternal(subscription);
        }

        private void UnsubscribeUnityEventInternal(UnityEventSubscription ueSub)
        {
            if (ueSub == null) return;
            if (!ueSub.IsEventValid()) return;
            ueSub.Unsubscribe();
        }

        private bool UnityEventSubscriptionMatches(UnityEventSubscription sub, GameObject expectedSource, UnityEvent expectedEvent, UnityAction expectedAction)
        {
            if (sub == null) return false;
            try
            {
                var t = sub.GetType();
                var sourceField = t.GetField("_sourceObject", BindingFlags.NonPublic | BindingFlags.Instance);
                var eventField = t.GetField("_uEvent", BindingFlags.NonPublic | BindingFlags.Instance);
                var actionField = t.GetField("_uAction", BindingFlags.NonPublic | BindingFlags.Instance);

                var src = sourceField?.GetValue(sub) as GameObject;
                var ev = eventField?.GetValue(sub) as UnityEvent;
                var act = actionField?.GetValue(sub) as Delegate;

                return ReferenceEquals(src, expectedSource) && ReferenceEquals(ev, expectedEvent) && (act != null && act.Equals(expectedAction));
            }
            catch
            {
                // Reflection failure -> no match
                return false;
            }
        }


        public void UnsubscribeEvent(object eventSource, string eventName)
        {
            if (eventSubscriptions != null && eventSubscriptions.Any())
            {
                EventSubscription subscription = eventSubscriptions.FirstOrDefault(x => x.EventName.Equals(eventName) && x.EventSource == eventSource);
                if (subscription != null)
                {
                    subscription.Unsubscribe();
                    RemoveRegisteredEventName(subscription.EventName);
                }
            }

            if (eventSubscriptionsWithArgs != null && eventSubscriptionsWithArgs.Any())
            {
                EventSubscription subscription = eventSubscriptionsWithArgs.FirstOrDefault(x => x.EventName.Equals(eventName) && x.EventSource == eventSource);
                if (subscription != null)
                {
                    subscription.Unsubscribe();
                    RemoveRegisteredEventName(subscription.EventName);
                }
            }
        }
        //UnSubscribeAllEvents
        public void UnsubscribeEvents(bool isOnDisable = false)
        {
            if (eventSubscriptions?.Count > 0)
            {
                for (int i = eventSubscriptions.Count - 1; i >= 0; i--)
                {
                    if (isOnDisable && eventSubscriptions[i].IgnoreDisabled) { continue; }
                    var subscription = eventSubscriptions[i];
                    subscription.Unsubscribe();
                    RemoveRegisteredEventName(subscription.EventName);
                }
            }
            if (eventSubscriptionsWithArgs?.Count > 0)
            {
                for (int i = eventSubscriptionsWithArgs.Count - 1; i >= 0; i--)
                {
                    if (isOnDisable && eventSubscriptionsWithArgs[i].IgnoreDisabled) { continue; }
                    var subscription = eventSubscriptionsWithArgs[i];
                    subscription.Unsubscribe();
                    RemoveRegisteredEventName(subscription.EventName);
                }
            }

            //Unity Events
            if (unityEventSubscriptions?.Count > 0)
            {
                foreach (var subscription in unityEventSubscriptions.ToList())
                {
                    UnsubscribeUnityEventInternal(subscription);
                }
            }

        }

        private void AddRegisteredEventName(string eventName)
        {
            if (registeredEventNames == null)
                registeredEventNames = new List<string>();

            if (!registeredEventNames.Contains(eventName))
                registeredEventNames.Add(eventName);
        }

        private void RemoveRegisteredEventName(string eventName)
        {
            if (registeredEventNames == null)
                registeredEventNames = new List<string>();

            if (!registeredEventNames.Contains(eventName))
                registeredEventNames.Remove(eventName);
        }
        #endregion

        #region Services


        /// <summary>
        /// Checks if a service or interface is registered.
        /// </summary>
        /// <typeparam name="T">The top-most interface or service type.</typeparam>
        /// <returns><c>true</c> if the service or interface is registered; otherwise, <c>false</c>.</returns>
        public bool CheckService<T>(bool checkCore = false)
        {
            return ServiceConductor.CheckForService<T>(checkCore);
        }
        public bool CheckForThisService()
        {
            return ServiceConductor.CheckForService(this.GetType());
        }
        public T GetService<T>()
        {
            var service = ServiceConductor.Get<T>();
            if (service == null)
                Debug.Log("There was a problem Registering this service: " + typeof(T) + " By this class: " + this, this);
            return service;
        }
        public bool GetService<T>(out T target)
        {
            target = ServiceConductor.Get<T>();
            return target != null;
        }

        protected void RequestService<T>(out T target)
        {
            if (!GetService(out target))
            {
                Debug.LogError($"{this.name}: Could not locate {typeof(T).Name} Service.", this);
            }
        }



        #endregion

        #region Components
        protected bool GetRequiredComponent<T>(out T component) where T : Component
        {
            component = GetComponent<T>();
            if (component != null) return true;
            Debug.LogError($"{this.name}: Missing required component: {typeof(T).Name}", this);
            return false;
        }
        #endregion


    }
}
