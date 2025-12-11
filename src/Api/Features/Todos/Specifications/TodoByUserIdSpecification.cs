using Api.Features.Users.Model;
using Ardalis.Specification;

namespace Api.Features.Todos.Specifications;

public class TodoByUserIdSpecification : Specification<Todo>
{
    public TodoByUserIdSpecification(
        TodoId todoId, 
        UserId userId)
    {
        Query.Where(t => t.Id == todoId && t.UserId == userId);
    }
}