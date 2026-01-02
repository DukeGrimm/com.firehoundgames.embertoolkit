using EmberToolkit.Common.Interfaces.Data;
using EmberToolkit.Common.Interfaces.Repository;
using System;
using System.Collections.Generic;

namespace EmberToolkit.DataManagement.Controllers
{
    public class SaveLoadController : ISaveLoadController, ISaveLoadEvents
    {
        private IDataRepository _dataRepo;
        //Events
        public event SaveLoadEventHandler OnSaveEvent;
        public event SaveLoadEventHandler OnLoadEvent;

        public Guid Id { get; }

        public SaveLoadController(IDataRepository dataRepo) {
            Id = Guid.NewGuid();
            _dataRepo = dataRepo;
        }

        public void Save(string fileName)
        {
            OnSaveEvent?.Invoke();
            _dataRepo.Save(fileName);
        }

        public void Load(string fileName)
        {
            _dataRepo.Load(fileName);
            OnLoadEvent?.Invoke();
        }

        public void SaveObject<T>(T objData, string filePath) => _dataRepo.SaveObject(objData, filePath);
        public T LoadObject<T>(string filePath) => _dataRepo.LoadObject<T>(filePath);
        public List<T> LoadAllObjects<T>(string extension) => _dataRepo.LoadAllObjects<T>(extension);
    }
}
