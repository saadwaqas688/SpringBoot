using MongoDB.Driver;
using MongoDB.Bson;
using Shared.Common;
using Microsoft.Extensions.Logging;

namespace Shared.Repositories;

public class BaseRepository<T> : IBaseRepository<T> where T : class
{
    protected readonly IMongoCollection<T> _collection;
    protected readonly ILogger<BaseRepository<T>> _logger;

    public BaseRepository(IMongoCollection<T> collection, ILogger<BaseRepository<T>> logger)
    {
        _collection = collection;
        _logger = logger;
    }

    public IMongoCollection<T> Collection => _collection;

    public virtual async Task<T?> GetByIdAsync(string id)
    {
        try
        {
            // Try to parse as ObjectId first, fallback to string if invalid
            FilterDefinition<T> filter;
            if (ObjectId.TryParse(id, out var objectId))
            {
                filter = Builders<T>.Filter.Eq("_id", objectId);
            }
            else
            {
                filter = Builders<T>.Filter.Eq("_id", id);
            }
            return await _collection.Find(filter).FirstOrDefaultAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting entity by id: {Id}", id);
            throw;
        }
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync()
    {
        try
        {
            return await _collection.Find(_ => true).ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all entities");
            throw;
        }
    }

    public virtual async Task<T> CreateAsync(T entity)
    {
        try
        {
            await _collection.InsertOneAsync(entity);
            return entity;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating entity");
            throw;
        }
    }

    public virtual async Task<T?> UpdateAsync(string id, T entity)
    {
        try
        {
            FilterDefinition<T> filter;
            if (ObjectId.TryParse(id, out var objectId))
            {
                filter = Builders<T>.Filter.Eq("_id", objectId);
            }
            else
            {
                filter = Builders<T>.Filter.Eq("_id", id);
            }
            var result = await _collection.ReplaceOneAsync(filter, entity);
            return result.ModifiedCount > 0 ? entity : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating entity with id: {Id}", id);
            throw;
        }
    }

    public virtual async Task<bool> DeleteAsync(string id)
    {
        try
        {
            FilterDefinition<T> filter;
            if (ObjectId.TryParse(id, out var objectId))
            {
                filter = Builders<T>.Filter.Eq("_id", objectId);
            }
            else
            {
                filter = Builders<T>.Filter.Eq("_id", id);
            }
            var result = await _collection.DeleteOneAsync(filter);
            return result.DeletedCount > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting entity with id: {Id}", id);
            throw;
        }
    }

    public virtual async Task<PagedResponse<T>> GetPagedAsync(
        int page, 
        int pageSize, 
        FilterDefinition<T>? filter = null, 
        SortDefinition<T>? sort = null)
    {
        try
        {
            filter ??= Builders<T>.Filter.Empty;
            sort ??= Builders<T>.Sort.Descending("_id");

            var totalCount = await _collection.CountDocumentsAsync(filter);
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var skip = (page - 1) * pageSize;
            var items = await _collection
                .Find(filter)
                .Sort(sort)
                .Skip(skip)
                .Limit(pageSize)
                .ToListAsync();

            return new PagedResponse<T>
            {
                Items = items,
                PageNumber = page,
                PageSize = pageSize,
                TotalCount = (int)totalCount
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting paged entities");
            throw;
        }
    }

    public virtual async Task<IEnumerable<T>> AggregateAsync(PipelineDefinition<T, T> pipeline)
    {
        try
        {
            return await _collection.Aggregate(pipeline).ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing aggregation pipeline");
            throw;
        }
    }

    public virtual async Task<long> CountAsync(FilterDefinition<T>? filter = null)
    {
        try
        {
            filter ??= Builders<T>.Filter.Empty;
            return await _collection.CountDocumentsAsync(filter);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error counting entities");
            throw;
        }
    }
}

