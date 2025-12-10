namespace Api.Features.Shared;

public static class Routes
{
    public static class Todos
    {
        private const string Base = "/api/todos";

        public const string GetTodos = Base;
        public const string Create = Base;
        public const string Update = Base + "/{todoId}";
        public const string Remove = Base + "/{todoId}";
        public const string Complete = Base + "/{todoId}/complete";

        public const string AddTags = Base + "/{todoId}/tags";
        public const string RemoveTag = Base + "/{todoId}/tags/{tagId}";

        public const string AddAttachment = Base + "/{todoId}/attachments";
        public const string RemoveAttachment = Base + "/{todoId}/attachments/{attachmentId}";
        
        public const string UpdateReminder = Base + "/{todoId}/deadline";
        public const string RemoveReminder = Base + "/{todoId}/reminder";
    }


    public static class Tags
    {
        private const string Base = "/api/tags";

        public const string GetTags = Base;
        public const string Create = Base;
        public const string Update = Base + "/{tagId}";
        public const string Delete = Base + "/{tagId}";
    }

    public static class Attachments
    {
        private const string Base = "/api/attachments";

        public const string Download = Base + "/{attachmentId}/download";
        public const string CompleteUpload = Base + "/{attachmentId}/complete";
    }
}