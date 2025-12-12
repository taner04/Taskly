using Api.Features.Todos.Model;
using MimeKit;

namespace ReminderService.Emails;

internal static class EmailBuilder
{
    internal static MimeMessage Build(
        string userEmail,
        List<Todo> todos)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("Reminder Service", "no-reply@taskly.com"));
        message.To.Add(MailboxAddress.Parse(userEmail));
        message.Subject = "Taskly - Upcoming todo reminders";

        message.Body = new BodyBuilder
        {
            HtmlBody = "<h1>Your upcoming todo reminders</h1><ul>" +
                       string.Join("", todos.Select(t =>
                           $"<li><strong>{t.Title}</strong>: Due at {t.Deadline:u}</li>")) +
                       "</ul>"
        }.ToMessageBody();

        return message;
    }
}