using Ordering.Domain.Enums;
using sendsprint.ecommerce.Common.PaymentGateway.Model;

namespace Ordering.Application.Dtos;

public record VerifyPaymentDto(
    string CollectionReference,
   GatewayType PaymentGateway);

public record PaymentDetailsDto(
    string CustomerId,
    string CollectionReference,
    decimal Amount,
    DateTime PaymentDate,
   GatewayType PaymentGateway);

