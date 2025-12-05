namespace Shared.Application.Options;

/// <summary>
/// Configuration options for CORS policy.
/// </summary>
public class CorsSettings
{
    public const string SectionName = "Cors";

    /// <summary>
    /// Allowed origins for CORS. If empty, allows all origins (development only).
    /// </summary>
    public string[] AllowedOrigins { get; set; } = Array.Empty<string>();

    /// <summary>
    /// Allowed methods for CORS. If empty, allows all methods.
    /// </summary>
    public string[] AllowedMethods { get; set; } = Array.Empty<string>();

    /// <summary>
    /// Allowed headers for CORS. If empty, allows all headers.
    /// </summary>
    public string[] AllowedHeaders { get; set; } = Array.Empty<string>();

    /// <summary>
    /// Whether to allow credentials. Default is false.
    /// </summary>
    public bool AllowCredentials { get; set; } = false;
}

