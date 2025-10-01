using System.ComponentModel.DataAnnotations;

namespace TrackPoint.Models
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }
}