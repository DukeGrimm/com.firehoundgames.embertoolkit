using EmberToolkit.Common.Enum.StateNodes;
using EmberToolkit.Common.Interfaces.StateNodes;
using EmberToolkit.Unity.Services;
using System;
using UnityEngine;

namespace EmberToolkit.Unity.StateNodes
{
    public abstract class StateNode<T> : IStateNode<T>
    {

        //protected MasterInputController masterInput;
        //protected IEmberInputManager _inputManager;

        protected abstract string _name { get; }
        public abstract T State { get; }

        public abstract event Action OnStateEntered;
        public abstract event Action OnStateExited;

        public abstract void ProcessStateChange();

        public StateNode()
        {
            //GetService(out _inputManager);
            //_inputManager.GetInputController(out masterInput);
        }

        public abstract void TriggerStateEnd();

        public void SubscribeToStateEvent(Action action, StateNodeEvents eventName)
        {
            if (eventName == StateNodeEvents.OnStateEntered) OnStateEntered += action;
            else if (eventName == StateNodeEvents.OnStateExited) OnStateExited += action;
        }
        public void UnsubscribeToStateEvent(Action action, StateNodeEvents eventName)
        {
            if (eventName == StateNodeEvents.OnStateEntered) OnStateEntered -= action;
            else if (eventName == StateNodeEvents.OnStateExited) OnStateExited -= action;
        }

        #region Services


        /// <summary>
        /// Checks if a service or interface is registered.
        /// </summary>
        /// <typeparam name="T">The top-most interface or service type.</typeparam>
        /// <returns><c>true</c> if the service or interface is registered; otherwise, <c>false</c>.</returns>
        public bool CheckService<S>(bool checkCore = false)
        {
            return ServiceConductor.CheckForService<S>(checkCore);
        }
        public bool CheckForThisService()
        {
            return ServiceConductor.CheckForService(GetType());
        }
        public S GetService<S>()
        {
            var service = ServiceConductor.Get<S>();
            if (service == null)
                Debug.Log($"{GetType().Name}:There was a problem Registering this service: {typeof(S)}");
            return service;
        }
        public bool GetService<S>(out S target)
        {
            target = ServiceConductor.Get<S>();
            return target != null;
        }

        protected void RequestService<S>(out S target)
        {
            if (!GetService(out target))
            {
                Debug.LogError($"{GetType().Name}: Could not locate {typeof(S).Name} Service.");
            }
        }



        #endregion

    }
}
