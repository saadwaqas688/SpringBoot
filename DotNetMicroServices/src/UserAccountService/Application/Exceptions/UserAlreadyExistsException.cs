namespace UserAccountService.Application.Exceptions;

/// <summary>
/// Exception thrown when attempting to create a user that already exists.
/// </summary>
public class UserAlreadyExistsException : Exception
{
    public UserAlreadyExistsException(string email)
        : base($"User with email '{email}' already exists.")
    {
        Email = email;
    }

    public string Email { get; }
}


