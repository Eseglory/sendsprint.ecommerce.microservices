using sendsprint.ecommerce.Common.PaymentGateway.Model;

namespace sendsprint.ecommerce.Common.PaymentGateway
{
    public interface IGatewayManager
    {
        IBankTransactionProvider GetCurrencyReceiveProvider(double amount, IEnumerable<int> exclusions = null);
        List<IBankTransactionProvider> GetCurrencyReceiveProviderList(decimal amount, IEnumerable<int> exclusions = null);
        IBankTransactionProvider GetCurrencySendProvider(double amount, IEnumerable<int> exclusions = null);
        IEnumerable<ITransactionProvider> GetAllGateways();
        IEnumerable<ITransactionProvider> GetGatewaysByType(GatewayType gatewayType, bool getAll = false);
        ITransactionProvider GetGatewayById(int gatewayId);
        bool AddTransactionProvider(ITransactionProvider gateway);
        bool RemoveTransactionProvider(int gatewayId);
        void ClearTransactionProviders();
    }
}
