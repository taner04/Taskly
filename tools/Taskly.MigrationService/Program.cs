using Taskly.WebApi.Common.Infrastructure.Persistence;
using Taskly.MigrationService;
using Taskly.ServiceDefaults;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddHostedService<Worker>();

builder.Services.AddOpenTelemetry().WithTracing(t => { t.AddSource(Worker.ActivitySourceName); });

builder.AddNpgsqlDbContext<TasklyDbContext>(AppHostConstants.Database);

var host = builder.Build();

host.Run();
