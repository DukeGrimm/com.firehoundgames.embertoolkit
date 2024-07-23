using EmberToolkit.Common.Interfaces.Configuration;
using EmberToolkit.Common.Interfaces.Data;
using EmberToolkit.Common.Interfaces.Encryption;
using EmberToolkit.Common.Interfaces.Serialization;
using EmberToolkit.DataManagement.Data;
using Newtonsoft.Json;
using Sirenix.Serialization;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace EmberToolkit.DataManagement.Serializers
{
    public class EmberJsonSerializer : IDataSerializer
    {
        private readonly string _saveFileLocation;
        private IAESController _aesController;
        private bool useEncrypt = false;

        public EmberJsonSerializer(IEmberSettings settings, IAESController aesController)
        {
            _saveFileLocation = settings.SavePath;
            _aesController = aesController;
            useEncrypt = settings.useEncryption;
            ValidateDirectory();
        }

        private void SerializeData<T>(T data, string name)
        {
            string filePath = GetFilePath(name);
            byte[] payloadBytes = SerializationUtility.SerializeValue<T>(data, DataFormat.JSON);
            string payload = System.Text.Encoding.UTF8.GetString(payloadBytes);//JsonConvert.SerializeObject(data, Formatting.None);
            if (useEncrypt) payload = _aesController.EncryptData(payload);
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
    }
}
