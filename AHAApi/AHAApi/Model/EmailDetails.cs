namespace AHAApi.Model
{
    public class EmailDetails
    {
        public string EmailSubject { get; set; }
        public string EmailBody { get; set; }
        public string EmailId { get; set; }
        public string InterviewerEmailId { get; set; }
        public string CandidateSelectedSlot { get; set; }
        public string SchedulingUrl { get; set; }
        public string InterviewProfileId { get; set; }
        public string InterviewerName { get; set; }
        public string CandidateName { get; set; }
        public bool IsRecipientInterviewer { get; set; }
        public bool ToBeInterviewScheduled { get; set; }
        public string JobTitle { get; set; }
        public string JobId { get; set; }
        public string JobDescription { get; set; }
    }
}
