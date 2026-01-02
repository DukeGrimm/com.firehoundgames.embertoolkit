using EmberToolkit.Common.Interfaces.Configuration;
using EmberToolkit.Common.Interfaces.Data;
using EmberToolkit.Common.Interfaces.Encryption;
using EmberToolkit.Common.Interfaces.Serialization;
using EmberToolkit.DataManagement.Data;
using EmberToolkit.DataManagement.Serializers.Converters;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace EmberToolkit.DataManagement.Serializers
{
    public class EmberJsonSerializer : IDataSerializer
    {
        private readonly string _saveFileLocation;
        private IAESController _aesController;
        private bool useEncrypt = false;

        // expose and reuse these settings across the codebase
        public JsonSerializerSettings _settings { get; }

        public EmberJsonSerializer(IEmberSettings settings, IAESController aesController)
        {
            _saveFileLocation = settings.SavePath;
            _aesController = aesController;
            useEncrypt = settings.useEncryption;

            // configure centralized settings (add converters here)
            var s = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.None // prefer explicit ItemType in payloads
            };

            _settings = s;

            ValidateDirectory();
        }

        public bool ConverterCheck<T>()
        {
            if (_settings != null && _settings.Converters != null)
            {
                foreach (var converter in _settings.Converters)
                {
                    if (converter is T)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private void SerializeData<T>(T data, string name)
        {
            string filePath = GetFilePath(name);
            string payload = JsonConvert.SerializeObject(data, Formatting.None);
            if(useEncrypt) payload = _aesController.EncryptData(payload);
            File.WriteAllText(filePath, payload);
        }

        private T DeserializeData<T>(string name)
        {
            string filePath = GetFilePath(name);
            if (File.Exists(filePath))
            {
                string jsonData = File.ReadAllText(filePath);
                if(useEncrypt) jsonData = _aesController.DecryptData(jsonData);
                return JsonConvert.DeserializeObject<T>(jsonData);
            }
            return default;
        }

        private string GetFilePath(string name)
        {
            return Path.Combine(_saveFileLocation, name);
        }
        public IStorageContainer? DeserializeData(string name)
        {
            return DeserializeData<StorageContainer>(name);
        }

        public void SerializeData(IStorageContainer dataContainer, string name)
        {
            SerializeData<IStorageContainer>(dataContainer, name);
        }

        public void SerializeObject<T>(T objData, string filePath)
        {
            SerializeData<T>(objData,filePath);
        }
        public T DeserializeObject<T>(string filePath)
        {
           return DeserializeData<T>(filePath);
        }

        public List<T> DeserializeAllObjects<T>(string extension)
        {
            var objects = new List<T>();
            var files = Directory.GetFiles(_saveFileLocation, "*." + extension);

            foreach (var file in files)
            {
                var obj = DeserializeObject<T>(file);
                objects.Add(obj);
            }

            return objects;
        }

        private void ValidateDirectory()
        {
            if (!Directory.Exists(_saveFileLocation))
            {
                Directory.CreateDirectory(_saveFileLocation);
            }
        }

        public void AddDictionaryConvertor<T>()
        {
            if (!ConverterCheck<T>())
            {
                // Add converter to settings
                _settings.Converters.Add(new DictionaryJsonConverter<T>());
            }
        }
        public T ConvertObject<T>(string jsonData)
        {
            return JsonConvert.DeserializeObject<T>(jsonData, _settings);
        }
    }
}
