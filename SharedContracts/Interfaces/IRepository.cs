using System.Collections.Generic;
using MongoDB.Driver;

namespace SharedContracts.Interfaces
{
    public interface IRepository<TImplementation, TInterface> where TInterface : IEntity
    {
        // CRUD
        void Create(TInterface entity);
        TInterface Read(long id);

        IEnumerable<TInterface> List(FilterDefinition<TImplementation> filter = null,
            SortDefinition<TImplementation> sort = null, int pageSize = 0, int page = 0);

        void Update(TInterface replacement);
        void Update(long id, IDictionary<string, object> updatedFields);
        void Delete(long id);
    }
}