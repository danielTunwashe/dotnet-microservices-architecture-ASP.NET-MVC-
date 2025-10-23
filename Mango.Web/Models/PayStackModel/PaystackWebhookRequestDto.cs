using System.ComponentModel.DataAnnotations;

namespace Mango.Web.Models.PayStackModel;

public class ProcessWebhookRequestDto
{
    [Required]
    public string RequestBody { get; set; }
    [Required]
    public string Signature { get; set; }
}
