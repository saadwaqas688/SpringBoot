using MongoDB.Driver;
using Shared.Common;

namespace Shared.Repositories;

public interface IBaseRepository<T> where T : class
{
    Task<T?> GetByIdAsync(string id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<T> CreateAsync(T entity);
    Task<T?> UpdateAsync(string id, T entity);
    Task<bool> DeleteAsync(string id);
    Task<PagedResponse<T>> GetPagedAsync(int page, int pageSize, FilterDefinition<T>? filter = null, SortDefinition<T>? sort = null);
    Task<IEnumerable<T>> AggregateAsync(PipelineDefinition<T, T> pipeline);
    Task<long> CountAsync(FilterDefinition<T>? filter = null);
    IMongoCollection<T> Collection { get; }
}

