using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class FeedbackForm
    {
        [Required]
        [Display(Name = "userId", Description = "User ID")]
        public Guid ParticipantId { get; set; }

        [Required]
        [Display(Name = "feedbackType", Description = "Feedback Type")]
        public string FeedbackType { get; set; }

        [Required]
        [Display(Name = "eventId", Description = "Event ID")]
        public Guid EventId { get; set; }

        [Display(Name = "reason", Description = "Reason")]
        public IEnumerable<ReasonForm> Reason { get; set; }
    }
}
