using EmberToolkit.Common.Enum.StateNodes;
using System;

namespace EmberToolkit.Common.Interfaces.StateNodes
{
    public interface IStateNode<T>
    {
        T State { get; }

        event Action OnStateEntered;
        event Action OnStateExited;

        void SubscribeToStateEvent(Action action, StateNodeEvents eventName);
        void UnsubscribeToStateEvent(Action action, StateNodeEvents eventName);

        void ProcessStateChange();
        void TriggerStateEnd();
    }

}
