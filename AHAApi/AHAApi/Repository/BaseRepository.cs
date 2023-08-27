namespace AHAApi
{
    using Microsoft.Extensions.Configuration;
    using MongoDB.Driver;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    public class BaseRepository<T> : IBaseRepository<T>
    {
        protected readonly IMongoCollection<T> _collection;

        public BaseRepository(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("CosmosDBConnection");
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase("your_database_name");
            _collection = database.GetCollection<T>("your_collection_name");
        }

        public async Task<T> GetByIdAsync(string id)
        {
            // Implement GetByIdAsync
            return Task.FromResult(default(T)).Result;
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            // Implement GetAllAsync

            return Task.FromResult(default(IEnumerable<T>)).Result;
        }

        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> filter)
        {
            // Implement FindAsync

            return Task.FromResult(default(IEnumerable<T>)).Result;
        }

        public async Task CreateAsync(T entity)
        {
            // Implement CreateAsync
        }

        public async Task UpdateAsync(string id, T entity)
        {
            // Implement UpdateAsync
        }

        public async Task DeleteAsync(string id)
        {
            // Implement DeleteAsync
        }

        Task<List<T>> IBaseRepository<T>.GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(T entity)
        {
            throw new NotImplementedException();
        }
    }

}
