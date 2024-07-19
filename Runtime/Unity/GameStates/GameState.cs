using EmberToolkit.Common.Enum.Game;
using EmberToolkit.Common.Interfaces.Game;
using EmberToolkit.Unity.Services;
using System;
using UnityEngine;

namespace EmberToolkit.Unity.GameStates
{
    public abstract class GameState<T> : IGameState<T>
    {
        //protected MasterInputController masterInput;
        //protected IEmberInputManager _inputManager;
        public abstract T State { get; }

        public abstract event Action OnStateEntered;
        public abstract event Action OnStateExited;

        public abstract void ProcessGameStateChange();

        public GameState()
        {
            //GetService(out _inputManager);
            //_inputManager.GetInputController(out masterInput);
        }

        public abstract void TriggerStateEnd();

        public void SubscribeToStateEvent(Action action, GameStateEvents eventName)
        {
            if (eventName == GameStateEvents.OnStateEntered) OnStateEntered += action;
            else if (eventName == GameStateEvents.OnStateExited) OnStateExited += action;
        }
        public void UnsubscribeToStateEvent(Action action, GameStateEvents eventName)
        {
            if (eventName == GameStateEvents.OnStateEntered) OnStateEntered -= action;
            else if (eventName == GameStateEvents.OnStateExited) OnStateExited -= action;
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
