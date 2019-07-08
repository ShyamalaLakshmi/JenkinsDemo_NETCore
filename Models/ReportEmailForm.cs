using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class ReportEmailForm
    {
        [Required]
        [EmailAddress]
        [Display(Name = "email", Description = "Email")]
        public string Email { get; set; }
    }
}
