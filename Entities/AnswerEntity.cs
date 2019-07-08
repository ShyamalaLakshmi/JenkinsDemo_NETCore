using System;

namespace Entities
{
    public class AnswerEntity
    {
        public Guid Id { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; }
        public bool Active { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset ModifiedAt { get; set; }

        public UserEntity User { get; set; }
        public QuestionEntity Question { get; set; }
    }
}
