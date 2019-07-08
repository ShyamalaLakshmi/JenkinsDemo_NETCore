using System;
using System.Collections.Generic;

namespace Models
{
    public class Question : Resource
    {
        public Guid Id { get; set; }
        //public Link AnswersLink { get; set; }
        public string Description { get; set; }
        public bool AllowMultipleAnswer { get; set; }
        public bool FreeTextQuestion { get; set; }
        public bool CustomQuestion { get; set; }
        public string FeedbackType { get; set; }
        public bool Active { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset ModifiedAt { get; set; }

        public IEnumerable<Answer> Answers { get; set; }
    }
}
