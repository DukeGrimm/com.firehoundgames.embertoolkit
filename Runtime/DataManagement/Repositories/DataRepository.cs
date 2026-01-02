using EmberToolkit.Common.Interfaces.Data;
using EmberToolkit.Common.Interfaces.Repository;
using EmberToolkit.Common.Interfaces.Serialization;
using EmberToolkit.DataManagement.Data;
using EmberToolkit.DataManagement.Serializers.Converters;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace EmberToolkit.DataManagement.Repositories
{
    public class DataRepository : IDataRepository
    {
        private IStorageContainer dataBox;
        private readonly IDataSerializer _dataSerializer;

        public DataRepository(IDataSerializer dataSerializer)
        {
            dataBox = new StorageContainer();
            _dataSerializer = dataSerializer;
        }

        public void AddRepository<T>(Guid id, Dictionary<Guid, T> data) => dataBox.RepositoryContainer[id] = data;

        public void RemoveRepository<T>(Guid id) => dataBox.RepositoryContainer.Remove(id);

        public void Save(string name) => _dataSerializer.SerializeData(dataBox, name);

        public void Load(string name)
        {
            var loadedContainer = _dataSerializer.DeserializeData(name);
            dataBox = loadedContainer ?? dataBox;
        }

        public Dictionary<Guid, T>? GetRepositoryData<T>(Guid id)
        {
            if (!dataBox.RepositoryContainer.TryGetValue(id, out var repositoryData))
                return null;

            // If the stored value is already the right type, return it.
            if (repositoryData is Dictionary<Guid, T> typedDict)
            {
                return typedDict;
            }

            var json = repositoryData.ToString();
            return _dataSerializer.ConvertObject<Dictionary<Guid,T>>(json);
        }

        public void SaveObject<T>(T objData, string filePath) => _dataSerializer.SerializeObject(objData, filePath);
        public T LoadObject<T>(string filePath) => _dataSerializer.DeserializeObject<T>(filePath);
        public List<T> LoadAllObjects<T>(string extension) => _dataSerializer.DeserializeAllObjects<T>(extension);

        public void AddDictionaryConvertor<T>() => _dataSerializer.AddDictionaryConvertor<T>();
    }
}
