using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace ReminderService;

public sealed class EmailService(IConfiguration configuration)
{
    public async Task SendEmailAsync(
        MimeMessage message,
        CancellationToken cancellation)
    {
        using var client = new SmtpClient();

        await client.ConnectAsync("localhost", 25, SecureSocketOptions.None, cancellation);

        await client.SendAsync(message, cancellation);
        await client.DisconnectAsync(true, cancellation);
    }
}