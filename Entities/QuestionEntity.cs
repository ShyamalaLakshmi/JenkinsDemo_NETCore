using System;
using System.Collections.Generic;

namespace Entities
{
    public class QuestionEntity
    {
        public Guid Id { get; set; }
        public string Description { get; set; }
        public bool AllowMultipleAnswer { get; set; }
        public bool FreeTextQuestion { get; set; }
        public bool CustomQuestion { get; set; }
        public string FeedbackType { get; set; }
        public bool Active { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset ModifiedAt { get; set; }

        public UserEntity User { get; set; }
        public IEnumerable<AnswerEntity> Answers { get; set; }
    }
}
