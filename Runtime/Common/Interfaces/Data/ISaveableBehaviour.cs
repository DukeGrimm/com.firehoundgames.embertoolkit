using EmberToolkit.Common.Interfaces.Repository;
using EmberToolkit.Common.Interfaces.Unity.Behaviours;
using System;
using System.Collections.Generic;
using System.Text;

namespace EmberToolkit.Common.Interfaces.Data
{
    public interface ISaveableBehaviour : IEmberObject
    {
        Type BehaviourType { get; set; }
        bool ApplySavedFields(IEmberBehaviour instance);

    }
}
