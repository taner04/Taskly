using Projects;
using ServiceDefaults;

var builder = DistributedApplication.CreateBuilder(args);

var db = builder.AddPostgres("database").WithPgAdmin();

var tasklyDb = db.AddDatabase(AppHostConstants.Database);

var migration = builder.AddProject<MigrationService>(AppHostConstants.MigrationService)
    .WithReference(tasklyDb)
    .WaitFor(tasklyDb);

builder.AddProject<Api>(AppHostConstants.Api)
    .WithReference(tasklyDb)
    .WaitFor(tasklyDb)
    .WaitForCompletion(migration);

builder.Build().Run();