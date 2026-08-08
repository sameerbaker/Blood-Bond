using System.Collections.Generic;
using System.Threading.Tasks;
using BloodBond.DAL.DTO.Request;
using BloodBond.DAL.DTO.Response;

namespace BloodBond.BLL.Service
{
    public interface IUserManagementService
    {
        Task<RegisterResponse> RegisterFirstAdminAsync(RegisterFirstAdminRequest request);

        // Admin-only operations
        Task<RegisterResponse> CreateUserAsync(CreateUserByAdminRequest request);
        Task ChangePasswordAsync(string userId, ChangePasswordRequest request);
        Task<IEnumerable<UserListResponse>> GetAllUsersAsync();
        Task<UserListResponse?> GetUserByIdAsync(string userId);
        Task<UserListResponse> BlockUserAsync(string userId);
        Task<UserListResponse> UnblockUserAsync(string userId);
        Task<UserListResponse> ChangeRoleAsync(string userId, ChangeRoleRequest request);
    }
}
