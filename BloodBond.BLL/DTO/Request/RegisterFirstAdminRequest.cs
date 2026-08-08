using System.ComponentModel.DataAnnotations;

namespace BloodBond.DAL.DTO.Request
{
    public class RegisterFirstAdminRequest
    {
        [Required]
        public string SecretKey { get; set; } = string.Empty;

        [Required, MaxLength(80)]
        public string FullName { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required, MinLength(8)]
        public string Password { get; set; } = string.Empty;
    }
}
