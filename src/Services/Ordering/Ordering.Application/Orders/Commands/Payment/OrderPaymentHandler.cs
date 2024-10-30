using Newtonsoft.Json;
using Ordering.Domain.Enums;
using sendsprint.ecommerce.Common.PaymentGateway;
using sendsprint.ecommerce.Common.PaymentGateway.Model;

namespace Ordering.Application.Orders.Commands.CreateOrder;
public class OrderPaymentHandler(IApplicationDbContext dbContext,
    IGatewayManager gatewayManager)
    : ICommandHandler<OrderPaymentCommand, OrderPaymentResult>
{
    public async Task<OrderPaymentResult> Handle(OrderPaymentCommand command, CancellationToken cancellationToken)
    {
        return await VerifyPayment(command.request, cancellationToken);
    }

    public async Task<OrderPaymentResult> VerifyPayment(VerifyPaymentDto request, CancellationToken cancellationToken)
    {
        var orderTransaction = dbContext.Orders.FirstOrDefault(c => c.TransactionReference == request.CollectionReference && c.Status == OrderStatus.Pending);

        if (orderTransaction == null)
        {
            return new OrderPaymentResult("Awaiting payment confirmation");
        }

        IEnumerable<int> exclusions = new List<int> { (int)GatewayType.FlutterWave };
        var gateways = gatewayManager.GetCurrencyReceiveProvider(50000, exclusions);

        var verifyPaymentDetails = await gateways.QueryTransaction(request.CollectionReference, request.CollectionReference);

        if (verifyPaymentDetails?.Data?.Status == "success" || verifyPaymentDetails?.Data?.Status == "successful"
            && verifyPaymentDetails.Data.Amount == orderTransaction.TotalPrice)
        {
            var paymentDetails = new PaymentDetailsDto(orderTransaction.CustomerId.ToString(),
            orderTransaction.TransactionReference, orderTransaction.TotalPrice,

            DateTime.Now, request.PaymentGateway);
            orderTransaction.Status = OrderStatus.PaymentMade;
            orderTransaction.Payment = JsonConvert.SerializeObject(paymentDetails);
            return new OrderPaymentResult("Payment received");
        }
        else if (verifyPaymentDetails?.Data?.Status == "failed")
        {
            orderTransaction.Status = OrderStatus.PaymentFailed;
        }

        dbContext.Orders.Update(orderTransaction);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new OrderPaymentResult("Awaiting payment confirmation");
    }
}
