using Taskly.WebApi.Common.Infrastructure.Persistence;
using Taskly.ServiceDefaults;
using Taskly.ReminderService;
using Taskly.ReminderService.Emails;
using Taskly.ReminderService.Processors;
using Taskly.ReminderService.Services;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddHostedService<Worker>();

builder.Services.AddOpenTelemetry().WithTracing(t => { t.AddSource(Worker.ActivitySourceName); });

builder.AddNpgsqlDbContext<TasklyDbContext>(AppHostConstants.Database);

builder.Services.AddScoped<TodoService>();
builder.Services.AddScoped<SendReminderProcessor>();
builder.Services.AddScoped<EmailService>();

var host = builder.Build();

host.Run();
