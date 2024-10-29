using sendsprint.ecommerce.Common.PaymentGateway.Model;

namespace sendsprint.ecommerce.Common.PaymentGateway
{
    public static class Extensions
    {
        public static Type ClassType(this GatewayUseCase gatewayUseCase)
        {
            return gatewayUseCase switch
            {
                GatewayUseCase.CurrencyReceive => typeof(IBankTransactionProvider),
                GatewayUseCase.CurrencySend => typeof(IBankTransactionProvider),
                _ => throw new ArgumentOutOfRangeException(nameof(gatewayUseCase), gatewayUseCase, "Please implement me")
            };
        }

        public static int PriorityField(this GatewayUseCase gatewayUseCase, ITransactionProvider p)
        {
            return gatewayUseCase switch
            {
                GatewayUseCase.CurrencyReceive => p.CurrencyReceivePriority,
                GatewayUseCase.CurrencySend => p.CurrencySendPriority,
                _ => throw new ArgumentOutOfRangeException(nameof(gatewayUseCase), gatewayUseCase, "Please implement me")
            };
        }
    }
}
