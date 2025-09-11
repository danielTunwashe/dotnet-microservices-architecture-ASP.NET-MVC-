using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Mango.Services.ShoppingCartAPI.Extensions;

public static class WebApplicationBuilderExtensions
{
    public static WebApplicationBuilder AddAppAuthentication(this WebApplicationBuilder builder)
    {
        //Bind the JWT Configuration to the JWTOptions model with the help of the DI

        var settingsSection = builder.Configuration.GetSection("ApiSettings");

        //The signing key used to sign the JWT in the Auth API.
        //The Coupon API must use the same key to verify the token.
        var secret = settingsSection.GetValue<string>("Secret");
        //Who created and issued the token (e.g., "AuthAPI").
        //Used to prevent tokens from unknown sources.
        var issuer = settingsSection.GetValue<string>("Issuer");
        //Who the token is meant for (e.g., "CouponAPI").
        //Ensures the token is actually intended for this API
        var audience = settingsSection.GetValue<string>("Audience");

        //This line converts your plain text secret into a byte array that will be wrapped into a SymmetricSecurityKey
        var key = Encoding.ASCII.GetBytes(secret);

        builder.Services.AddAuthentication(x =>
        {
            x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(x =>
        {
            x.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = issuer,
                ValidAudience = audience,
                ValidateAudience = true
            };
        });

        return builder;
    }
}
