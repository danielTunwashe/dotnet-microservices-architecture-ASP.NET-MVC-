using Mango.services.AuthAPI.Models;
using Mango.services.AuthAPI.Service.IService;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Mango.services.AuthAPI.Service;

public class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly JwtOptions _jwtOptions;

    public JwtTokenGenerator(IOptions<JwtOptions> jwtOptions)
    {
        _jwtOptions = jwtOptions.Value;
    }

    public string GenerateToken(ApplicationUser applicationUser, IEnumerable<string> roles)
    {
        var tokenHandler = new JwtSecurityTokenHandler();

        //We have the secret key
        var key = Encoding.ASCII.GetBytes(_jwtOptions.Secret);


        // we have the claim list
        var claimList = new List<Claim>
        {   //Compulsory three claims that should be stored in JWT token
            new Claim(JwtRegisteredClaimNames.Email, applicationUser.Email),
            new Claim(JwtRegisteredClaimNames.Sub, applicationUser.Id),
            new Claim(JwtRegisteredClaimNames.Name, applicationUser.UserName),
        };


        //Adding the roles of the particular user..
        claimList.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));


        //We need a token descriptor
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Audience = _jwtOptions.Audience,
            Issuer = _jwtOptions.Issuer,
            Subject = new ClaimsIdentity(claimList),
            Expires = DateTime.UtcNow.AddMinutes(1), // Token will be valid for 7 days
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        //Finally we need to generate the token..
        var token = tokenHandler.CreateToken(tokenDescriptor); 

        return tokenHandler.WriteToken(token);
    }
}
