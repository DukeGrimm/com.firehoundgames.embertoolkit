using EmberToolkit.Common.Enum.Game;
using EmberToolkit.Common.Interfaces.Game;
using EmberToolkit.Common.Interfaces.Unity.Behaviours.Managers.Game;
using EmberToolkit.Unity.Behaviours;
using EmberToolkit.Unity.GameStates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace GuildLegends.Game.GameStates
{
    public abstract class GameStateManager<T> : EmberSingleton, IGameStateManager<T>
    {

        protected Dictionary<T, GameState<T>> _GameStates = new Dictionary<T, GameState<T>>();
        private bool _Initialized;

        private T currentGameState;

        public event Action<T> OnGameStateChanged;
        public T CurrentGameState => currentGameState;


        // Start is called before the first frame update
        protected override void Awake()
        {
            base.Awake();
            if (!_Initialized) InitializeGameStates(GetGameStateTypes());
            //ChangeGameState(GameStateStage.StartUp);
        }

        protected abstract IEnumerable<Type>? GetGameStateTypes();

        protected virtual void InitializeGameStates(IEnumerable<Type>? gameStateTypes = null)
        {
            _GameStates.Clear();
            if (gameStateTypes == null)
            {
                gameStateTypes = Assembly.GetAssembly(typeof(GameState<T>)).GetTypes().Where(t => typeof(GameState<T>).IsAssignableFrom(t) && t.IsAbstract == false);
            }
            foreach(var gameStateClass in gameStateTypes)
            {
                var gState = Activator.CreateInstance(gameStateClass) as GameState<T>;
                _GameStates.Add(gState.State, gState);
            }
            _Initialized= true;
        }

        public void ChangeGameState(T targetStage)
        {
            GameState<T> stageClass;
            if(_GameStates.TryGetValue(targetStage, out stageClass))
            {
                TriggerCurStateEnd();
                stageClass.ProcessGameStateChange();
                OnGameStateChanged?.Invoke(targetStage);
                currentGameState = targetStage;
            }
            else
            {
                Debug.LogError($"GameStateManager: Could not find targte GameStateStage of {targetStage}");
                return;
            }

        }

        public void TriggerCurStateEnd()
        {
            Type stateEnumType = typeof(T);
            object nullStateObj = System.Enum.ToObject(stateEnumType, 0);
            if (currentGameState != null && currentGameState.Equals(nullStateObj)) return;
            GameState<T> curState = _GameStates[currentGameState];
            curState.TriggerStateEnd();
        }
        public bool FindGameState(out IGameState<T> gameState, T targetStage)
        {
            GameState<T> output;
            _GameStates.TryGetValue(targetStage, out output);
            if (output == null)
            {
                gameState = null;
                return false;
            }
            else { 
                gameState = output;
                return true;
            }
        }
        #region Events
        /// <summary>
        /// Subscribes to the specified event of a given game state, triggering the provided action when the event occurs.
        /// </summary>
        /// <param name="stage">The target game state.</param>
        /// <param name="eventName">The event to subscribe to ("Entered" or "Exited").</param>
        /// <param name="action">The action to be triggered when the event occurs.</param>
        public void SubscribeToStateEvent(T stage, GameStateEvents eventName,Action action)
        {
            if (FindGameState(out IGameState<T> targetStage, stage)) targetStage.SubscribeToStateEvent(action, eventName);
        }
        /// <summary>
        /// Unsubscribes from the specified event of a given game state, removing the provided action from the event's list of subscribers.
        /// </summary>
        /// <param name="stage">The target game state.</param>
        /// <param name="eventName">The event to unsubscribe from ("Entered" or "Exited").</param>
        /// <param name="action">The action to be removed from the event's subscribers.</param>
        public void UnsubscribeToStateEvent(T stage, GameStateEvents eventName, Action action)
        {
            if (FindGameState(out IGameState<T> targetStage, stage)) targetStage.UnsubscribeToStateEvent(action, eventName);
        }

        #endregion

    }
}
