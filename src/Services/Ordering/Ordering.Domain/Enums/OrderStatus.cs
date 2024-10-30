namespace Ordering.Domain.Enums;
public enum OrderStatus
{
    Pending = 1,
    Completed = 2,
    PaymentMade = 3,
    ShippedForDelivery = 4,
    Cancelled = 5,
    PaymentFailed = 6
}
