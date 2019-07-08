namespace Models
{
    public class EventResponse : PagedCollection<Event>
    {
        public Link Pocs { get; set; }
        public Link Participants { get; set; }
        public Form EventsQuery { get; set; }
    }
}
