using EmberToolkit.Common.Attributes;
using EmberToolkit.Common.Interfaces.Data;
using EmberToolkit.Common.Interfaces.Unity.Behaviours;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace EmberToolkit.Common.DataTypes
{
    /// <summary>
    /// Helper DTO for persisting MonoBehaviour state.
    /// 
    /// Purpose:
    /// - Extracts fields marked with [SaveField] from an IEmberBehaviour and stores them
    ///   in a serializable form (SaveableFields dictionary) together with a GUID.
    /// - Applies saved values back to a matching IEmberBehaviour instance (ApplySavedFields).
    /// 
    /// Notes:
    /// - Values stored in SaveableFields may be raw CLR values or JSON tokens (JToken) after deserialization.
    /// - ApplySavedFields includes conversion logic (e.g. JToken -> target field type, Guid parsing).
    /// - Intended as a lightweight transfer object used by repositories/serializers to separate
    ///   persistence concerns from runtime Behaviour implementations.
    /// </summary>
    [Serializable]
    public class SaveableBehaviour : ISaveableBehaviour
    {
        public Guid Id { get; set; }
        public Dictionary<string, object> SaveableFields { get; set; } = new Dictionary<string, object>();
        public Type BehaviourType { get; set; }

        public SaveableBehaviour() {
        
        }

        public SaveableBehaviour(Type targetType, IEmberBehaviour targetBehaviour)
        {
            Id = targetBehaviour.Id;
            //There may have been issues serializing Type directly.
            BehaviourType = targetType;
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

        public bool ApplySavedFields(Type targetType, IEmberBehaviour instance)
        {
            //Using TargetType to avoid issues with Type serialization.
            if (Id == instance.Id)
            {
                foreach (KeyValuePair<string, object> savedField in SaveableFields)
                {
                    FieldInfo field = targetType.GetField(savedField.Key, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
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
                            if (field.FieldType == typeof(System.Guid) && value is string guidStr)
                            {
                                try
                                {
                                    value = Guid.Parse(guidStr);
                                    field.SetValue(instance, value);
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine($"Error parsing Guid for field {field.Name}: {ex.Message}");
                                    return false;
                                }
                            }
                            else
                            {
                                field.SetValue(instance, value);
                            }
                        }
                    }
                }
                return true;
            }

            return false;
        }
    }
}