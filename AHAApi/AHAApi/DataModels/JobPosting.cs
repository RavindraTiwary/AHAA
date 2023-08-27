using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace AHAApi.DataModels
{
    public class JobPosting
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }

        public string Tags { get; set; }

        public string PostedOn { get; set; }

        public bool IsPositionActive { get; set; }
        public int Status { get; set; }
    }
}
