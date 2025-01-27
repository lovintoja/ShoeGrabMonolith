using ShoeGrabMonolith.Database.Contexts;

namespace ShoeGrabMonolith.Services;

public class UserManagementService : IUserManagementService
{
    private readonly UserContext _userContext;

    public UserManagementService(UserContext userContext)
    {
        _userContext = userContext;
    }

    public bool UpdateUserPassword(string username, string password)
    {
        var user = _userContext.Users.Where(u => u.Username == username).FirstOrDefault(u => u.Password == password);
        
        return user != null;
    }
}
