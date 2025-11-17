using Projects;

var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Taskly_Api>("taskly-api");

builder.Build().Run();