namespace Models
{
    public class RootResponse : Resource
    {
        //public Link Info { get; set; }

        public Link Dashboard { get; set; }

        public Link Questions { get; set; }

        public Link Users { get; set; }

        public Link Feedback { get; set; }

        public Link Events { get; set; }

        public Link Reports { get; set; }

        public Link UserInfo { get; set; }

        public Form Token { get; set; }
    }
}
