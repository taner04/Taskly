using Projects;
using ServiceDefaults;

var builder = DistributedApplication.CreateBuilder(args);

var db = builder.AddPostgres("database").WithPgAdmin();

var tasklyDb = db.AddDatabase(AppHostConstants.Database);

var migration = builder.AddProject<MigrationService>(AppHostConstants.MigrationService)
    .WithReference(tasklyDb)
    .WaitFor(tasklyDb);

var blobStorage = builder.AddContainer(AppHostConstants.BlobStorage, "mcr.microsoft.com/azure-storage/azurite:3.14.0")
    .WithVolume("/data")
    .WithHttpEndpoint(10000, 10000, "blob");

var api = builder.AddProject<Api>(AppHostConstants.Api)
    .WithReference(tasklyDb)
    .WaitFor(tasklyDb)
    .WaitFor(blobStorage)
    .WaitForCompletion(migration);

builder.AddContainer(AppHostConstants.Web, "taskly-web")
    .WithHttpEndpoint(3000, 3000, "http")
    .WaitFor(api);

builder.Build().Run();