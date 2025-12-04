namespace Api.Features.Shared;

public static class Routes
{
    public static class Todos
    {
        private const string Base = "api/todos";

        public const string GetTodos = Base;
        public const string Create = Base;
        public const string Update = Base + "/{todoId:guid}";
        public const string Remove = Base + "/{todoId:guid}";
        public const string Complete = Base + "/{todoId:guid}/complete";

        public const string AddTags = Base + "/{todoId:guid}/tags";
        public const string RemoveTag = Base + "/{todoId:guid}/tags/{tagId:guid}";

        public const string AddAttachment = Base + "/{todoId:guid}/attachments";
        public const string RemoveAttachment = Base + "/{todoId:guid}/attachments/{attachmentId:guid}";
    }

    public static class Tags
    {
        private const string Base = "api/tags";

        public const string GetTags = Base;
        public const string Create = Base;
        public const string Update = Base + "/{tagId:guid}";
        public const string Delete = Base + "/{tagId:guid}";
    }

    public static class Attachments
    {
        private const string Base = "api/attachments";

        public const string Download = Base + "/{attachmentId:guid}/download";
        public const string CompleteUpload = Base + "/{attachmentId:guid}/complete";
    }
}