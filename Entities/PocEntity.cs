using System;

namespace Entities
{
    public class PocEntity
    {
        public Guid Id { get; set; }
        public long PocId { get; set; }
        public string PocName { get; set; }
        public string PocContactNumber { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ModifiedAt { get; set; }

        public EventEntity Event { get; set; }
    }
}
