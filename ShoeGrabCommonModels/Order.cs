using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ShoeGrabCommonModels;
public class Order
{
    public int Id { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.New;
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;
    public double TotalPrice { get; set; }
    public Address ShippingAddress { get; set; } = new();
    public PaymentInfo PaymentInfo { get; set; } = new();
    public string ContactPhone { get; set; } = string.Empty;
    public string? Note { get; set; }
    public List<OrderItem> Items { get; set; } = new();

    public int UserId { get; set; }
    public User User { get; set; }
}
