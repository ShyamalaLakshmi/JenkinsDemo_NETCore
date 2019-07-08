using System;
using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class ReminderEmailForm
    {
        [Required]
        [Display(Name = "eventId", Description = "Event ID")]
        public Guid EventId { get; set; }
    }
}
