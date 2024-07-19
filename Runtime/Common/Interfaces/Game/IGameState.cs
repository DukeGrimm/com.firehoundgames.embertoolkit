using EmberToolkit.Common.Enum.Game;
using System;

namespace EmberToolkit.Common.Interfaces.Game
{
    public interface IGameState<T>
    {
        T State { get; }

        event Action OnStateEntered;
        event Action OnStateExited;

        void SubscribeToStateEvent(Action action, GameStateEvents eventName);
        void UnsubscribeToStateEvent(Action action, GameStateEvents eventName);

        void ProcessGameStateChange();
        void TriggerStateEnd();
    }

}
