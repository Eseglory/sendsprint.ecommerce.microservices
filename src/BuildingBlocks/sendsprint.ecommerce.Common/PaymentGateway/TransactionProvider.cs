using sendsprint.ecommerce.Common.PaymentGateway.Model;

namespace sendsprint.ecommerce.Common.PaymentGateway
{
    public abstract class TransactionProvider : ITransactionProvider
    {
        public int GatewayId { get; set; }
        public int CurrencySendPriority { get; set; }
        public int BillSendPriority { get; set; }
        public int CurrencyReceivePriority { get; set; }
        public int BulkAirtimeSendPriority { get; set; }
        public int AirtimeSendPriority { get; set; }
        public int AirtimeReceivePriority { get; set; }
        public int SmsSendPriority { get; set; }
        public int HlrPriority { get; set; }
        public int MaxAmount { get; set; }
        public int AlertBalance { get; set; }
        public string? CredentialKey { get; set; }
        public string? Url { get; set; }
        public GatewayStatus Status { get; set; }
        public abstract GatewayType GatewayType { get; }

        protected GatewayBalanceResponse QueryGatwewayAvailablity(GatewayBalanceResponse balance)
        {
            var gatewayBalance = (balance != null) ? balance.Balance : 0;
            var gatewayData = (balance != null) ? balance.Data : "";

            if (balance != null && balance.Balance > AlertBalance)
            {
                return new GatewayBalanceResponse { Status = GatewayResponseEnum.Success, Message = "Gateway is available", Balance = gatewayBalance, Data = balance.Data };
            }
            return new GatewayBalanceResponse
            { Status = GatewayResponseEnum.Failed, Message = "Gateway is unavailable", Balance = gatewayBalance, Data = gatewayData };
        }
    }
}
