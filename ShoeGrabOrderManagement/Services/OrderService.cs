using Microsoft.EntityFrameworkCore;
using ShoeGrabCommonModels;
using ShoeGrabCommonModels.Contexts;
using ShoeGrabCRMService.Services;


namespace ShoeGrabOrderManagement.Services;

public class OrderService : IOrderService
{
    private readonly IEmailService _emailService;
    private readonly UserContext _context;

    public OrderService(UserContext context, IEmailService emailService)
    {
        _context = context;
        _emailService = emailService;
    }

    public async Task<bool> CreateOrder(int userId, Order order)
    {
        foreach (var item in order.Items)
        {
            var product = await _context.Products.FindAsync(item.ProductId);

            item.UnitPrice = product.Price;
        }

        order.TotalPrice = order.Items.Sum(i => i.Quantity * i.UnitPrice);
        order.UserId = userId;
        try
        {
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            return false;
        }

        try
        {
            var user = await _context.Users.FindAsync(userId);
            await _emailService.SendOrderConfirmationEmailAsync(user.Email);
        }
        catch (Exception)
        {
            //If email does not get sent, order still went through.
            return true;
        }
        return true;
    }

    public async Task<IEnumerable<Order>> GetOrders(int userId)
    {
        return await _context.Orders
            .Where(o => o.UserId == userId)
            .Include(o => o.Items)
            .ToListAsync();
    }
}
