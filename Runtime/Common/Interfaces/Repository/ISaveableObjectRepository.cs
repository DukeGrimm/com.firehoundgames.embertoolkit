using EmberToolkit.Common.Interfaces.Data;
using System;
using System.Collections.Generic;

namespace EmberToolkit.Common.Interfaces.Repository
{
    public interface ISaveableObjectRepository : IRepository<ISaveableObject>
    {
        void SaveObject(ISaveableObject payload);
        void LoadObject(ISaveableObject target);
        S GetObject<S>(Guid id);

        List<S> GetAllObjects<S>();
    }
}