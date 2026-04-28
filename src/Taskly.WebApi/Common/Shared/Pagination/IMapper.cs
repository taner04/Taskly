namespace Taskly.WebApi.Common.Shared.Pagination;

public interface IMapper<in TEntity, out TTarget>
{
    IEnumerable<TTarget> Map(IEnumerable<TEntity> source);
}