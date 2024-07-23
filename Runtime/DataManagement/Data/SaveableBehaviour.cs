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
            if (BehaviourType == instance.ItemType)
            {
                foreach (KeyValuePair<string, object> savedField in SaveableFields)
                {
                    FieldInfo field = BehaviourType.GetField(savedField.Key, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    if (field != null)
                    {
                        object value = savedField.Value;

                        // Check if the value is a JToken (which includes JObject and JArray)
                        if (value is Newtonsoft.Json.Linq.JToken token)
                        {
                            try
                            {
                                object convertedValue;

                                // Special handling for Guid fields
                                if (field.FieldType == typeof(System.Guid))
                                {
                                    string guidString = token.ToString();
                                    convertedValue = Guid.Parse(guidString);
                                }
                                else
                                {
                                    // Attempt to convert the JToken to the expected field type for other types
                                    convertedValue = token.ToObject(field.FieldType);
                                }

                                field.SetValue(instance, convertedValue);
                            }
                            catch (Exception ex)
                            {
                                // Handle or log the exception as needed
                                Console.WriteLine($"Error converting field {field.Name}: {ex.Message}");
                                return false;
                            }
                        }
                        else
                        {
                            // If it's not a JToken, proceed as before
                            field.SetValue(instance, value);
                        }
                    }
                }
                return true;
            }

            return false;
        }
    }
}