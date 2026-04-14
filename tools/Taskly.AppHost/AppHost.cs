using Projects;
using Taskly.ServiceDefaults;

var builder = DistributedApplication.CreateBuilder(args);

var db = builder.AddPostgres("database").WithPgAdmin();

var tasklyDb = db.AddDatabase(AppHostConstants.Database);

var migration = builder.AddProject<Taskly_MigrationService>(AppHostConstants.MigrationService)
    .WithReference(tasklyDb)
    .WaitFor(tasklyDb);

// var blobStorage = builder.AddContainer(AppHostConstants.Azure, "mcr.microsoft.com/azure-storage/azurite:latest")
//     .WithVolume("azure-data", "/data")
//     .WithEntrypoint("azurite")
//     .WithArgs("--blobHost", "0.0.0.0",
//         "--queueHost", "0.0.0.0",
//         "--tableHost", "0.0.0.0",
//         "--loose",
//         "--skipApiVersionCheck")
//     .WithHttpEndpoint(10000, 10000, "blob"); // Blob service 

var blobStorage = builder.AddAzureStorage(AppHostConstants.AzureBlobStorage)
    .RunAsEmulator(r => r.WithImage("azure-storage/azurite", "3.35.0"))
    .AddBlobs(AppHostConstants.AzureBlobContainerName);

var papercut = builder.AddPapercutSmtp("papercut", 80, 25);

builder.AddProject<Taskly_WebApi>(AppHostConstants.Api)
    .WithReference(tasklyDb)
    .WaitFor(tasklyDb)
    .WithReference(blobStorage)
    .WaitFor(blobStorage)
    .WithReference(papercut)
    .WaitFor(papercut)
    .WaitForCompletion(migration);

builder.Build().Run();