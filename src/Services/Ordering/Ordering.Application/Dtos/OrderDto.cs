using Ordering.Domain.Enums;

namespace Ordering.Application.Dtos;

public record OrderDto(
    Guid Id,
    Guid CustomerId,
    string OrderName,
    AddressDto ShippingAddress,
    AddressDto BillingAddress,
    string Payment,
    //OrderStatus Status,
    List<OrderItemDto> OrderItems);
