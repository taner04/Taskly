using System.Text;
using Taskly.WebApi.Common.Infrastructure.Services.Emails;

namespace Taskly.WebApi.Features.Todos.EmailTemplates;

internal sealed class ReminderEmailTemplate(string userEmail, Todo todo) : IEmailTemplate
{
    public string To => userEmail;
    public string Subject => "Task Reminder - Action Required";
    public string Body => BuildBody(todo);

    private static string BuildBody(Todo todo)
    {
        var sb = new StringBuilder();

        AppendHtmlHeader(sb);
        AppendStyles(sb);
        AppendBodyHeader(sb);
        AppendTaskInfo(sb, todo);
        AppendDeadlineInfo(sb, todo);
        AppendCallToAction(sb);
        AppendFooter(sb);
        AppendHtmlFooter(sb);

        return sb.ToString();
    }

    private static void AppendHtmlHeader(StringBuilder sb)
    {
        sb.AppendLine("<!DOCTYPE html>");
        sb.AppendLine("<html lang=\"en\">");
        sb.AppendLine("<head>");
        sb.AppendLine("<meta charset=\"UTF-8\">");
        sb.AppendLine("<meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">");
    }

    private static void AppendStyles(StringBuilder sb)
    {
        sb.AppendLine("<style>");
        sb.AppendLine("* { margin: 0; padding: 0; box-sizing: border-box; }");
        sb.AppendLine(
            "body { font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; color: #333; background-color: #f5f5f5; }");
        sb.AppendLine(".email-container { max-width: 600px; margin: 0 auto; background-color: #ffffff; }");
        sb.AppendLine(
            ".header { background: linear-gradient(135deg, #3498db 0%, #2980b9 100%); color: white; padding: 40px 20px; text-align: center; }");
        sb.AppendLine(".header h1 { font-size: 28px; margin-bottom: 10px; }");
        sb.AppendLine(".header p { font-size: 14px; opacity: 0.9; }");
        sb.AppendLine(".content { padding: 40px 20px; }");
        sb.AppendLine(
            ".reminder-info { background-color: #f0f8ff; padding: 15px; border-left: 4px solid #3498db; margin-bottom: 30px; border-radius: 4px; }");
        sb.AppendLine(".reminder-info p { font-size: 14px; color: #666; }");
        sb.AppendLine(
            ".reminder-info h2 { font-size: 18px; color: #333; margin-bottom: 20px; border-bottom: 2px solid #3498db; padding-bottom: 10px; }");
        sb.AppendLine(
            "h2 { font-size: 18px; color: #333; margin-bottom: 20px; border-bottom: 2px solid #3498db; padding-bottom: 10px; }");
        sb.AppendLine(
            ".info-section { background-color: #e7f3ff; padding: 15px; border-left: 4px solid #3498db; border-radius: 4px; margin-bottom: 20px; font-size: 14px; color: #1e5a8e; }");
        sb.AppendLine(
            ".footer { background-color: #f5f5f5; padding: 30px 20px; text-align: center; border-top: 1px solid #ddd; }");
        sb.AppendLine(".footer p { font-size: 12px; color: #999; margin-bottom: 10px; }");
        sb.AppendLine(
            ".cta-button { display: inline-block; background-color: #3498db; color: white; padding: 12px 30px; text-decoration: none; border-radius: 4px; font-weight: 600; margin: 20px 0; }");
        sb.AppendLine(".cta-button:hover { background-color: #2980b9; }");
        sb.AppendLine("</style>");
        sb.AppendLine("</head>");
    }

    private static void AppendBodyHeader(StringBuilder sb)
    {
        sb.AppendLine("<body>");
        sb.AppendLine("<div class=\"email-container\">");
        sb.AppendLine("<div class=\"header\">");
        sb.AppendLine("<h1>📋 Task Reminder</h1>");
        sb.AppendLine("<p>You have a task that needs your attention</p>");
        sb.AppendLine("</div>");
        sb.AppendLine("<div class=\"content\">");
    }

    private static void AppendTaskInfo(StringBuilder sb, Todo todo)
    {
        sb.AppendLine("<div class=\"reminder-info\">");
        sb.AppendLine($"<h2>{EscapeHtml(todo.Title)}</h2>");
        if (!string.IsNullOrEmpty(todo.Description))
        {
            sb.AppendLine($"<p>{EscapeHtml(todo.Description)}</p>");
        }

        sb.AppendLine("</div>");
    }

    private static void AppendDeadlineInfo(StringBuilder sb, Todo todo)
    {
        sb.AppendLine("<div class=\"info-section\">");
        sb.AppendLine("<strong>Deadline Information</strong><br>");
        if (todo.Deadline.HasValue)
        {
            sb.AppendLine($"Due: <strong>{todo.Deadline.Value:MMMM dd, yyyy 'at' hh:mm tt}</strong><br>");
        }

        sb.AppendLine("Please complete this task as soon as possible.");
        sb.AppendLine("</div>");
    }

    private static void AppendCallToAction(StringBuilder sb)
    {
        sb.AppendLine("<center>");
        sb.AppendLine("<a href=\"#\" class=\"cta-button\">View Task</a>");
        sb.AppendLine("</center>");
    }

    private static void AppendFooter(StringBuilder sb)
    {
        sb.AppendLine("</div>");
        sb.AppendLine("<div class=\"footer\">");
        sb.AppendLine("<p>This is an automated reminder email from Taskly</p>");
        sb.AppendLine("<p>&copy; 2026 Taskly. All rights reserved.</p>");
        sb.AppendLine(
            "<p><a href=\"#\" style=\"color: #3498db; text-decoration: none;\">Unsubscribe</a> | <a href=\"#\" style=\"color: #3498db; text-decoration: none;\">Privacy Policy</a></p>");
        sb.AppendLine("</div>");
    }

    private static void AppendHtmlFooter(StringBuilder sb)
    {
        sb.AppendLine("</div>");
        sb.AppendLine("</body>");
        sb.AppendLine("</html>");
    }

    private static string EscapeHtml(string text) =>
        text
            .Replace("&", "&amp;")
            .Replace("<", "&lt;")
            .Replace(">", "&gt;")
            .Replace("\"", "&quot;")
            .Replace("'", "&#39;");
}