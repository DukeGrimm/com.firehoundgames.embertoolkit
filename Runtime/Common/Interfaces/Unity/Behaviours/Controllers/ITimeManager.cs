using System;

namespace EmberToolkit.Common.Interfaces.Unity.Behaviours.Controllers
{
    public interface ITimeManager
    {
        DateTime InGameTime { get; }

        void LoadGameTime(DateTime gameTime);

        bool IsPaused { get; }

        public void SetPauseState(bool pauseState);
        public void TogglePause();
        public void PushTimeForward(int minutes, int hours = 0);
        public void SetTimeFactor(int factor);

    }
}
