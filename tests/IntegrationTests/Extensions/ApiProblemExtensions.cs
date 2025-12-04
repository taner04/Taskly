using System.Net.Http.Json;
using Api.Shared;

namespace IntegrationTests.Extensions;

public static class ApiProblemExtensions
{
    public static async Task<ApiProblemDetails> ReadProblemAsync(
        this HttpResponseMessage response)
    {
        var problem = await response.Content.ReadFromJsonAsync<ApiProblemDetails>();

        if (problem is null)
        {
            throw new InvalidOperationException(
                "Expected an ApiProblemDetails response but got null or invalid JSON.");
        }

        return problem;
    }
}