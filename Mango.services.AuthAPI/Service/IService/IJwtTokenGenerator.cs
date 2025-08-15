using Mango.services.AuthAPI.Models;

namespace Mango.services.AuthAPI.Service.IService;

public interface IJwtTokenGenerator
{
    //Creating an interface to generate the JWT including the roles of the user
    string GenerateToken(ApplicationUser applicationUser, IEnumerable<string> roles);
}
