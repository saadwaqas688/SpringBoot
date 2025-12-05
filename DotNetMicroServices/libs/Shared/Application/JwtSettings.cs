using System.ComponentModel.DataAnnotations;

namespace Shared.Application.Options;

/// <summary>
/// Configuration options for JWT authentication.
/// </summary>
public class JwtSettings
{
    public const string SectionName = "JwtSettings";

    /// <summary>
    /// Secret key for signing JWT tokens. Must be at least 32 characters long.
    /// </summary>
    [Required(ErrorMessage = "JWT SecretKey is required")]
    [MinLength(32, ErrorMessage = "JWT SecretKey must be at least 32 characters long")]
    public string SecretKey { get; set; } = string.Empty;

    /// <summary>
    /// Issuer of the JWT token.
    /// </summary>
    [Required(ErrorMessage = "JWT Issuer is required")]
    public string Issuer { get; set; } = string.Empty;

    /// <summary>
    /// Audience of the JWT token.
    /// </summary>
    [Required(ErrorMessage = "JWT Audience is required")]
    public string Audience { get; set; } = string.Empty;

    /// <summary>
    /// Token expiration time in minutes. Default is 60 minutes.
    /// </summary>
    [Range(1, 1440, ErrorMessage = "Token expiration must be between 1 and 1440 minutes (24 hours)")]
    public int ExpirationMinutes { get; set; } = 60;
}

