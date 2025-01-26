using Microsoft.AspNetCore.Mvc;
using ShoeGrabMonolith.Services;

namespace ShoeGrabMonolith.Controllers;
[ApiController]
[Route("[controller]")]
public class UserManagementController : ControllerBase
{
    private readonly ILogger<UserManagementController> _logger;
    private readonly IUserManagementService _userManagementService;

    public UserManagementController(ILogger<UserManagementController> logger, IUserManagementService userManagementService)
    {
        _logger = logger;
        _userManagementService = userManagementService;
    }

    [HttpPost(Name = "api/user/changepassword")]
    public bool ChangeUserPassword(string username, string password)
    {
        return _userManagementService.UpdateUserPassword(username, password);
    }
}
