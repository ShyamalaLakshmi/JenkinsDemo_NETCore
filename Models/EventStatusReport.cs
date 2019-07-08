using System;

namespace Models
{
    public class EventStatusReport
    {
        public string EventId { get; set; }
        public string EventName { get; set; }
        public DateTime EventDate { get; set; }
        public string Month { get; set; }
        public string BeneficiaryName { get; set; }
        public string BaseLocation { get; set; }
        public string CouncilName { get; set; }
        public string Project { get; set; }
        public decimal AverageRating { get; set; }
        public int TotalParticipants { get; set; }
        public int TotalAttended { get; set; }
        public int TotalNotAttended { get; set; }
        public int TotalUnregistered { get; set; }
        public int TotalFeedbackReceived { get; set; }
        public int TotalAttendedFeedback { get; set; }
        public int TotalNotAttendedFeedback { get; set; }
        public int TotalUnregisteredFeedback { get; set; }
        public string BusinessUnit { get; set; }
    }
}
