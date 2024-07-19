using EmberToolkit.Common.Interfaces.Data;
using EmberToolkit.Common.Interfaces.Repository;
using EmberToolkit.Common.Interfaces.Serialization;
using EmberToolkit.DataManagement.Data;
using EmberToolkit.DataManagement.Serializers.Converters;
using Newtonsoft.Json;
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

        public void AddRepository<T>(Dictionary<Guid, T> data) => dataBox.RepositoryContainer[typeof(T)] = data;

        public void RemoveRepository<T>() => dataBox.RepositoryContainer.Remove(typeof(T));

        public void Save(string name) => _dataSerializer.SerializeData(dataBox, name);

        public void Load(string name)
        {
            var loadedContainer = _dataSerializer.DeserializeData(name);
            dataBox = loadedContainer ?? dataBox;
        }

        public Dictionary<Guid, T>? GetRepositoryData<T>()
        {
            if (dataBox.RepositoryContainer.TryGetValue(typeof(T), out var repositoryData))
            {
                var json = repositoryData.ToString();
                var holder = new Dictionary<Guid, T>();
                var settings = new JsonSerializerSettings();
                settings.Converters.Add(new DictionaryJsonConverter<T>());
                return JsonConvert.DeserializeObject<Dictionary<Guid, T>>(json, settings);
            }

            return null;
        }

        public void SaveObject<T>(T objData, string filePath) => _dataSerializer.SerializeObject(objData, filePath);
        public T LoadObject<T>(string filePath) => _dataSerializer.DeserializeObject<T>(filePath);
        public List<T> LoadAllObjects<T>(string extension) => _dataSerializer.DeserializeAllObjects<T>(extension);
    }
}
