namespace Models
{
    public class UsersResponse : PagedCollection<User>
    {
        public Form Register { get; set; }

        public Link Me { get; set; }
    }
}
