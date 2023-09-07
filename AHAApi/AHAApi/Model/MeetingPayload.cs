namespace AHAApi.Model
{
    // Define a class to represent your meeting payload
    public class MeetingPayload
    {
        public string subject { get; set; }
        public bool recordAutomatically { get; set; }
        public bool allowTranscription { get; set; }
    }
}
