
using Newtonsoft.Json;

namespace sendsprint.ecommerce.Common.PaymentGateway.Model
{
    public class RecievePaymentResponseModel
    {
        [JsonProperty("status")]
        public bool Status { get; set; }

        [JsonProperty("message")]
        public required string Message { get; set; }

        [JsonProperty("link")]
        public required string Link { get; set; }

        [JsonProperty("platformType")]
        public GatewayType PlatformType { get; set; }
    }
}
