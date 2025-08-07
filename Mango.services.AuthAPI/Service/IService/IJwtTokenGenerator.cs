using Mango.services.AuthAPI.Models;

namespace Mango.services.AuthAPI.Service.IService;

public interface IJwtTokenGenerator
{
    string GenerateToken(ApplicationUser applicationUser);
}
