namespace Taskly.WebApi.Common.Shared.Pagination;

public interface IPaginationMapper<in TEntity, out TTarget>
{
    IEnumerable<TTarget> Map(IEnumerable<TEntity> source);
}