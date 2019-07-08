using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class QuestionForm
    {
        [Display(Name = "id", Description = "Id")]
        public Guid? Id { get; set; }

        [Required]
        [Display(Name = "description", Description = "Description")]
        public string Description { get; set; }

        [Display(Name = "allowMultipleAnswer", Description = "Allow Multiple Answers")]
        public bool AllowMultipleAnswer { get; set; }

        [Display(Name = "freeTextQuestion", Description = "Free Text Question")]
        public bool FreeTextQuestion { get; set; }

        [Display(Name = "customQuestion", Description = "Custom Question")]
        public bool CustomQuestion { get; set; }

        [Required]
        [Display(Name = "feedbackType", Description = "Feedback Type")]
        public string FeedbackType { get; set; }

        [Display(Name = "active", Description = "Active")]
        public bool Active { get; set; }

        [Display(Name = "answers", Description = "Answers")]
        public IEnumerable<AnswerForm> Answers { get; set; }
    }
}
