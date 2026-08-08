using System.ComponentModel.DataAnnotations;

namespace BloodBond.DAL.DTO.Request
{
    public class ChangeRoleRequest
    {
        [Required]
        public string Role { get; set; } = string.Empty; // User, BloodBankManager, Admin
    }
}
