namespace Mango.Web.Models.PayStackModel;

public class InitializePaymentResponseDto
{
    public string AuthorizationUrl { get; set; }
    public string AccessCode { get; set; }
    public string Reference { get; set; }
}
