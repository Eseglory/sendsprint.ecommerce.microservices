
namespace sendsprint.ecommerce.Common.PaymentGateway.Model
{
    public class PaymentProvider
    {
        public required string Name { get; set; }
        public required decimal Amount { get; set; }
        public required string PaymentLink { get; set; }
        public bool IsDefault { get; set; }
    }
}
