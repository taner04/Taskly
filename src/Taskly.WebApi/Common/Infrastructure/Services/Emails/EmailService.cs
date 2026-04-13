using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using Taskly.WebApi.Common.Composition.Options;

namespace Taskly.WebApi.Common.Infrastructure.Services.Emails;

public partial class EmailService(ILogger<EmailService> logger, IOptions<EmailConfig> options)
{
    private const string SenderEmail = "noreply@shoply.com";
    private readonly EmailConfig _emailConfig = options.Value;

    public async Task SendEmailAsync(IEmailTemplate template, CancellationToken cancellationToken)
    {
        var mimeMessage = new MimeMessage();

        mimeMessage.From.Add(MailboxAddress.Parse(SenderEmail));
        mimeMessage.To.Add(MailboxAddress.Parse(template.To));
        mimeMessage.Subject = template.Subject;

        mimeMessage.Body = new BodyBuilder
        {
            TextBody = template.Body
        }.ToMessageBody();

        using var client = new SmtpClient();

        try
        {
            await client.ConnectAsync(_emailConfig.Host, _emailConfig.Port, SecureSocketOptions.None,
                cancellationToken);
            await client.SendAsync(mimeMessage, cancellationToken);
            LogEmailSentToRecipient(logger, template.To, template.Subject);
        }
        catch
        {
            LogErrorSendingEmail(logger);
        }
        finally
        {
            await client.DisconnectAsync(true, cancellationToken);
            LogEmailClosed(logger);
        }
    }

    [LoggerMessage(LogLevel.Information, "Email sent to {recipient} with subject {subject}")]
    static partial void LogEmailSentToRecipient(ILogger<EmailService> logger, string recipient, string subject);

    [LoggerMessage(LogLevel.Error, "Error sending email")]
    static partial void LogErrorSendingEmail(ILogger<EmailService> logger);

    [LoggerMessage(LogLevel.Information, "Email Closed")]
    static partial void LogEmailClosed(ILogger<EmailService> logger);
}