namespace Models
{
    public class EventStatusResponse : PagedCollection<EventStatus>
    {
        public Form EventsQuery { get; set; }
    }
}