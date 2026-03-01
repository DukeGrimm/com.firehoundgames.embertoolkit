using EmberToolkit.Common.Interfaces.Configuration;
using EmberToolkit.Common.Interfaces.Data;
using EmberToolkit.Common.Interfaces.Repository;
//using EmberToolkit.Common.Serialization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace EmberToolkit.DataManagement.Repositories
{
    public class SaveableObjectRepository : Repository<ISaveableObject>, ISaveableObjectRepository
    {
        public SaveableObjectRepository(Guid objRepoId, ISaveLoadEvents saveLoadEvents, IDataRepository dataRepo) : base(saveLoadEvents, dataRepo, default, objRepoId)
        {
        }

        public void SaveObject(ISaveableObject payload)
        {
            if(payload == null) throw new ArgumentNullException(nameof(payload));
            if (Get(payload.Id) != null) Update(payload);
            else Add(payload);
        }

        public void LoadObject(ISaveableObject target)
        {
            var targetObject = target;
            var savedData = Get(targetObject.Id);
            if (savedData == null) Debug.Write("Error Couldn't Find SaveableObject: " + targetObject.Id.ToString());
            targetObject.MapFromPayload(savedData); 
        }

        public S GetObject<S>(Guid id) => Get<S>(id);

        public List<S> GetAllObjects<S>() => GetAll<S>().ToList();



    }
}
