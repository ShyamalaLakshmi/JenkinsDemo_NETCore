using System;

namespace Models
{
    public class User : Resource
    {
        public Guid Id { get; set; }

        public string Email { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTimeOffset CreatedAt { get; set; }
    }
}
