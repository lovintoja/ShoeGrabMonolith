using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoeGrabMonolith.Database.Contexts;
using ShoeGrabMonolith.Models;
using ShoeGrabMonolith.Services;
using System.Security.Claims;

namespace ShoeGrabMonolith.Controllers;

public class AuthenticationController : ControllerBase
{
    private readonly ITokenService _tokenService;
    private readonly UserContext _userContext;

    public AuthenticationController(ITokenService tokenService, UserContext context)
    {
        _tokenService = tokenService;
        _userContext = context;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestModel model)
    {
        var user = await Task.FromResult(
            _userContext.Users
                .Where(u => u.Username == model.Username)
                .FirstOrDefault());

        if (user == null || user.Password != model.Password)
        {
            return Unauthorized();
        }

        var token = _tokenService.GenerateToken(user.Id);

        return Ok(new { token });
    }

    [Authorize]
    [HttpGet]
    public IActionResult GetProtectedData()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Ok(new { message = "Protected data accessed by user " + userId });
    }
}
