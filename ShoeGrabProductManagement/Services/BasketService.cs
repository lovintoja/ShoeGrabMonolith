using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ShoeGrabCommonModels;
using ShoeGrabCommonModels.Contexts;

namespace ShoeGrabProductManagement.Services;

public class BasketService : IBasketService
{
    private readonly UserContext _context;

    public BasketService(UserContext context, IMapper mapper)
    {
        _context = context;
    }

    public async Task<Basket?> CreateBasket(int userId)
    {
        var basket = new Basket { UserId = userId };

        try
        {
            await _context.Baskets.AddAsync(basket);
            await _context.SaveChangesAsync();
        }
        catch (Exception)
        {
            return null;
        }
        return basket;
    }

    public async Task<Basket?> GetBasket(int userId)
    {
        return await _context.Baskets.FirstOrDefaultAsync(b => b.UserId == userId);
    }

    public async Task<bool> RemoveBasket(int userId)
    {
        try
        {
            var basket = await _context.Baskets.FindAsync(userId);

            if (basket == null)
            {
                return false;
            }

            _context.Baskets.Remove(basket);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception)
        { 
            return false; 
        }
    }

    public async Task<bool> UpdateBasket(int userId, Basket updatedBasket)
    {
        try
        {
            if (updatedBasket == null)
            {
                throw new ArgumentNullException(nameof(updatedBasket), "Updated basket cannot be null.");
            }

            var existingBasket = await _context.Baskets.FirstOrDefaultAsync(b => b.UserId == userId);

            if (existingBasket == null)
            {
                existingBasket = await CreateBasket(userId);
                if (existingBasket == null)
                {
                    return false;
                }
            }
            _context.Entry(existingBasket).Collection(b => b.Items).IsModified = true;
            existingBasket.Items = updatedBasket.Items;

            await _context.SaveChangesAsync();

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating basket for user {userId}: {ex.Message}");
            return false;
        }
    }
}
