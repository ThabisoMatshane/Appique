using System.ComponentModel.DataAnnotations;

namespace Appique_v2.Models
{
    public class ContactFormModel
    {
        [Required(ErrorMessage = "Your name is required.")]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email address is required.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        [StringLength(200)]
        public string Email { get; set; } = string.Empty;

        [Phone]
        [StringLength(30)]
        public string? Phone { get; set; }

        [StringLength(150)]
        public string? Company { get; set; }

        [StringLength(100)]
        public string? Service { get; set; }

        [Required(ErrorMessage = "Please tell us about your project.")]
        [StringLength(3000)]
        public string Message { get; set; } = string.Empty;
    }
}
