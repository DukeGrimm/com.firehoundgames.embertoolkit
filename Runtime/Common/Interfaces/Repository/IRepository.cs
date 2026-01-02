using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmberToolkit.Common.Interfaces.Repository
{
    public interface IRepository<T> : IEmberObject
    {
        //// Adds a new entity to the repository
        //protected void Add(T entity);

        //// Retrieves an entity by its unique identifier
        //protected T? Get(Guid id);
        //protected S? Get<S>(Guid id);
        //// Retrieves entities matching the provided filter
        //protected IEnumerable<T> GetWhere(Func<T, bool> filter);

        //// Retrieves all entities in the repository
        //protected IEnumerable<T> GetAll();
        //protected IEnumerable<S> GetAll<S>();

        //// Updates an existing entity in the repository
        //protected void Update(T entity);

        //// Deletes an entity from the repository
        //protected void Delete(Guid id);
        bool ShouldSave { get; }

        void Save();
        void Load();
    }
}
