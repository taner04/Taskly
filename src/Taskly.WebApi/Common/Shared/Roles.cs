namespace Taskly.WebApi.Common.Shared;

public static class Policies
{
    public static class Roles
    {
        public const string Admin = "admin-role";
        public const string User = "user-role";
    }

    public static class RateLimiting
    {
        public const string Global = "global-ratelimit";
        public const string WebHook = "webhook-ratelimit";
    }
}