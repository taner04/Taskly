namespace Api.Features.Todos;

public static class TodoRoutes
{
    private const string Base = "api/todos";

    public const string GetAll = Base;
    public const string Create = Base;
    public const string GetById = Base + "/{todoId:guid}";
    public const string Update = Base + "/{todoId:guid}";
    public const string Delete = Base + "/{todoId:guid}";
    public const string Complete = Base + "/{todoId:guid}/complete";
}
