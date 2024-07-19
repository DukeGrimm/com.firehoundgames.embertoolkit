﻿using EmberToolkit.Common.Interfaces.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace EmberToolkit.Common.Interfaces.Repository
{
    public interface ISaveableBehaviourRepository
    {
        void SaveBehaviour(ISaveableBehaviour payload);
        ISaveableBehaviour LoadBehaviour(Guid targetId);
        S GetObject<S>(Guid id);

        List<S> GetAllObjects<S>();
    }
}
