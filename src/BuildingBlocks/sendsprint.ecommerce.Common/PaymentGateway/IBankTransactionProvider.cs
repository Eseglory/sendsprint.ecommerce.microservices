using sendsprint.ecommerce.Common.PaymentGateway.Model;

namespace sendsprint.ecommerce.Common.PaymentGateway
{
    public interface IBankTransactionProvider : ITransactionProvider
    {
        Task<PaymentProvider> ReceiveFundsDetails(ReceiveFundsRequest request);
        Task<RecievePaymentResponseModel> ReceiveFundsLink(ReceiveFundsRequest request);
        Task<BaseResponse<TransactionStatusResponse>> QueryTransaction(string transactionReference, string collectionReference);
    }
}
