
namespace sendsprint.ecommerce.Common.PaymentGateway.Model
{
    public enum GatewayType : int
    {
        Paystack = 1,
        FlutterWave = 2,
    }

    public enum GatewayStatus : int
    {
        Online = 1,
        Offline,
        Error,
        Disabled,
        Seeding
    }

    public enum GatewayResponseEnum
    {
        Success = 1,
        Failed,
        Pending
    }

    public enum TransactionStatus
    {
        SUCCESS = 1,
        CANCELLED,
        FAILED,
        PENDING,
        UNKNOWN,
        NOT_FOUND
    }

    public enum GatewayUseCase
    {
        CurrencyReceive,
        CurrencySend
    }

}
