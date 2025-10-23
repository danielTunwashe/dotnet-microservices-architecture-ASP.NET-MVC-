namespace Mango.Web.Models.PayStackModel
{
    public class VerifyPaymentResponseDto
    {
        public bool Status { get; set; }
        public string GatewayResponse { get; set; }
        public string Reference { get; set; }
        public string Message { get; set; }
    }
}
