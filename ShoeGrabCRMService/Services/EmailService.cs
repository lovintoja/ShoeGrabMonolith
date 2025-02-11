using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using ShoeGrabCommonModels;

namespace ShoeGrabCRMService.Services;

public class EmailService : IEmailService
{
    private readonly SmtpClient _smtpClient;
    private readonly string _senderEmail;

    public EmailService(IConfiguration config)
    {
        //here initialization would take place
        _smtpClient = new SmtpClient();
        _senderEmail = "stub";
    }

    public async Task<bool> SendOrderConfirmationEmailAsync(string recipientEmail)
    {
        await Task.Delay(50);

        return true;
    }
}