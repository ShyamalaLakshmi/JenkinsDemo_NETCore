using System;

namespace Models
{
    public class Feedback
    {
        public Guid Id { get; set; }
        public Guid ReasonId { get; set; }
        public int Rating { get; set; }
        public string Pros { get; set; }
        public string Cons { get; set; }
        public bool Attended { get; set; }
        public bool NotAttended { get; set; }
        public bool Unregistered { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ModifiedAt { get; set; }
    }
}
