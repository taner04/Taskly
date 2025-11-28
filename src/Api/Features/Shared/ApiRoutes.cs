namespace Api.Features.Shared;

public static class ApiRoutes
{
    public static class Todos
    {
        private const string Base = "api/todos";

        public const string GetTodos = Base;
        public const string Create = Base;
        public const string Update = Base + "/{todoId:guid}";
        public const string Delete = Base + "/{todoId:guid}";
        public const string Complete = Base + "/{todoId:guid}/complete";
    }

    public static class Tags
    {
        private const string Base = "api/tags";

        public const string GetTags = Base;
        public const string Create = Base;
        public const string Update = Base + "/{tagId:guid}";
        public const string Delete = Base + "/{tagId:guid}";
    }
}