
namespace sendsprint.ecommerce.Common.PaymentGateway.Model
{
    public class GatewayBaseResponseModel
    {
        public GatewayResponseEnum Status { get; set; }
        public required string Message { get; set; }
        public required dynamic Data { get; set; }
    }
}
