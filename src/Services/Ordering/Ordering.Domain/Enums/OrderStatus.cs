namespace Ordering.Domain.Enums;
public enum OrderStatus
{
    Pending = 1,
    PaymentMade = 2,
    OutForDelivery = 3,
    OrderCompleted = 4,
    Cancelled = 5
}
