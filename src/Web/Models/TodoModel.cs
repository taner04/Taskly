namespace Web.Models
{
    public class TodoModel
    {
        public Guid Id { get; init; }
        public string Title { get; init; }
        public string? Description { get; init; }
        public string Priority { get; init; }
        public bool IsCompleted { get; init; }
    }
}
