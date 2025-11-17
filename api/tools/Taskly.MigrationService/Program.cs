using Taskly.MigrationService;
using Taskly.Api.Infrastructure.Data;
using Taskly.ServiceDefaults;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddHostedService<Worker>();

builder.Services.AddOpenTelemetry().WithTracing(t => { t.AddSource(Worker.ActivitySourceName); });

builder.AddNpgsqlDbContext<ApplicationDbContext>(AppHostConstants.Database);

var host = builder.Build();

host.Run();