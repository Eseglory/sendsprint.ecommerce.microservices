using Newtonsoft.Json;
using System.Transactions;

namespace sendsprint.ecommerce.Common.PaymentGateway.Model
{
    public class TransactionStatusResponse
    {
        [JsonProperty("transactionStatus")]
        public TransactionStatus TransactionStatus { get; set; }

        [JsonProperty("status")]
        public string? Status { get; set; }

        [JsonProperty("amount")]
        public double? Amount { get; set; }

        [JsonProperty("settled_amount")]
        public double? SettledAmount { get; set; }

        [JsonProperty("transactionInfo")]
        public string? TransactionInfo { get; set; }

        [JsonProperty("narration")]
        public string? Narration { get; set; }

        [JsonProperty("mReference")]
        public string? Reference { get; set; }

        [JsonProperty("responseCode")]
        public string? ResponseCode { get; set; }

        [JsonProperty("statusDescription")]
        public string? StatusDescription { get; set; }

    }
}
