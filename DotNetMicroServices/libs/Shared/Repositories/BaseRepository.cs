using MongoDB.Driver;
using MongoDB.Bson;
using Shared.Common;
using Microsoft.Extensions.Logging;

namespace Shared.Repositories;

/// <summary>
/// Base repository class providing common CRUD operations for MongoDB collections.
/// This class implements generic repository pattern to reduce code duplication across repositories.
/// </summary>
/// <typeparam name="T">The entity type that this repository manages</typeparam>
public class BaseRepository<T> : IBaseRepository<T> where T : class
{
    protected readonly IMongoCollection<T> _collection;
    protected readonly ILogger<BaseRepository<T>> _logger;

    /// <summary>
    /// Initializes a new instance of the BaseRepository class.
    /// </summary>
    /// <param name="collection">The MongoDB collection to perform operations on</param>
    /// <param name="logger">Logger instance for logging errors and operations</param>
    public BaseRepository(IMongoCollection<T> collection, ILogger<BaseRepository<T>> logger)
    {
        _collection = collection;
        _logger = logger;
    }

    /// <summary>
    /// Gets the MongoDB collection instance for direct access if needed.
    /// </summary>
    public IMongoCollection<T> Collection => _collection;

    /// <summary>
    /// Retrieves a single entity by its unique identifier.
    /// Supports both ObjectId and string-based IDs by attempting to parse the ID as ObjectId first.
    /// </summary>
    /// <param name="id">The unique identifier of the entity to retrieve (can be ObjectId or string)</param>
    /// <returns>The entity if found, null otherwise</returns>
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

    /// <summary>
    /// Retrieves all entities from the collection without any filtering or pagination.
    /// Use with caution on large collections as it loads all documents into memory.
    /// </summary>
    /// <returns>A collection of all entities in the database</returns>
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

    /// <summary>
    /// Creates a new entity in the MongoDB collection.
    /// MongoDB will automatically generate an ObjectId if the entity doesn't have an ID.
    /// </summary>
    /// <param name="entity">The entity object to insert into the collection</param>
    /// <returns>The created entity (with generated ID if applicable)</returns>
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

    /// <summary>
    /// Updates an existing entity by replacing it entirely with the provided entity.
    /// Uses ReplaceOne operation which replaces the entire document, not just changed fields.
    /// Supports both ObjectId and string-based IDs.
    /// </summary>
    /// <param name="id">The unique identifier of the entity to update</param>
    /// <param name="entity">The updated entity object that will replace the existing one</param>
    /// <returns>The updated entity if the update was successful, null if entity was not found</returns>
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

    /// <summary>
    /// Deletes an entity from the collection by its unique identifier.
    /// Supports both ObjectId and string-based IDs.
    /// </summary>
    /// <param name="id">The unique identifier of the entity to delete</param>
    /// <returns>True if the entity was successfully deleted, false if it was not found</returns>
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

    /// <summary>
    /// Retrieves a paginated subset of entities from the collection.
    /// Supports optional filtering and sorting. Useful for displaying large datasets in pages.
    /// </summary>
    /// <param name="page">The page number (1-based index)</param>
    /// <param name="pageSize">The number of items per page</param>
    /// <param name="filter">Optional MongoDB filter definition to filter results. If null, returns all entities.</param>
    /// <param name="sort">Optional MongoDB sort definition to order results. Defaults to descending by _id if not provided.</param>
    /// <returns>A PagedResponse object containing the items for the requested page, page number, page size, and total count</returns>
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

    /// <summary>
    /// Executes a MongoDB aggregation pipeline on the collection.
    /// Aggregation pipelines allow complex data processing, transformations, and calculations.
    /// Use this for advanced queries that require grouping, joining, or complex filtering.
    /// </summary>
    /// <param name="pipeline">The MongoDB aggregation pipeline definition containing stages to execute</param>
    /// <returns>A collection of entities resulting from the aggregation pipeline</returns>
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

    /// <summary>
    /// Counts the number of entities in the collection matching the optional filter.
    /// Useful for getting total counts before pagination or for statistics.
    /// </summary>
    /// <param name="filter">Optional MongoDB filter definition to count only matching entities. If null, counts all entities.</param>
    /// <returns>The total count of entities matching the filter (or all entities if filter is null)</returns>
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

