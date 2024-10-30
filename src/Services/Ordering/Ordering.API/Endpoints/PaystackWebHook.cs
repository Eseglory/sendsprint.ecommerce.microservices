

using Microsoft.AspNetCore.Mvc;
using Ordering.Application.Orders.Commands.CreateOrder;

namespace Ordering.API.Endpoints;
public record PayStackWebHookResponse(string result);

public class PayStackWebHook : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/orders/paystack-webhook", async ([FromQuery]string trxref, ISender sender) =>
        {
            var result = await sender.Send(new OrderPaymentCommand(new VerifyPaymentDto(trxref, sendsprint.ecommerce.Common.PaymentGateway.Model.GatewayType.Paystack)));

            return Results.Created($"/orders/paystack-webhook/{result}", result);
        })
        .WithName("PayStackWebHook")
        .Produces<PayStackWebHookResponse>(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithSummary("PayStack WebHook")
        .WithDescription("PayStack WebHook");
    }
}