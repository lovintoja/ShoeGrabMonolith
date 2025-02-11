using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoeGrabCommonModels;
using ShoeGrabCommonModels.Contexts;
using ShoeGrabCommonModels.Dto;

namespace ShoeGrabAdminService.Controllers;

[Route("api/admin/products")]
[Authorize(Roles = UserRole.Admin)]
public class ProductManagementController : ControllerBase
{
    private readonly UserContext _context;
    private readonly IMapper _mapper;

    public ProductManagementController(UserContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [HttpPost]
    [Authorize(Roles = UserRole.Admin)]
    public async Task<ActionResult<ProductDto>> AddProduct([FromBody] ProductDto request)
    {
        try
        {
            var product = _mapper.Map<Product>(request);
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return Ok(request);
        }
        catch (Exception)
        {
            return BadRequest("Unable to update product");
        }
    }

    [HttpPut]
    public async Task<ActionResult<ProductDto>> UpdateProduct([FromBody] ProductDto request)
    {
        var product = await _context.Products.FindAsync(request.Id);
        if (product == null)
            return NotFound("Could not found product with this id");

        try
        {
            var mappedRequest = _mapper.Map(request, product);
            await _context.SaveChangesAsync();
            return new OkObjectResult(request);
        }
        catch (Exception)
        {
            return BadRequest("Unable to update product");
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null)
            return NotFound("Not found product with this id");

        try
        {
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return Ok();
        }
        catch (Exception)
        {
            return BadRequest();

        }
    }
}
