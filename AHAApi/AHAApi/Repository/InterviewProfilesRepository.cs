using AHAApi.DataModels;
using AHAApi.Repository.Interfaces;
using MongoDB.Driver;

namespace AHAApi.Repository
{
    public class InterviewProfilesRepository
    {
        private readonly IMongoCollection<InterviewProfiles> _collection;

        public InterviewProfilesRepository(IMongoDatabase database)
        {
            _collection = database.GetCollection<InterviewProfiles>("interviewProfiles");
        }

        // Create a new job application
        public async Task CreateAsync(InterviewProfiles InterviewProfiles)
        {
            await _collection.InsertOneAsync(InterviewProfiles);
        }

        // Read a job application by its ID
        public async Task<InterviewProfiles> GetByIdAsync(string id)
        {
            var filter = Builders<InterviewProfiles>.Filter.Eq(j => j.Id, id);
            return await _collection.Find(filter).FirstOrDefaultAsync();
        }

        // Read all job applications
        public async Task<List<InterviewProfiles>> GetAllAsync()
        {
            return await _collection.Find(_ => true).ToListAsync();
        }

        // Update a job application
        public async Task UpdateAsync(InterviewProfiles InterviewProfiles)
        {
            var filter = Builders<InterviewProfiles>.Filter.Eq(j => j.Id, InterviewProfiles.Id);
            await _collection.ReplaceOneAsync(filter, InterviewProfiles);
        }

        // Delete a job application by its ID
        public async Task DeleteAsync(string id)
        {
            var filter = Builders<InterviewProfiles>.Filter.Eq(j => j.Id, id);
            await _collection.DeleteOneAsync(filter);
        }

    }
}
