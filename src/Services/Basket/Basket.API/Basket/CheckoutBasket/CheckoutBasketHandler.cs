
using BuildingBlocks.Messaging.Events;
using MassTransit;
using sendsprint.ecommerce.Common;
using sendsprint.ecommerce.Common.PaymentGateway;
using sendsprint.ecommerce.Common.PaymentGateway.Model;

namespace Basket.API.Basket.CheckoutBasket;

public record CheckoutBasketCommand(BasketCheckoutDto BasketCheckoutDto)
    : ICommand<List<PaymentProvider>>;

public class CheckoutBasketCommandValidator
    : AbstractValidator<CheckoutBasketCommand>
{
    public CheckoutBasketCommandValidator()
    {
        RuleFor(x => x.BasketCheckoutDto).NotNull().WithMessage("BasketCheckoutDto can't be null");
        RuleFor(x => x.BasketCheckoutDto.UserName).NotEmpty().WithMessage("UserName is required");
    }
}

public class CheckoutBasketCommandHandler
    (IBasketRepository repository, IPublishEndpoint publishEndpoint,
    IGatewayManager gatewayManager, IHttpContextAccessor accessor)
    : ICommandHandler<CheckoutBasketCommand, List<PaymentProvider>>
{
    public async Task<List<PaymentProvider>> Handle(CheckoutBasketCommand command, CancellationToken cancellationToken)
    {
        // get existing basket with total price
        // Set totalprice on basketcheckout event message
        // send basket checkout event to rabbitmq using masstransit
        // delete the basket

        var basket = await repository.GetBasket(command.BasketCheckoutDto.UserName, cancellationToken);
        if (basket == null)
        {
            return new List<PaymentProvider>();
        }
        var transactionReference = $"SSP-{CodeGenerator.RandomAlphabetString(21)}";
        var eventMessage = command.BasketCheckoutDto.Adapt<BasketCheckoutEvent>();
        eventMessage.TotalPrice = basket.TotalPrice;
        eventMessage.TransactionReference = transactionReference;

        await publishEndpoint.Publish(eventMessage, cancellationToken);
        //TODO: move the delete to payment gateway callback
        //await repository.DeleteBasket(command.BasketCheckoutDto.UserName, cancellationToken);

        #region get payment gateway services
        var gateways = gatewayManager.GetCurrencyReceiveProviderList(eventMessage.TotalPrice);
        var paymentProviders = new List<PaymentProvider>();

        var paymentRequest = new ReceiveFundsRequest
        {
            //TODO get this details from identity service
            Email = "engreseglory@gmail.com",
            PhoneNumber = "08034441916",
            Name = "Glory Efionayi",
            Reference = transactionReference,
            Amount = eventMessage.TotalPrice,
            UserRequestIPAddress = 
            accessor.HttpContext?.Connection?.RemoteIpAddress?.ToString() ?? string.Empty
        };
        if (gateways != null)
        {
            foreach (var gw in gateways)
            {
                var pd = await gw.ReceiveFundsDetails(paymentRequest);
                paymentProviders.Add(pd);
            }
        }
        #endregion
        paymentProviders[0].IsDefault = true;
        return paymentProviders;
    }
}
