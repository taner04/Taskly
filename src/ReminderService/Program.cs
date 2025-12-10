using Api.Infrastructure.Data;
using ReminderService;
using ServiceDefaults;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddHostedService<Worker>();

builder.Services.AddOpenTelemetry().WithTracing(t => { t.AddSource(Worker.ActivitySourceName); });

builder.AddNpgsqlDbContext<ApplicationDbContext>(AppHostConstants.Database);
builder.Services.AddSingleton<EmailService>();

var host = builder.Build();

host.Run();