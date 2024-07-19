using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace EmberToolkit.DataManagement.Serializers.Converters
{    public class PropertyConverter<T> : JsonConverter<T>
    {
        public override T ReadJson(JsonReader reader, Type objectType, T existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return default!;

            JObject obj = JObject.Load(reader);
            T instance = Activator.CreateInstance<T>();

            foreach (PropertyInfo property in typeof(T).GetProperties())
            {
                if (obj.TryGetValue(property.Name, out JToken value))
                {
                    object convertedValue = value.ToObject(property.PropertyType, serializer);
                    property.SetValue(instance, convertedValue);
                }
            }

            return instance;
        }

        public override void WriteJson(JsonWriter writer, T value, JsonSerializer serializer)
        {
            JObject obj = new JObject();

            foreach (PropertyInfo property in typeof(T).GetProperties())
            {
                object propertyValue = property.GetValue(value);
                obj.Add(property.Name, JToken.FromObject(propertyValue, serializer));
            }

            obj.WriteTo(writer);
        }
    }
}
