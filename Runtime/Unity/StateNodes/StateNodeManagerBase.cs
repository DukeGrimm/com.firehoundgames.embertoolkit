using EmberToolkit.Common.Enum.StateNodes;
using EmberToolkit.Common.Interfaces.StateNodes;
using EmberToolkit.Common.Interfaces.Unity.Behaviours.Managers.StateNodes;
using EmberToolkit.Unity.Behaviours;
using EmberToolkit.Unity.StateNodes;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace GuildLegends.Game.StateNodes
{
    public abstract class StateNodeManagerBase<T> : EmberSingleton, IStateNodeManager<T>
    {
        
        [ShowInInspector, ReadOnly]
        protected Dictionary<T, StateNode<T>> _States = new Dictionary<T, StateNode<T>>();
        private bool _Initialized;

        private T currentState;

        public event Action<T> OnStateNodeChanged;
        public T CurrentState => currentState;


        // Start is called before the first frame update
        protected override void Awake()
        {
            base.Awake();
            if (!_Initialized) InitializeStateNodes(GetStateNodeTypes());
            //ChangeGameState(GameStateStage.StartUp);
        }

        protected abstract IEnumerable<Type>? GetStateNodeTypes();

        protected virtual void InitializeStateNodes(IEnumerable<Type>? stateNodeTypes = null)
        {
            _States.Clear();
            if (stateNodeTypes == null)
            {
                stateNodeTypes = Assembly.GetAssembly(typeof(StateNode<T>)).GetTypes().Where(t => typeof(StateNode<T>).IsAssignableFrom(t) && t.IsAbstract == false);
            }
            foreach(var stateClass in stateNodeTypes)
            {
                var gState = Activator.CreateInstance(stateClass) as StateNode<T>;
                _States.Add(gState.State, gState);
            }
            _Initialized= true;
        }

        public void ChangeStateNode(T targetStage)
        {
            StateNode<T> stageClass;
            if(_States.TryGetValue(targetStage, out stageClass))
            {
                TriggerCurStateEnd();
                stageClass.ProcessStateChange();
                OnStateNodeChanged?.Invoke(targetStage);
                currentState = targetStage;
            }
            else
            {
                Debug.LogError($"StateNodeManager: Could not find target StateStage of {targetStage}");
                return;
            }

        }

        public void TriggerCurStateEnd()
        {
            Type stateEnumType = typeof(T);
            object nullStateObj = System.Enum.ToObject(stateEnumType, 0);
            if (currentState != null && currentState.Equals(nullStateObj)) return;
            StateNode<T> curState = _States[currentState];
            curState.TriggerStateEnd();
        }
        public bool FindStateNode(out IStateNode<T> stateNode, T targetStage)
        {
            StateNode<T> output;
            _States.TryGetValue(targetStage, out output);
            if (output == null)
            {
                stateNode = null;
                return false;
            }
            else { 
                stateNode = output;
                return true;
            }
        }
        #region Events
        /// <summary>
        /// Subscribes to the specified event of a given state, triggering the provided action when the event occurs.
        /// </summary>
        /// <param name="stage">The target state.</param>
        /// <param name="eventName">The event to subscribe to ("Entered" or "Exited").</param>
        /// <param name="action">The action to be triggered when the event occurs.</param>
        public void SubscribeToStateEvent(T stage, StateNodeEvents eventName,Action action)
        {
            if (FindStateNode(out IStateNode<T> targetStage, stage)) targetStage.SubscribeToStateEvent(action, eventName);
        }
        /// <summary>
        /// Unsubscribes from the specified event of a given state, removing the provided action from the event's list of subscribers.
        /// </summary>
        /// <param name="stage">The target state.</param>
        /// <param name="eventName">The event to unsubscribe from ("Entered" or "Exited").</param>
        /// <param name="action">The action to be removed from the event's subscribers.</param>
        public void UnsubscribeToStateEvent(T stage, StateNodeEvents eventName, Action action)
        {
            if (FindStateNode(out IStateNode<T> targetStage, stage)) targetStage.UnsubscribeToStateEvent(action, eventName);
        }

        #endregion

    }
}
