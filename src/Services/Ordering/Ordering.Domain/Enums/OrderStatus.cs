namespace Ordering.Domain.Enums;
public enum OrderStatus
{
    Draft = 1,
    Pending = 2,
    PaymentMade = 3,
    OutForDelivery = 4,
    OrderCompleted = 5,
    Cancelled = 6
}
