using ShoeGrabCommonModels;

namespace ShoeGrabCRMService.Services;

public interface IEmailService
{
    Task SendOrderConfirmationEmailAsync(string recipientEmail, string recipientName, Order order);
}