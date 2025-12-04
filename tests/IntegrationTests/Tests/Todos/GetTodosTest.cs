using System.Net;
using System.Net.Http.Json;
using Api.Features.Attachments.Models;
using Api.Features.Tags.Model;
using Api.Features.Todos.Endpoints;
using Api.Features.Todos.Model;
using Api.Shared.Api;
using IntegrationTests.Extensions;

namespace IntegrationTests.Tests.Todos;

public class GetTodosTests(TestingFixture fixture) : TestingBase(fixture)
{
    private const string Url = Routes.Todos.GetTodos;

    [Fact]
    public async Task GetTodos_ReturnsTodosForAuthenticatedUser()
    {
        var client = CreateAuthenticatedClient();
        var userId = client.GetUserId();

        var todoA = new Todo("AAA", "Desc A", TodoPriority.Low, userId);
        var todoB = new Todo("BAA", "Desc B", TodoPriority.High, userId);

        DbContext.Todos.Add(todoA);
        DbContext.Todos.Add(todoB);

        await DbContext.SaveChangesAsync(CurrentCancellationToken);

        var response = await client.GetAsync(Url, CurrentCancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var dtos = await response.Content.ReadFromJsonAsync<List<GetTodos.TodoDto>>(CurrentCancellationToken);
        Assert.NotNull(dtos);

        Assert.Equal(2, dtos!.Count);
        Assert.Contains(dtos, t => t.Id == todoA.Id.Value);
        Assert.Contains(dtos, t => t.Id == todoB.Id.Value);
    }

    [Fact]
    public async Task GetTodos_DoesNotReturnTodosFromOtherUsers()
    {
        var client = CreateAuthenticatedClient();
        var userId = client.GetUserId();

        var ownTodo = new Todo("Mine", "Desc", TodoPriority.Medium, userId);
        var foreignTodo = new Todo("NotMine", "Desc", TodoPriority.High, "auth0|other");

        DbContext.Todos.AddRange(ownTodo, foreignTodo);
        await DbContext.SaveChangesAsync(CurrentCancellationToken);

        var response = await client.GetAsync(Url, CurrentCancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var dtos = await response.Content.ReadFromJsonAsync<List<GetTodos.TodoDto>>(CurrentCancellationToken);
        Assert.NotNull(dtos);

        Assert.Single(dtos!);
        Assert.Equal(ownTodo.Id.Value, dtos.First().Id);
    }

    [Fact]
    public async Task GetTodos_ReturnsTagsAndAttachments()
    {
        var client = CreateAuthenticatedClient();
        var userId = client.GetUserId();

        var todo = new Todo("Task", "Desc", TodoPriority.Low, userId);

        var tag = new Tag("Work", userId);
        todo.Tags.Add(tag);

        var attachment = Attachment.CreatePending(todo.Id, "file.txt", "text/plain");
        attachment.MarkUploaded(10);
        todo.Attachments.Add(attachment);

        DbContext.Todos.Add(todo);
        DbContext.Tags.Add(tag);
        DbContext.Attachments.Add(attachment);

        await DbContext.SaveChangesAsync(CurrentCancellationToken);

        var response = await client.GetAsync(Url, CurrentCancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var dtos = await response.Content.ReadFromJsonAsync<List<GetTodos.TodoDto>>(CurrentCancellationToken);
        Assert.NotNull(dtos);

        var dto = dtos.Single();

        Assert.Single(dto.Tags);
        Assert.Equal(tag.Id.Value, dto.Tags[0].Id);

        Assert.Single(dto.Attachments);
        Assert.Equal(attachment.Id.Value, dto.Attachments[0].Id);
        Assert.Equal("file.txt", dto.Attachments[0].FileName);
    }

    [Fact]
    public async Task GetTodos_WhenUserNotAuthenticated_ReturnsUnauthorized()
    {
        var client = CreateUnauthenticatedClient();

        var response = await client.GetAsync(Url, CurrentCancellationToken);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}