using System;

namespace Entities
{
    public class ParticipantEntity
    {
        public Guid Id { get; set; }
        public string EventId { get; set; }
        public string EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string Designation { get; set; }
        public string BeneficiaryName { get; set; }
        public string Location { get; set; }
        public string EventDate { get; set; }
        //Can go with enum
        public bool NotAttended { get; set; }
        //Can go with enum
        public bool Unregistered { get; set; }
        //Can go with enum
        public bool Attended { get; set; }
        public bool IsEmailSent { get; set; }
        public DateTime MailSentAt { get; set; }
        public bool IsFeedbackReceived { get; set; }
        public Guid? FeedbackId { get; set; }
        public string FileId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ModifiedAt { get; set; }

        public EventEntity Event { get; set; }
    }
}
