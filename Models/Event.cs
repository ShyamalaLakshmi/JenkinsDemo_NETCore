using System;
using System.Collections.Generic;

namespace Models
{
    public class Event : Resource
    {
        public Guid Id { get; set; }
        [SearchableString]
        public string EventId { get; set; }
        [SearchableString]
        public string EventName { get; set; }
        public string EventDescription { get; set; }
        [SearchableDateTime]
        public DateTime EventDate { get; set; }
        [SearchableString]
        public string BeneficiaryName { get; set; }
        public decimal TotalVolunteerHours { get; set; }
        public decimal TotalTravelHours { get; set; }
        public decimal OverallVolunteeringHours { get; set; }
        public int LivesImpacted { get; set; }
        [SearchableString]
        public string Status { get; set; }
        public string CouncilId { get; set; }
        public string FileId { get; set; }
        public int ActivityType { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime ModifiedAt { get; set; }
        [SearchableString]
        public string Month { get; set; }
        [SearchableString]
        public string BaseLocation { get; set; }
        public string VenueAddress { get; set; }
        [SearchableString]
        public string CouncilName { get; set; }
        [SearchableString]
        public string Project { get; set; }
        public string Category { get; set; }
        public int TotalNoOfVolunteers { get; set; }
        public decimal AverageRating { get; set; }
        [SearchableString]
        public string BusinessUnit { get; set; }


        public IEnumerable<Poc> Poc { get; set; }
        public IEnumerable<Participant> Participant { get; set; }
        public IEnumerable<Feedback> Feedback { get; set; }

    }
}
