using Api.Infrastructure.Data;
using ReminderService;
using ReminderService.Emails;
using ReminderService.Processors;
using ReminderService.Services;
using ServiceDefaults;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddHostedService<Worker>();

builder.Services.AddOpenTelemetry().WithTracing(t => { t.AddSource(Worker.ActivitySourceName); });

builder.AddNpgsqlDbContext<ApplicationDbContext>(AppHostConstants.Database);
builder.Services.AddHostedService<Worker>();

builder.Services.AddScoped<TodoService>();
builder.Services.AddScoped<SendReminderProcessor>();
builder.Services.AddScoped<EmailService>();

var host = builder.Build();

host.Run();