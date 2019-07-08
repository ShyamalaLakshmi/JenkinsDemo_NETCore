using System;

namespace Models
{
    public class EventReport
    {
        public string EventId { get; set; }
        public string EventName { get; set; }
        public string EventDescription { get; set; }
        public DateTime EventDate { get; set; }
        public string BeneficiaryName { get; set; }
        public decimal TotalVolunteerHours { get; set; }
        public decimal TotalTravelHours { get; set; }
        public decimal OverallVolunteeringHours { get; set; }
        public int LivesImpacted { get; set; }
        public string Status { get; set; }
        public string CouncilId { get; set; }
        public string FileId { get; set; }
        public int ActivityType { get; set; }
        public string Month { get; set; }
        public string BaseLocation { get; set; }
        public string VenueAddress { get; set; }
        public string CouncilName { get; set; }
        public string Project { get; set; }
        public string Category { get; set; }
        public int TotalNoOfVolunteers { get; set; }
        public decimal AverageRating { get; set; }
        public string BusinessUnit { get; set; }
    }
}
