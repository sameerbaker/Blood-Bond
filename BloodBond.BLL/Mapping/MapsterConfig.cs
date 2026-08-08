using BloodBond.DAL.DTO.Request;
using BloodBond.DAL.DTO.Response;
using BloodBond.DAL.Models;
using Mapster;

namespace BloodBond.BLL.Mapping
{
    
    public static class MapsterConfig
    {
        public static void RegisterMappings()
        {
            
            TypeAdapterConfig<ApplicationUser, RegisterResponse>
                .NewConfig()
                .Map(dest => dest.UserId, src => src.Id)
                .Map(dest => dest.Email, src => src.Email ?? string.Empty)
                .Map(dest => dest.FullName, src => src.FullName);
        }
    }
}
