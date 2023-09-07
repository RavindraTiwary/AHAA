using AHAApi.DataModels;
using AHAApi.Repository.Interfaces;
using MongoDB.Bson;
using MongoDB.Driver;

namespace AHAApi.Repository
{
    public class JobPostingRepository
    {
        private readonly IMongoCollection<JobPosting> _collection;

        public JobPostingRepository(IMongoDatabase database)
        {
            _collection = database.GetCollection<JobPosting>("jobPostings");
        }

        // Create a new job application
        public async Task<string> CreateAsync(JobPosting JobPosting)
        {
            await _collection.InsertOneAsync(JobPosting);
            return JobPosting?.Id?.ToString() ?? string.Empty;
        }

        // Read a job application by its ID
        public async Task<JobPosting> GetByIdAsync(string id)
        {
            var filter = Builders<JobPosting>.Filter.Eq(j => j.Id, id);
            return await _collection.Find(filter).FirstOrDefaultAsync();
        }

        // Read all job applications
        public async Task<List<JobPosting>> GetAllAsync()
        {
            return await _collection.Find(_ => true).ToListAsync();
        }

        // Update a job application
        public async Task UpdateAsync(JobPosting JobPosting)
        {
            var filter = Builders<JobPosting>.Filter.Eq(j => j.Id, JobPosting.Id);
            await _collection.ReplaceOneAsync(filter, JobPosting);
        }

        // Delete a job application by its ID
        public async Task DeleteAsync(string id)
        {
            var filter = Builders<JobPosting>.Filter.Eq(j => j.Id, id);
            await _collection.DeleteOneAsync(filter);
        }

    }
}
