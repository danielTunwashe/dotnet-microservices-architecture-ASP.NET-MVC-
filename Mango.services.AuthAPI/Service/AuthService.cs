using Mango.services.AuthAPI.Data;
using Mango.services.AuthAPI.Models;
using Mango.services.AuthAPI.Models.Dto;
using Mango.services.AuthAPI.Service.IService;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Mango.services.AuthAPI.Service;

public class AuthService : IAuthService
{
    private readonly AppDbContext _dbContext;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;


    public AuthService(AppDbContext dbContext, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IJwtTokenGenerator jwtTokenGenerator)
    {
        _dbContext = dbContext;
        _userManager = userManager;
        _roleManager = roleManager;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<bool> AssignRole(string email, string roleName)
    {
        //First retrieve the user from the database using the email
        var user = await _dbContext.ApplicationUsers.FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());

        if (user != null)
        {
            //If the roles does not exist then we create one..
            if (!_roleManager.RoleExistsAsync(roleName).GetAwaiter().GetResult())
            {
                //Create the role using the role manager
                _roleManager.CreateAsync(new IdentityRole(roleName)).GetAwaiter().GetResult();
            }
            //assugn the role to the user..
            await _userManager.AddToRoleAsync(user, roleName);
            return true;
        }
        return false;
        
    }

    public async Task<LoginResponseDto> Login(LoginRequestDto loginRequestDto)
    {
        //First retrieve the user from the database using the UserName
        var user = await _dbContext.ApplicationUsers.FirstOrDefaultAsync(u => u.UserName.ToLower() == loginRequestDto.UserName.ToLower());
        //since user is valid check their passwords..
        bool isValid = await _userManager.CheckPasswordAsync(user, loginRequestDto.Password);

        if(user == null ||  isValid == false)
        {
            return new LoginResponseDto() { User = null, Token = "" };
        }

        //If the user is valid then we need to generate a token for them
        var token = _jwtTokenGenerator.GenerateToken(user);

        UserDto userDto = new UserDto()
        {
            ID = user.Id,
            Email = user.Email,
            Name = user.Name,
            PhoneNumber = user.PhoneNumber
        };

        LoginResponseDto loginResponseDto = new LoginResponseDto()
        {
            User = userDto,
            Token = token // Here you would generate a JWT token or similar
        };

        return loginResponseDto;
    }

    public async Task<string> Register(RegistrationRequestDto registrationRequestDto)
    {
        ApplicationUser user = new()
        {
            UserName = registrationRequestDto.Email,
            Email = registrationRequestDto.Email,
            NormalizedEmail = registrationRequestDto.Email.ToUpper(),
            Name = registrationRequestDto.Name,
            PhoneNumber = registrationRequestDto.PhoneNumber
        };

        try
        {
            //Using userManager to create the user which expect an Identity user
            //And a paswword 
            var result = await  _userManager.CreateAsync(user, registrationRequestDto.Password);
            if (result.Succeeded)
            {
                var userToReturn = _dbContext.ApplicationUsers.First(u => u.UserName == registrationRequestDto.Email);

                UserDto userDto = new()
                {
                    ID = userToReturn.Id,
                    Email = userToReturn.Email,
                    Name = userToReturn.Name,
                    PhoneNumber = userToReturn.PhoneNumber
                };

                return "";
            }
            else
            {
                //Return the errors from the identity user..
                return result.Errors.FirstOrDefault().Description;
            }
        }
        catch (Exception ex)
        {

            
        }
        return "Error Encountered...";
    }
}
