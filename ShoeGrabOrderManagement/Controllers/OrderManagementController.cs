using ShoeGrabCommonModels.Contexts;
using ShoeGrabOrderManagement.Dto;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoeGrabCommonModels;
using System.Security.Claims;
using ShoeGrabCRMService.Services;

namespace ShoeGrabOrderManagement.Controllers;

[Route("api/order")]
public class OrderManagementController : ControllerBase
{
    private readonly UserContext _context;
    private readonly IMapper _mapper;
    private readonly IEmailService _emailService;

    public OrderManagementController(UserContext context, IMapper mapper, IEmailService emailService)
    {
        _context = context;
        _mapper = mapper;
        _emailService = emailService;
    }

    [HttpPost]
    [Authorize]
    [Route("create")]
    public async Task<ActionResult<OrderDto>> CreateOrder([FromBody] CreateOrderDto request)
    {
        var order = _mapper.Map<Order>(request);
        var userId = User.FindFirst(ClaimTypes.Authentication).Value;
        var user = _context.Users.SingleOrDefaultAsync(u => u.Id == int.Parse(userId)).Result;

        foreach (var item in request.Items)
        {
            var product = await _context.Products.FindAsync(item.ProductId);
            if (product == null)
                return BadRequest($"Product {item.ProductId} not found.");

            order.Items.Add(new OrderItem
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                UnitPrice = product.Price
            });
        }

        order.TotalPrice = order.Items.Sum(i => i.Quantity * i.UnitPrice);
        order.UserId = int.Parse(userId);

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        await _emailService.SendOrderConfirmationEmailAsync(user.Email, user.Username, order);
        var orderDto = _mapper.Map<OrderDto>(order);
        return Ok(orderDto);
    }

    [HttpGet]
    [Authorize]
    [Route("get/{id}")]
    public async Task<ActionResult<OrderDetailsDto>> GetOrder(int id)
    {
        var order = await _context.Orders
            .Include(o => o.Items)
            .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order == null)
            return NotFound();

        return Ok(_mapper.Map<OrderDetailsDto>(order));
    }

    [HttpGet]
    [Authorize]
    [Route("getuser")]
    public async Task<ActionResult<PagedResponse<OrderSummaryDto>>> GetUserOrders(
        [FromQuery] OrderFilter filter,
        [FromQuery] PaginationParams pagination)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.Authentication).Value);

        var query = _context.Orders
            .Where(o => o.UserId == userId)
            .AsQueryable();

        if (filter.StartDate.HasValue)
            query = query.Where(o => o.OrderDate >= filter.StartDate.Value);

        if (filter.EndDate.HasValue)
            query = query.Where(o => o.OrderDate <= filter.EndDate.Value);

        if (!string.IsNullOrEmpty(filter.Status))
            query = query.Where(o => o.Status == filter.Status);

        var totalRecords = await query.CountAsync();
        var orders = await query
            .OrderByDescending(o => o.OrderDate)
            .Skip((pagination.PageNumber - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .ToListAsync();

        var orderSummaries = _mapper.Map<List<OrderSummaryDto>>(orders);

        return Ok(new PagedResponse<OrderSummaryDto>
        {
            Data = orderSummaries,
            PageNumber = pagination.PageNumber,
            PageSize = pagination.PageSize,
            TotalRecords = totalRecords
        });
    }

    [HttpPut]
    [Authorize]
    [Route("update/{id}")]
    public async Task<IActionResult> UpdateOrder(
        int id,
        [FromBody] UpdateOrderDto request)
    {
        var order = await _context.Orders.FindAsync(id);
        if (order == null)
            return NotFound();

        var userId = int.Parse(User.FindFirst(ClaimTypes.Authentication).Value);
        if (order.UserId != userId && !User.IsInRole(UserRole.Admin))
            return Forbid();

        _mapper.Map(request, order);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete]
    [Authorize]
    [Route("cancel/{id}")]
    public async Task<IActionResult> CancelOrder(int id)
    {
        var order = await _context.Orders.FindAsync(id);
        if (order == null)
            return NotFound();

        var userId = int.Parse(User.FindFirst(ClaimTypes.Authentication).Value);
        if (order.UserId != userId && !User.IsInRole(UserRole.Admin))
            return Forbid();

        order.Status = OrderStatus.Cancelled;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpGet]
    [Authorize]
    [Route("items/{id}")]
    public async Task<ActionResult<List<OrderItemExtendedDto>>> GetOrderItems(int id)
    {
        var items = await _context.OrderItems
            .Include(i => i.Product)
            .Where(i => i.OrderId == id)
            .ToListAsync();

        return Ok(_mapper.Map<List<OrderItemExtendedDto>>(items));
    }

    [HttpGet]
    [Authorize]
    [Route("status/{id}")]
    public async Task<ActionResult<OrderStatusDto>> GetOrderStatus(int id)
    {
        var status = await _context.Orders
            .Where(o => o.Id == id)
            .Select(o => new OrderStatusDto
            {
                OrderId = o.Id,
                Status = o.Status
            })
            .FirstOrDefaultAsync();

        if (status == null)
            return NotFound();

        return Ok(status);
    }

    [HttpGet]
    [Authorize(Roles = UserRole.Admin)]
    [Route("search")]
    public async Task<ActionResult<AdminOrderDto>> SearchOrders(
        [FromQuery] AdminOrderSearchQuery query)
    {
        var dbQuery = _context.Orders
            .Include(o => o.User)
            .AsQueryable();

        if (query.UserId.HasValue)
            dbQuery = dbQuery.Where(o => o.UserId == query.UserId.Value);

        if (query.StartDate.HasValue)
            dbQuery = dbQuery.Where(o => o.OrderDate >= query.StartDate.Value);

        if (query.EndDate.HasValue)
            dbQuery = dbQuery.Where(o => o.OrderDate <= query.EndDate.Value);

        if (!string.IsNullOrEmpty(query.Status))
            dbQuery = dbQuery.Where(o => o.Status == query.Status);

        var totalRecords = await dbQuery.CountAsync();
        var orders = await dbQuery
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();

        var adminOrders = _mapper.Map<List<AdminOrderDto>>(orders);

        return Ok(adminOrders);
    }
}