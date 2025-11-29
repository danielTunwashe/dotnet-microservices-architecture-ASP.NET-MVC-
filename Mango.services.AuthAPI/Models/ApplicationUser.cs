using Microsoft.AspNetCore.Identity;

namespace Mango.services.AuthAPI.Models;

public class ApplicationUser : IdentityUser
{
    public string Name { get; set; } = default!;
}
