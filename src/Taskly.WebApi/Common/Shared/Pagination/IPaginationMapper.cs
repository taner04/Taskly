namespace Taskly.WebApi.Common.Shared.Pagination;

public interface IPaginationMapper<TEntity, TTarget>
{
    List<TTarget> Map(List<TEntity> source);
}