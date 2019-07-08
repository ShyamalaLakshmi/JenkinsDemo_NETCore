using System;
using System.Collections.Generic;

namespace Entities
{
    public class EventEntity
    {
        public Guid Id { get; set; }
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
        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime ModifiedAt { get; set; }
        public string Month { get; set; }
        public string BaseLocation { get; set; }
        public string VenueAddress { get; set; }
        public string CouncilName { get; set; }
        public string Project { get; set; }
        public string Category { get; set; }
        public int TotalNoOfVolunteers { get; set; }
        public string BusinessUnit { get; set; }

        public IEnumerable<PocEntity> Poc { get; set; }
        public IEnumerable<ParticipantEntity> Participant { get; set; }
        public IEnumerable<FeedbackEntity> Feedback { get; set; }
    }
}
