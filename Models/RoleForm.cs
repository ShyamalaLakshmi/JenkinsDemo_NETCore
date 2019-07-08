using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class RoleForm
    {
        [Required]
        [EmailAddress]
        [Display(Name = "email", Description = "Emails")]
        public string Email { get; set; }

        [Display(Name = "add", Description = "Add")]
        public bool Add { get; set; }
    }
}
