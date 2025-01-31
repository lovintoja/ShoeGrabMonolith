using ShoeGrabCommonModels;
using System.ComponentModel.DataAnnotations;

namespace ShoeGrabCommonModels;
public class UserProfile
{
    public int Id { get; set; }
    [Required]
    public string Address { get; set; } = string.Empty;
    [Required]
    [Phone]
    public string PhoneNumber { get; set; } = string.Empty;
    [Required]
    [DataType(DataType.Date)]
    public DateTime DateOfBirth { get; set; } = DateTime.UtcNow;
    public string Bio { get; set; } = string.Empty;
    public int UserId { get; set; }
    public User User { get; set; }
}