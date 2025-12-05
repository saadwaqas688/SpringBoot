using System.ComponentModel.DataAnnotations;

namespace Shared.Application.Options;

/// <summary>
/// Configuration options for MongoDB connection.
/// </summary>
public class MongoDbSettings
{
    public const string SectionName = "MongoDb";

    /// <summary>
    /// MongoDB connection string.
    /// </summary>
    [Required(ErrorMessage = "MongoDB ConnectionString is required")]
    public string ConnectionString { get; set; } = "mongodb://localhost:27017";

    /// <summary>
    /// MongoDB database name.
    /// </summary>
    [Required(ErrorMessage = "MongoDB DatabaseName is required")]
    public string DatabaseName { get; set; } = string.Empty;
}

