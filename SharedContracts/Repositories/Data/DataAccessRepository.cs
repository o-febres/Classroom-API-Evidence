using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Humanizer;
using MongoDB.Driver;
using SharedContracts.Interfaces;

namespace SharedContracts.Repositories.Data
{
    public abstract class DataAccessRepository<TImplementation, TInterface> : IRepository<TImplementation, TInterface>
        where TInterface : IEntity where TImplementation : TInterface,
        new()
    {
        private readonly IMongoCollection<TImplementation> _collection;

        protected DataAccessRepository()
        {
            var settings = new MongoClientSettings
            {
                Servers = new List<MongoServerAddress>
                {
                    new MongoServerAddress("londattst01"),
                    new MongoServerAddress("londattst02"),
                    new MongoServerAddress("londattst03")
                },
                Credential = MongoCredential.CreateCredential("admin", "mongo_admin", "j8rjWucfUuK3UXg8"),
                UseSsl = true,
                SslSettings = new SslSettings
                {
                    ClientCertificates = new List<X509Certificate>
                    {
                        new X509Certificate2(@"C:\mdb\mongodb.pem", "l0rdw3asel")
                    },
                    ClientCertificateSelectionCallback =
                        (sender, host, certificates, certificate, issuers) => certificates[0],
                    ServerCertificateValidationCallback = (sender, certificate, chain, errors) => true
                }
            };

            var client = new MongoClient(settings);
            var database = client.GetDatabase("ClassroomApi");

            var collectionName = typeof(TImplementation).Name.Pluralize();
            _collection = database.GetCollection<TImplementation>(collectionName);
        }

        public void Create(TInterface item)
        {
            _collection.InsertOne((TImplementation) item);
        }

        public TInterface Read(long id)
        {
            try
            {
                return _collection.Find(_ => _.Id == id).First();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public IEnumerable<TInterface> List(FilterDefinition<TImplementation> filter = null,
            SortDefinition<TImplementation> sort = null, int pageSize = 0, int page = 0)
        {
            var options = new FindOptions
            {
                BatchSize = pageSize
            };
            if (filter == null) filter = FilterDefinition<TImplementation>.Empty;

            return _collection.Find(filter, options).Sort(sort).Skip(pageSize * (page - 1)).Limit(pageSize).ToList()
                .Cast<TInterface>();
        }


        public void Update(TInterface replacement)
        {
            _collection.ReplaceOne(Builders<TImplementation>.Filter.Eq(e => e.Id, replacement.Id),
                (TImplementation) replacement);
        }

        public void Update(long id, IDictionary<string, object> updatedFields)
        {
            var updateQuery =
                Builders<TImplementation>.Update.Combine(updatedFields.Select(field =>
                    Builders<TImplementation>.Update.Set(field.Key, field.Value)));
            _collection.UpdateOne(Builders<TImplementation>.Filter.Eq(e => e.Id, id), updateQuery);
        }

        public void Delete(long id)
        {
            _collection.DeleteOne(Builders<TImplementation>.Filter.Eq("Id", id));
        }
    }
}