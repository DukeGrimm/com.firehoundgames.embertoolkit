using System;

namespace EmberToolkit.Common.Interfaces.Unity.Behaviours.Managers.Time
{
    public interface ITimeManagerEvents
    {
        event Action<DateTime> OnTimeUpdated;
        event Action<DateTime> OnDateUpdated;
    }
}
