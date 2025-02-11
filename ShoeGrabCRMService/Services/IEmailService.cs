using ShoeGrabCommonModels;

namespace ShoeGrabCRMService.Services;

public interface IEmailService
{
    Task<bool> SendOrderConfirmationEmailAsync(string recipientEmail);
}