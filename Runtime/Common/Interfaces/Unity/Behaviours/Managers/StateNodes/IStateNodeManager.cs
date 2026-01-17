using EmberToolkit.Common.Enum.StateNodes;
using EmberToolkit.Common.Interfaces.StateNodes;
using System;
using System.Collections.Generic;
using System.Text;

namespace EmberToolkit.Common.Interfaces.Unity.Behaviours.Managers.StateNodes
{
    public interface IStateNodeManager<T>
    {
        event Action<T> OnStateNodeChanged;
        bool FindStateNode(out IStateNode<T> stateNode, T targetStage);
        void ChangeStateNode(T targetStage);
        /// <summary>
        /// Subscribes to the specified event of a given game state, triggering the provided action when the event occurs.
        /// </summary>
        /// <param name="stage">The target state.</param>
        /// <param name="eventName">The event to subscribe to ("Entered" or "Exited").</param>
        /// <param name="action">The action to be triggered when the event occurs.</param>
        void SubscribeToStateEvent(T stage, StateNodeEvents eventName, Action action);
        /// <summary>
        /// Unsubscribes from the specified event of a given state, removing the provided action from the event's list of subscribers.
        /// </summary>
        /// <param name="stage">The target state.</param>
        /// <param name="eventName">The event to unsubscribe from ("Entered" or "Exited").</param>
        /// <param name="action">The action to be removed from the event's subscribers.</param>
        void UnsubscribeToStateEvent(T stage, StateNodeEvents eventName, Action action);

    }
}
