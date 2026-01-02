using EmberToolkit.Common.Interfaces.Data;
using System.Collections.Generic;

namespace EmberToolkit.Common.Interfaces.Serialization
{
    public interface IDataSerializer
    {
        IStorageContainer? DeserializeData(string name);
        void SerializeData(IStorageContainer dataContainer, string name);
        void SerializeObject<T>(T objData, string filePath);
        T DeserializeObject<T>(string filePath);
        List<T> DeserializeAllObjects<T>(string extension);
        void AddDictionaryConvertor<T>();
        public T ConvertObject<T>(string jsonData);

    }
}
