using EmberToolkit.Common.Attributes;
using EmberToolkit.Common.Interfaces.Data;
using EmberToolkit.Common.Interfaces.Unity.Behaviours;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace EmberToolkit.DataManagement.Data
{
    [Serializable]
    public class SaveableBehaviour : ISaveableBehaviour
    {
        public Guid Id { get; set; }
        public Type ItemType { get; set; }
        public Dictionary<string, object> SaveableFields { get; set; } = new Dictionary<string, object>();
        public Type BehaviourType { get; set; }

        public string Name => nameof(ItemType);

        public SaveableBehaviour() {
        
        }

        public SaveableBehaviour(IEmberBehaviour targetBehaviour)
        {
            Id = targetBehaviour.Id;
            ItemType = this.GetType();
            BehaviourType = targetBehaviour.ItemType;
            ExtractSaveableFields(targetBehaviour);
        }

        private void ExtractSaveableFields(IEmberBehaviour target)
        {
           FieldInfo[] fields = BehaviourType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            foreach (FieldInfo field in fields)
            {
                SaveFieldAttribute attribute = field.GetCustomAttribute<SaveFieldAttribute>();
                if (attribute != null)
                {
                    object value = field.GetValue(target);
                    SaveableFields[field.Name] = value;
                }
            }
        }

        public bool ApplySavedFields(IEmberBehaviour instance)
        {
            if(BehaviourType == instance.ItemType)
            {
                foreach (KeyValuePair<string, object> savedField in SaveableFields)
                {
                    FieldInfo field = BehaviourType.GetField(savedField.Key, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    if (field != null)
                    {
                        field.SetValue(instance, savedField.Value);
                    }
                }
                return true;
            }

            return false;


        }
    }
}