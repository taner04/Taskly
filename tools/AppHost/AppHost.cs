using Projects;
using ServiceDefaults;

var builder = DistributedApplication.CreateBuilder(args);

var db = builder.AddPostgres("database").WithPgAdmin();

var tasklyDb = db.AddDatabase(AppHostConstants.Database);

var migration = builder.AddProject<MigrationService>(AppHostConstants.MigrationService)
    .WithReference(tasklyDb)
    .WaitFor(tasklyDb);

var blobStorage = builder.AddContainer(AppHostConstants.Azure, "mcr.microsoft.com/azure-storage/azurite:latest")
    .WithVolume("/data")
    .WithEntrypoint("azurite")
    .WithArgs("--blobHost", "0.0.0.0",
        "--queueHost", "0.0.0.0",
        "--tableHost", "0.0.0.0",
        "--loose")
    .WithHttpEndpoint(10000, 10000, "blob") // Blob service 
    .WithHttpEndpoint(10001, 10001, "queue") // Queue service
    .WithHttpEndpoint(10002, 10002, "table"); // Table service


var api = builder.AddProject<Api>(AppHostConstants.Api)
    .WithReference(tasklyDb)
    .WaitFor(tasklyDb)
    .WaitFor(blobStorage)
    .WaitForCompletion(migration);

builder.AddContainer(AppHostConstants.Web, "taskly-web")
    .WithHttpEndpoint(3000, 3000, "http")
    .WaitFor(api);

builder.Build().Run();