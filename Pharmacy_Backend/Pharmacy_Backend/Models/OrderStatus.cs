namespace Pharmacy.API.Models;

public enum OrderStatus
{
    Placed,
    Confirmed,
    Processing,
    Shipped,
    Delivered,
    Cancelled
}