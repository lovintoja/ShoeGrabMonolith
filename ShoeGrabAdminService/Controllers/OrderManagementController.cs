using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoeGrabCommonModels;
using AutoMapper;
using ShoeGrabCommonModels.Contexts;
using ShoeGrabAdminService.Dto;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using ShoeGrabCommonModels.Dto;

namespace ShoeGrabAdminService.Controllers;

[Route("api/admin/order")]
[Authorize(Roles = UserRole.Admin)]
public class OrderManagementController : ControllerBase
{
    private readonly UserContext _context;
    private readonly IMapper _mapper;

    public OrderManagementController(IMapper mapper, UserContext context)
    {
        _mapper = mapper;
        _context = context;
    }

    [HttpDelete]
    [Route("delete")]
    public async Task<IActionResult> DeleteOrder(int orderId)
    {
        var order = await _context.Orders.FindAsync(orderId);
        if (order == null)
        {
            return NotFound("Order not found");
        }
        try
        {
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
            return Ok();
        }
        catch (Exception)
        {
            return BadRequest("Something went wrong during order delete operation.");
        }
    }

    [HttpPatch]
    [Route("{orderId}/status")]
    public async Task<IActionResult> ChangeOrderStatus(int orderId, [FromBody] OrderStatusUpdateDto request)
    {
        var order = await _context.Orders.FindAsync(orderId);
        if (order == null)
            return NotFound("Order not found");

        if (!Enum.TryParse(typeof(OrderStatus), request.NewStatus, out var status))
            return BadRequest("Wrong order status format");

        try
        {
            var mappedStatus = (OrderStatus)status;
            order.Status = mappedStatus;
            await _context.SaveChangesAsync();
            return Ok();
        }
        catch (Exception)
        {
            return BadRequest("Something went wrong during order status update.");
        }
    }

    [HttpGet]
    [Route("search")]
    public async Task<ActionResult<IEnumerable<OrderDto>>> SearchOrders(
        [FromQuery] AdminOrderSearchQuery request)
    {
        var query = _context.Orders
            .Where(o => o.UserId == request.UserId)
            .AsQueryable();

        if (request.StartDate.HasValue)
            query = query.Where(o => o.OrderDate >= request.StartDate.Value);

        if (request.EndDate.HasValue)
            query = query.Where(o => o.OrderDate <= request.EndDate.Value);

        if (!string.IsNullOrEmpty(request.Status) && Enum.TryParse(typeof(OrderStatus), request.Status, out var status))
        {
            var mappedStatus = (OrderStatus)status;
            query = query.Where(o => o.Status == mappedStatus);
        }

        var orders = await query
            .OrderByDescending(o => o.OrderDate)
            .Include(o => o.Items)
            .ToListAsync();

        var orderSummaries = _mapper.Map<List<OrderDto>>(orders);

        return Ok(orderSummaries);
    }
}