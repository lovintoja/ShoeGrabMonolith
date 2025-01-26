namespace ShoeGrabMonolith.Services;

public interface IUserManagementService
{
    bool UpdateUserPassword(string username, string password); 
}
