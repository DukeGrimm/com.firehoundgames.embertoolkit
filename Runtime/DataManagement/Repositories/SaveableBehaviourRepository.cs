using EmberToolkit.Common.Interfaces.Data;
using EmberToolkit.Common.Interfaces.Repository;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace EmberToolkit.DataManagement.Repositories
{
    public class SaveableBehaviourRepository : Repository<ISaveableBehaviour>, ISaveableBehaviourRepository
    {
        public SaveableBehaviourRepository(ISaveLoadEvents saveLoadEvents, bool shouldSave = true) : base(saveLoadEvents, shouldSave)
        {
        }

        public S GetObject<S>(Guid id) => Get<S>(id);

        public List<S> GetAllObjects<S>() => GetAll<S>().ToList();

        public void SaveBehaviour(ISaveableBehaviour payload)
        {
            if (payload == null) throw new ArgumentNullException(nameof(payload));
            if (Get(payload.Id) != null)
            {
                Update(payload);
                return;
            }
            Add(payload);
        }

        public ISaveableBehaviour LoadBehaviour(Guid targetId)
        {
            var savedData = Get(targetId);
            if (savedData == null) Debug.Write("Error Couldn't Find SaveableBehaviour: " + targetId.ToString());
            else return savedData;
            return null;
            //targetObject.MapFromPayload(savedData);
        }
    }
}
