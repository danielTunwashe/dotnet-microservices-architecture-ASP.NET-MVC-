using System.ComponentModel.DataAnnotations;

namespace Mango.services.AuthAPI.Models.Dto;

public class LoginRequestDto
{
    [Required]
    public string UserName { get; set; }
    [Required]
    public string Password { get; set; }
}
