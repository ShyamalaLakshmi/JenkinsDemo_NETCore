using System;
using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class ReasonForm
    {
        [Required]
        [Display(Name = "questionId", Description = "Question ID")]
        public Guid QuestionId { get; set; }

        [Display(Name = "answerId", Description = "Answer ID")]
        public string AnswerId { get; set; }
    }
}
