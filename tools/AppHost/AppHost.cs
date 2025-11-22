using Projects;
using ServiceDefaults;

var builder = DistributedApplication.CreateBuilder(args);

var db = builder.AddPostgres("database").WithPgAdmin();

var tasklyDb = db.AddDatabase(AppHostConstants.Database);

var migration = builder.AddProject<MigrationService>(AppHostConstants.MigrationService)
    .WithReference(tasklyDb)
    .WaitFor(tasklyDb);

var api = builder.AddProject<Api>(AppHostConstants.Api)
    .WithReference(tasklyDb)
    .WaitFor(tasklyDb)
    .WaitForCompletion(migration);

builder.AddContainer(AppHostConstants.Web, "taskly-web")
    .WithHttpEndpoint(3000, 3000, name: "http")
    .WaitFor(api);

builder.Build().Run();