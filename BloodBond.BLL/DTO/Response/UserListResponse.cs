using System;

namespace BloodBond.DAL.DTO.Response
{
    public class UserListResponse
    {
        public string Id { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public bool IsBlocked { get; set; }
        public DateTime CreatedAt { get; set; }
        public string[] Roles { get; set; } = Array.Empty<string>();
    }
}
