using EmberToolkit.Common.Interfaces.Data;
using EmberToolkit.Common.Interfaces.Repository;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EmberToolkit.DataManagement.Repositories
{
    public class Repository<T> : IRepository<T> where T : IEmberObject
    {
        private ISaveLoadEvents _saveLoadEvents;
        [ShowInInspector]
        protected Dictionary<Guid, T> _entities = new Dictionary<Guid, T>();

        public bool ShouldSave { get; private set; }

        public Repository(ISaveLoadEvents saveLoadEvents, bool shouldSave = true)
        {
            _saveLoadEvents = saveLoadEvents;
            ShouldSave = shouldSave;
            if (shouldSave)
            {
                _saveLoadEvents.OnSaveEvent += Save;
                _saveLoadEvents.OnLoadEvent += Load;
            }
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
        public void Load(IDataRepository dataRepo)
        {
           var loadedData = dataRepo.GetRepositoryData<T>();
            if (loadedData != null) _entities = loadedData;

        }
        /// <summary>
        /// Send Data to DataRepo to be saved.
        /// </summary>
        public virtual void Save(IDataRepository dataRepo) =>  dataRepo.AddRepository<T>(_entities);

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
