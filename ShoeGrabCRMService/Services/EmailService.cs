using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using ShoeGrabCommonModels;

namespace ShoeGrabCRMService.Services;

public class SmtpEmailService : IEmailService
{
    private readonly SmtpClient _smtpClient;
    private readonly string _senderEmail;

    public SmtpEmailService(IConfiguration config)
    {
        _senderEmail = config["Smtp:SenderEmail"];
        _smtpClient = new SmtpClient(config["Smtp:Host"])
        {
            Port = int.Parse(config["Smtp:Port"]),
            Credentials = new NetworkCredential(
                config["Smtp:Username"],
                config["Smtp:Password"]
            ),
            EnableSsl = true
        };
    }

    public async Task SendOrderConfirmationEmailAsync(
        string recipientEmail,
        string recipientName,
        Order order)
    {
        var mailMessage = new MailMessage
        {
            From = new MailAddress(_senderEmail),
            Subject = "Your Order Confirmation",
            Body = BuildOrderConfirmationEmailBody(recipientName, order),
            IsBodyHtml = true
        };
        mailMessage.To.Add(recipientEmail);

        await _smtpClient.SendMailAsync(mailMessage);
    }

    private string BuildOrderConfirmationEmailBody(string recipientName, Order order)
    {
        return $@"
            <h1>Thank you for your order, {recipientName}!</h1>
            <p>Order ID: {order.Id}</p>
            <p>Total: ${order.TotalPrice}</p>
            <p>Status: {order.Status}</p>
            <h3>Items:</h3>
            <ul>
                {string.Join("", order.Items.Select(i =>
                    $"<li>{i.Quantity}x {i.Product.Name} (${i.UnitPrice} each)</li>"))}
            </ul>
        ";
    }
}