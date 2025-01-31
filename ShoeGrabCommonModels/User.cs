using System.ComponentModel.DataAnnotations;

namespace ShoeGrabCommonModels;

public class User
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Username { get; set; }

    [Required]
    [EmailAddress]
    [StringLength(255)]
    public string Email { get; set; }

    [Required]
    public string PasswordHash { get; set; }

    [Required]
    public UserProfile Profile { get; set; } = new UserProfile();
    [Required]
    public string Role { get; set; } = UserRole.User;
}
