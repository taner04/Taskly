using Google.Protobuf.WellKnownTypes;
using Projects;
using Taskly.ServiceDefaults;

var builder = DistributedApplication.CreateBuilder(args);

var db = builder.AddPostgres("database").WithPgAdmin();

var tasklyDb = db.AddDatabase(AppHostConstants.Database);

var migration = builder.AddProject<Taskly_MigrationService>(AppHostConstants.MigrationService)
    .WithReference(tasklyDb)
    .WaitFor(tasklyDb);

builder.AddProject<Taskly_Api>(AppHostConstants.Api)
    .WithReference(tasklyDb)
    .WaitFor(tasklyDb)
    .WaitForCompletion(migration);

builder.Build().Run();