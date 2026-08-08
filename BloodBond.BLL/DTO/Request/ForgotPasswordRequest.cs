using System.ComponentModel.DataAnnotations;

namespace BloodBond.DAL.DTO.Request
{
    public class ForgotPasswordRequest
    {
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;
    }
}
