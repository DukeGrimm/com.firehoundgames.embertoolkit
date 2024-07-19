using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmberToolkit.DataManagement.Serializers.Converters
{
    public class DictionaryJsonConverter<T> : JsonConverter<Dictionary<Guid, T>>
    {
        public override Dictionary<Guid, T> ReadJson(JsonReader reader, Type objectType, Dictionary<Guid, T> existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var dictionary = new Dictionary<Guid, T>();

            if (reader.TokenType == JsonToken.StartObject)
            {
                JObject jsonObject = JObject.Load(reader);

                foreach (var property in jsonObject.Properties())
                {
                    Guid key = Guid.Parse(property.Name);
                    JObject itemObject = (JObject)property.Value;

                    // Get the ItemType value from the itemObject
                    string itemType = itemObject.Value<string>("ItemType");

                    // Deserialize the item using the ItemType value
                    T item = (T)itemObject.ToObject(Type.GetType(itemType), serializer);

                    dictionary.Add(key, item);
                }
            }

            return dictionary;
        }

        public override void WriteJson(JsonWriter writer, Dictionary<Guid, T> value, JsonSerializer serializer)
        {
            // Writing JSON is not required for this example, so not implemented
            throw new NotImplementedException();
        }
    }
}
