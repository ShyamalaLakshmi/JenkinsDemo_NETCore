using System;
using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class AnswerForm
    {
        [Display(Name = "id", Description = "Id")]
        public Guid? Id { get; set; }

        [Required]
        [Display(Name = "description", Description = "Description")]
        public string Description { get; set; }

        [Display(Name = "icon", Description = "Icon Class")]
        public string Icon { get; set; }

        [Display(Name = "active", Description = "Active")]
        public bool Active { get; set; }
    }
}
