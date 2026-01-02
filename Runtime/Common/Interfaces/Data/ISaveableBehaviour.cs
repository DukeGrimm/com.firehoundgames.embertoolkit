using EmberToolkit.Common.Interfaces.Repository;
using EmberToolkit.Common.Interfaces.Unity.Behaviours;
using System;

namespace EmberToolkit.Common.Interfaces.Data
{
    public interface ISaveableBehaviour : IEmberObject
    {
        bool ApplySavedFields(Type targetType, IEmberBehaviour instance);
    }
}
