using EmberToolkit.Common.Interfaces.Data;
using EmberToolkit.Common.Interfaces.Repository;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EmberToolkit.DataManagement.Repositories
{
    public abstract class Repository<T> : IRepository<T> where T : IEmberObject
    {
        private ISaveLoadEvents _saveLoadEvents;
        private IDataRepository _dataRepo;
        [ShowInInspector]
        protected Dictionary<Guid, T> _entities = new Dictionary<Guid, T>();

        public bool ShouldSave { get; private set; }

        public Guid Id { get; private set; }

        public Repository(ISaveLoadEvents saveLoadEvents, IDataRepository dataRepo, bool shouldSave = true, Guid repoId = new Guid())
        {
            _saveLoadEvents = saveLoadEvents;
            _dataRepo = dataRepo;
            if (repoId != Guid.Empty)
            {
                Id = repoId;
                InitializeRepository(repoId);
            }
            else
            {
                Id = Guid.NewGuid();
                InitializeRepository(Id);
            }

            ShouldSave = shouldSave;
            if (shouldSave)
            {
                _saveLoadEvents.OnSaveEvent += Save;
                _saveLoadEvents.OnLoadEvent += Load;
            }
        }

        protected virtual void InitializeRepository(Guid id)
        {
            Id = id;
            _dataRepo.AddDictionaryConvertor<T>();
        }

        protected void Add(T entity) => _entities.Add(entity.Id, entity);

        protected void Delete(Guid id) => _entities.Remove(id);

        protected T Get(Guid id)
        {
            _entities.TryGetValue(id, out T entity);
            return entity;
        }

        protected S Get<S>(Guid id)
        {
            var obj = Get(id);
            if (obj == null) return default(S);
            return (S)Convert.ChangeType(obj, typeof(S));
        }

        protected IEnumerable<T> GetWhere(Func<T, bool> filter) => _entities.Values.Where(filter);

        protected IEnumerable<T> GetAll() => _entities.Values;
        protected IEnumerable<S> GetAll<S>() => _entities.Values.OfType<S>();
        /// <summary>
        /// Attempt to load data from DataRepo
        /// </summary>
        public void Load()
        {
           var loadedData = _dataRepo.GetRepositoryData<T>(Id);
            if (loadedData != null) _entities = loadedData;

        }
        /// <summary>
        /// Send Data to DataRepo to be saved.
        /// </summary>
        public virtual void Save() =>  _dataRepo.AddRepository<T>(Id, _entities);

        protected void Update(T entity)
        {
            if (_entities.ContainsKey(entity.Id))
            {
                _entities[entity.Id] = entity;
            }
            else
            {
                throw new ArgumentException("Entity not found in the repository.");
            }
        }



    }
}
