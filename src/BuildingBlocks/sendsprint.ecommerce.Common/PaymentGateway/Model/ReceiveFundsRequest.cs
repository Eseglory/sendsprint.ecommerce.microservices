
namespace sendsprint.ecommerce.Common.PaymentGateway.Model
{
    public class ReceiveFundsRequest
    {
        public decimal Amount { get; set; }
        public required string Reference { get; set; }
        public required string Email { get; set; }
        public required string PhoneNumber { get; set; }
        public required string Name { get; set; }
        public required string UserRequestIPAddress { get; set; }
        public string? CallbackUrl { get; set; }
        public string? ReturnUrl { get; set; }
    }
}
