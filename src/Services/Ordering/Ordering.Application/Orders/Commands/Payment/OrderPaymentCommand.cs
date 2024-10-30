using BuildingBlocks.CQRS;
using FluentValidation;
using Ordering.Application.Dtos;

namespace Ordering.Application.Orders.Commands.CreateOrder;

public record OrderPaymentCommand(VerifyPaymentDto request)
    : ICommand<OrderPaymentResult>;
public record OrderPaymentResult(string result);

public class OrderPaymentCommandValidator : AbstractValidator<OrderPaymentCommand>
{
    public OrderPaymentCommandValidator()
    {
        RuleFor(x => x.request.CollectionReference).NotEmpty().WithMessage("Collection Reference is required");
    }
}