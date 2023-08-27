using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace AHAApi.DataModels
{
    public class Candidate
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string FirstName { get; set; }
        public string Lastname { get; set; }
        public string ResumeLink { get; set; }
        public bool IsActive { get; set; }
        public DateTime LastUpdatedOn { get; set; }

    }
}