using EmberToolkit.Common.Interfaces.Data;
using EmberToolkit.Common.Interfaces.Repository;
using Sirenix.OdinInspector;
using System;
using System.Reflection;

namespace EmberToolkit.DataManagement.Data
{
    public class SaveableObject : ISaveableObject
    {
        private ISaveLoadEvents _events;
        private ISaveableObjectRepository _repo;
        [ShowInInspector, ReadOnly]
        public Guid Id { get; private set; }   

        public SaveableObject(Guid objId, ISaveLoadEvents saveLoadEvents = null, ISaveableObjectRepository repo = null)
        {
            Id = objId != Guid.Empty ? objId : Guid.NewGuid();
            _events = saveLoadEvents;
            _repo = repo;
            if(_events != null)
            {
                _events.OnSaveEvent += Save;
                _events.OnLoadEvent += Load;
            }
        }

        public void MapFromPayload(object source)
        {
            Type sourceType = source.GetType();
            Type targetType = this.GetType();

            // Get all fields from the source object
            FieldInfo[] sourceFields = sourceType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            // Iterate over the fields and copy their values to the corresponding fields in the target object
            foreach (FieldInfo sourceField in sourceFields)
            {
                // Find the corresponding field in the target object by name
                FieldInfo targetField = targetType.GetField(sourceField.Name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

                // Copy the value from the source field to the target field
                if (targetField != null && targetField.FieldType == sourceField.FieldType)
                {
                    object value = sourceField.GetValue(source);
                    targetField.SetValue(this, value);
                }
            }
        }

        public void Save()
        {
            if (_repo == null) return;
            _repo.SaveObject(this);
        }

        public void Load()
        {
            if (_repo == null) return;
            _repo.LoadObject(this);
        }
    }
}
