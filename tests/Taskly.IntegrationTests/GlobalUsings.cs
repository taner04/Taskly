// Global using directives

global using System.Data.Common;
global using System.Net;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.Extensions.DependencyInjection;
global using Azure.Storage.Blobs;
global using DotNet.Testcontainers.Builders;
global using FluentAssertions;
global using Taskly.IntegrationTests.Extensions;
global using Taskly.IntegrationTests.Factories;
global using Taskly.IntegrationTests.Infrastructure;
global using Taskly.IntegrationTests.Infrastructure.Fixtures;
global using Taskly.WebApi.Features.Attachments.Models;
global using Taskly.WebApi.Features.Tags.Models;
global using Taskly.WebApi.Features.Todos.Endpoints;
global using Taskly.WebApi.Features.Todos.Models;
global using Taskly.WebApi.Features.Users.Models;
global using TagId = Taskly.WebApi.Features.Tags.Models.TagId;
global using TodoId = Taskly.WebApi.Features.Todos.Models.TodoId;
global using UserId = Taskly.WebApi.Features.Users.Models.UserId;