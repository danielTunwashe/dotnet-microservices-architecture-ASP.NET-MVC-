using Mango.Web.Models;
using Mango.Web.Models.AuthApiModels;
using Mango.Web.Service.IService;
using Mango.Web.Utility;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Mango.Web.Controllers;

public class AuthController : Controller
{
    private readonly IAuthService _authService;
    private readonly ITokenProvider _tokenProvider;
    public AuthController(IAuthService authService, ITokenProvider tokenProvider)
    {
        _authService = authService;
        _tokenProvider = tokenProvider;

    }

    [HttpGet]
    public IActionResult Login()
    {
        LoginRequestDto loginRequestDto = new();
        return View(loginRequestDto);
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginRequestDto loginRequestDto)
    {
        //After getting the RegistrationRequestDto from the View, 
        //we will invoke the _authService LoginAsync and create a ResponseDto
        ResponseDto responseDto = await _authService.LoginAsync(loginRequestDto);

        if (responseDto != null && responseDto.IsSuccess)
        {
            LoginResponseDto loginResponseDto = 
                JsonConvert.DeserializeObject<LoginResponseDto>(JsonConvert.SerializeObject(responseDto.Result));

            //After logging in we need to tell the asp.net our user as actually logged in..
            //So for this, we create an action called SignInUser(and configure it with the claims principal and other necessary things..)
            await SignInUser(loginResponseDto);
            _tokenProvider.SetToken(loginResponseDto.Token);

            //redirect to the index action of the home controller..
            return RedirectToAction("Index", "Home");
        }
        //else
        //{
        //    ModelState.AddModelError("CustomError", responseDto.Message);
        //    return View(loginRequestDto);
        //}
        else
        {
            TempData["error"] = responseDto?.Message;
            return View(loginRequestDto);
        }

    }

    [HttpGet]
    public IActionResult Register()
    {
        //Storing list items in a List and putting them in a viewBag..
        var roleList = new List<SelectListItem>()
        {
            new SelectListItem{Text=SD.RoleAdmin, Value=SD.RoleAdmin},
            new SelectListItem{Text=SD.RoleCustomer, Value=SD.RoleCustomer},
        };

        ViewBag.RoleList = roleList;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegistrationRequestDto registrationRequestDto)
    {
        //After getting the RegistrationRequestDto from the View, 
        //we will invoke the _authService registerAsync and create a ResponseDto
        ResponseDto result = await _authService.RegisterAsync(registrationRequestDto);
        ResponseDto assignRole;

        if(result!=null && result.IsSuccess)
        {
            if (string.IsNullOrEmpty(registrationRequestDto.Role))
            {
                registrationRequestDto.Role = SD.RoleCustomer;
            }
            assignRole = await _authService.AssignRoleAsync(registrationRequestDto);
            if (assignRole != null && assignRole.IsSuccess)
            {
                //display a tempdata in the notification that registration is successful
                //redirect user to login
                TempData["success"] = "Registration Successful";
                return RedirectToAction(nameof(Login));   
            }
        }
        else
        {
            TempData["error"] = result?.Message;
        }

            //Storing list items in a List and putting them in a viewBag..
            var roleList = new List<SelectListItem>()
            {
                new SelectListItem{Text=SD.RoleAdmin, Value=SD.RoleAdmin},
                new SelectListItem{Text=SD.RoleCustomer, Value=SD.RoleCustomer},
            };

        ViewBag.RoleList = roleList;
        return View(registrationRequestDto);
    }


    [HttpGet]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync();
        _tokenProvider.ClearToken();
        return RedirectToAction("Index","Home");
    }

    //sign in the user usig .NET identity..
    private async Task SignInUser(LoginResponseDto model)
    {
        //this will read the JWT token that a logged in user has
        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(model.Token);

        //and from the token we will extract all the claims that we added..
        var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);

        identity.AddClaim(new Claim(JwtRegisteredClaimNames.Email, 
            jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Email).Value));
        identity.AddClaim(new Claim(JwtRegisteredClaimNames.Sub,
            jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Sub).Value));
        identity.AddClaim(new Claim(JwtRegisteredClaimNames.Name,
            jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Name).Value));


        //must add this claim also..
        identity.AddClaim(new Claim(ClaimTypes.Name,
            jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Email).Value));

        //Extracting role claims in the jwt 
        identity.AddClaim(new Claim(ClaimTypes.Role,
            jwt.Claims.FirstOrDefault(u => u.Type == "role").Value));


        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
    }

}
