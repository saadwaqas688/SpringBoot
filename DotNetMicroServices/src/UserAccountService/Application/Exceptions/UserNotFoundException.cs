namespace UserAccountService.Application.Exceptions;

/// <summary>
/// Exception thrown when a user is not found.
/// </summary>
public class UserNotFoundException : Exception
{
    public UserNotFoundException(string userId)
        : base($"User with ID '{userId}' was not found.")
    {
        UserId = userId;
    }

    public UserNotFoundException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    public string? UserId { get; }
}


