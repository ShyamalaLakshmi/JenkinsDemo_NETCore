namespace Models
{
    public class QuestionResponse : PagedCollection<Question>
    {
        public Link Answers { get; set; }
        public Form QuestionsQuery { get; set; }
        public Form QuestionForm { get; set; }
    }
}
