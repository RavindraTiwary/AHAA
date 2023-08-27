using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace AHAApi.DataModels
{
    public class InterviewProfiles
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public string? JobId { get; set; }
        public string? JobTitle { get; set; }
        public string? JobDescription { get; set; }
        public string? CandidateName { get; set; }
        public string? CandidateEmailId { get; set; }
        public string? CandidateSelectedSlot { get; set; }
        public string? InterviewerName { get; set; }
        public string? InterviewerEmailId { get; set; }
        public string? InterviewSlot1 { get; set; }
        public string? InterviewSlot2 { get; set; }
        public string? InterviewSlot3 { get; set; }
    }
}
