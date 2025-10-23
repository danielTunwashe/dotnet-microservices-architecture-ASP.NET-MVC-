using Mango.services.AuthAPI.Models.Dto;

namespace Mango.services.AuthAPI.Service.IService;

public interface IAuthService
{
    Task<string> Register(RegistrationRequestDto registrationRequestDto);
    Task<LoginResponseDto> Login(LoginRequestDto loginRequestDto);

    Task<bool> AssignRole(string email, string roleName);
    Task<GetUserByIdOutput> GetUserById(GetUserByIdInput input);
}
