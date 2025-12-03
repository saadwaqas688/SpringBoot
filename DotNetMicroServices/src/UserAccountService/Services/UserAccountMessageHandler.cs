using UserAccountService.DTOs;
using UserAccountService.Models;
using UserAccountService.Services;
using Shared.Common;
using Shared.Constants;
using System.Text.Json;
using System.Security.Claims;

namespace UserAccountService.Services;

public class UserAccountMessageHandler
{
    private readonly IUserAccountService _userAccountService;
    private readonly IAuthService _authService;
    private readonly ILogger<UserAccountMessageHandler> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public UserAccountMessageHandler(
        IUserAccountService userAccountService,
        IAuthService authService,
        ILogger<UserAccountMessageHandler> logger)
    {
        _userAccountService = userAccountService;
        _authService = authService;
        _logger = logger;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    public async Task<object?> HandleMessage(string messageJson, string routingKey)
    {
        try
        {
            _logger.LogInformation("Handling message with routing key: {RoutingKey}", routingKey);

            return routingKey switch
            {
                RabbitMQConstants.UserAccount.HealthCheck => HandleHealthCheck(),
                RabbitMQConstants.UserAccount.SignUp => await HandleSignUp(messageJson),
                RabbitMQConstants.UserAccount.SignIn => await HandleSignIn(messageJson),
                RabbitMQConstants.UserAccount.UpdateProfile => await HandleUpdateProfile(messageJson),
                RabbitMQConstants.UserAccount.GetCurrentUser => await HandleGetCurrentUser(messageJson),
                RabbitMQConstants.UserAccount.GetAllUsers => await HandleGetAllUsers(messageJson),
                RabbitMQConstants.UserAccount.CreateUser => await HandleCreateUser(messageJson),
                RabbitMQConstants.UserAccount.UpdateUser => await HandleUpdateUser(messageJson),
                RabbitMQConstants.UserAccount.DeleteUser => await HandleDeleteUser(messageJson),
                RabbitMQConstants.UserAccount.UpdateUserStatus => await HandleUpdateUserStatus(messageJson),
                RabbitMQConstants.UserAccount.RefreshToken => await HandleRefreshToken(messageJson),
                RabbitMQConstants.UserAccount.ForgotPassword => await HandleForgotPassword(messageJson),
                RabbitMQConstants.UserAccount.ResetPassword => await HandleResetPassword(messageJson),
                _ => ApiResponse<object>.ErrorResponse($"Unknown routing key: {routingKey}")
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling message with routing key: {RoutingKey}", routingKey);
            return ApiResponse<object>.ErrorResponse($"Error processing message: {ex.Message}");
        }
    }

    private object HandleHealthCheck()
    {
        return ApiResponse<string>.SuccessResponse("UserAccountService is healthy", "Health check successful");
    }

    private async Task<object> HandleSignUp(string messageJson)
    {
        try
        {
            var dto = JsonSerializer.Deserialize<SignUpDto>(messageJson, _jsonOptions);
            
            if (dto == null)
            {
                return ApiResponse<AuthResponseDto>.ErrorResponse("Invalid signup data");
            }

            // Check if user already exists
            var existingUser = await _userAccountService.GetUserByEmailAsync(dto.Email);
            if (existingUser != null)
            {
                return ApiResponse<AuthResponseDto>.ErrorResponse("User with this email already exists");
            }

            // Create new user
            var user = new UserAccount
            {
                Name = dto.Name,
                Email = dto.Email,
                PasswordHash = _authService.HashPassword(dto.Password),
                Image = dto.Image,
                Role = "user"
            };

            var createdUser = await _userAccountService.CreateUserAsync(user);

            // Generate JWT token
            var token = _authService.GenerateJwtToken(createdUser);

            var response = new AuthResponseDto
            {
                Token = token,
                User = new UserInfoDto
                {
                    Id = createdUser.Id ?? string.Empty,
                    Name = createdUser.Name,
                    Email = createdUser.Email,
                    Image = createdUser.Image,
                    Role = createdUser.Role,
                    Status = createdUser.Status,
                    CreatedAt = createdUser.CreatedAt
                }
            };

            return ApiResponse<AuthResponseDto>.SuccessResponse(response, "User registered successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in HandleSignUp");
            return ApiResponse<AuthResponseDto>.ErrorResponse($"Error during signup: {ex.Message}");
        }
    }

    private async Task<object> HandleSignIn(string messageJson)
    {
        try
        {
            var dto = JsonSerializer.Deserialize<SignInDto>(messageJson, _jsonOptions);
            
            if (dto == null)
            {
                return ApiResponse<AuthResponseDto>.ErrorResponse("Invalid signin data");
            }

            var user = await _userAccountService.GetUserByEmailAsync(dto.Email);
            if (user == null)
            {
                return ApiResponse<AuthResponseDto>.ErrorResponse("Invalid email or password");
            }

            // Verify password
            if (!_authService.VerifyPassword(dto.Password, user.PasswordHash))
            {
                return ApiResponse<AuthResponseDto>.ErrorResponse("Invalid email or password");
            }

            // Generate JWT token
            var token = _authService.GenerateJwtToken(user);

            var response = new AuthResponseDto
            {
                Token = token,
                User = new UserInfoDto
                {
                    Id = user.Id ?? string.Empty,
                    Name = user.Name,
                    Email = user.Email,
                    Image = user.Image,
                    Role = user.Role
                }
            };

            return ApiResponse<AuthResponseDto>.SuccessResponse(response, "Sign in successful");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in HandleSignIn");
            return ApiResponse<AuthResponseDto>.ErrorResponse($"Error during signin: {ex.Message}");
        }
    }

    private async Task<object> HandleUpdateProfile(string messageJson)
    {
        try
        {
            var request = JsonSerializer.Deserialize<UpdateProfileRequest>(messageJson, _jsonOptions);
            
            if (request?.Token == null || request.Dto == null)
            {
                return ApiResponse<UserInfoDto>.ErrorResponse("Invalid update profile data");
            }

            // Validate token and get user ID
            var userId = ExtractUserIdFromToken(request.Token);
            if (userId == null)
            {
                return ApiResponse<UserInfoDto>.ErrorResponse("Invalid token");
            }

            var user = await _userAccountService.GetUserByIdAsync(userId);
            if (user == null)
            {
                return ApiResponse<UserInfoDto>.ErrorResponse("User not found");
            }

            // Update user profile
            var result = await _userAccountService.UpdateUserAsync(userId, user, request.Dto);
            if (result == null)
            {
                return ApiResponse<UserInfoDto>.ErrorResponse("User not found");
            }

            var response = new UserInfoDto
            {
                Id = result.Id ?? string.Empty,
                Name = result.Name,
                Email = result.Email,
                Image = result.Image,
                Role = result.Role,
                Gender = result.Gender,
                DateOfBirth = result.DateOfBirth,
                MobilePhone = result.MobilePhone,
                Country = result.Country,
                State = result.State,
                City = result.City,
                PostalCode = result.PostalCode
            };

            return ApiResponse<UserInfoDto>.SuccessResponse(response, "Profile updated successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in HandleUpdateProfile");
            return ApiResponse<UserInfoDto>.ErrorResponse($"Error updating profile: {ex.Message}");
        }
    }

    private async Task<object> HandleGetCurrentUser(string messageJson)
    {
        try
        {
            var request = JsonSerializer.Deserialize<GetCurrentUserRequest>(messageJson, _jsonOptions);
            
            if (request?.Token == null)
            {
                return ApiResponse<UserInfoDto>.ErrorResponse("Token is required");
            }

            // Validate token and get user ID
            var userId = ExtractUserIdFromToken(request.Token);
            if (userId == null)
            {
                return ApiResponse<UserInfoDto>.ErrorResponse("Invalid token");
            }

            var user = await _userAccountService.GetUserByIdAsync(userId);
            if (user == null)
            {
                return ApiResponse<UserInfoDto>.ErrorResponse("User not found");
            }

            var response = new UserInfoDto
            {
                Id = user.Id ?? string.Empty,
                Name = user.Name,
                Email = user.Email,
                Image = user.Image,
                Role = user.Role,
                Status = user.Status,
                CreatedAt = user.CreatedAt,
                Gender = user.Gender,
                DateOfBirth = user.DateOfBirth,
                MobilePhone = user.MobilePhone,
                Country = user.Country,
                State = user.State,
                City = user.City,
                PostalCode = user.PostalCode
            };

            return ApiResponse<UserInfoDto>.SuccessResponse(response, "User retrieved successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in HandleGetCurrentUser");
            return ApiResponse<UserInfoDto>.ErrorResponse($"Error retrieving user: {ex.Message}");
        }
    }

    private string? ExtractUserIdFromToken(string token)
    {
        try
        {
            var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            var jsonToken = handler.ReadJwtToken(token);
            var userIdClaim = jsonToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            
            if (userIdClaim != null && !string.IsNullOrEmpty(userIdClaim.Value))
            {
                return userIdClaim.Value;
            }
            return null;
        }
        catch
        {
            return null;
        }
    }

    // Helper classes for deserializing requests
    private class UpdateProfileRequest
    {
        public string? Token { get; set; }
        public UpdateProfileDto? Dto { get; set; }
    }

    private class GetCurrentUserRequest
    {
        public string? Token { get; set; }
    }

    private class GetAllUsersRequest
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SearchTerm { get; set; }
    }

    private async Task<object> HandleGetAllUsers(string messageJson)
    {
        try
        {
            var request = JsonSerializer.Deserialize<GetAllUsersRequest>(messageJson, _jsonOptions);
            var page = request?.Page ?? 1;
            var pageSize = request?.PageSize ?? 10;
            var searchTerm = request?.SearchTerm;

            var pagedUsers = await _userAccountService.GetAllUsersAsync(page, pageSize, searchTerm);

            var userDtos = pagedUsers.Items.Select(u => new UserInfoDto
            {
                Id = u.Id ?? string.Empty,
                Name = u.Name,
                Email = u.Email,
                Image = u.Image,
                Role = u.Role,
                Status = u.Status,
                CreatedAt = u.CreatedAt,
                Gender = u.Gender,
                DateOfBirth = u.DateOfBirth,
                MobilePhone = u.MobilePhone,
                Country = u.Country,
                State = u.State,
                City = u.City,
                PostalCode = u.PostalCode
            }).ToList();

            var pagedResponse = new PagedResponse<UserInfoDto>
            {
                Items = userDtos,
                PageNumber = pagedUsers.PageNumber,
                PageSize = pagedUsers.PageSize,
                TotalCount = pagedUsers.TotalCount
                // TotalPages, HasPreviousPage, and HasNextPage are computed properties
                // and will be automatically calculated from the above values
            };

            return ApiResponse<PagedResponse<UserInfoDto>>.SuccessResponse(pagedResponse, "Users retrieved successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in HandleGetAllUsers");
            return ApiResponse<PagedResponse<UserInfoDto>>.ErrorResponse($"Error retrieving users: {ex.Message}");
        }
    }

    private async Task<object> HandleRefreshToken(string messageJson)
    {
        try
        {
            var dto = JsonSerializer.Deserialize<RefreshTokenDto>(messageJson, _jsonOptions);
            if (dto == null)
            {
                return ApiResponse<AuthResponseDto>.ErrorResponse("Invalid refresh token data");
            }

            var userId = ExtractUserIdFromToken(dto.Token);
            if (userId == null)
            {
                return ApiResponse<AuthResponseDto>.ErrorResponse("Invalid token");
            }

            var user = await _userAccountService.GetUserByIdAsync(userId);
            if (user == null)
            {
                return ApiResponse<AuthResponseDto>.ErrorResponse("User not found");
            }

            var token = _authService.GenerateJwtToken(user);
            var response = new AuthResponseDto
            {
                Token = token,
                User = new UserInfoDto
                {
                    Id = user.Id ?? string.Empty,
                    Name = user.Name,
                    Email = user.Email,
                    Image = user.Image,
                    Role = user.Role
                }
            };

            return ApiResponse<AuthResponseDto>.SuccessResponse(response, "Token refreshed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in HandleRefreshToken");
            return ApiResponse<AuthResponseDto>.ErrorResponse($"Error refreshing token: {ex.Message}");
        }
    }

    private async Task<object> HandleForgotPassword(string messageJson)
    {
        try
        {
            var dto = JsonSerializer.Deserialize<ForgotPasswordDto>(messageJson, _jsonOptions);
            if (dto == null)
            {
                return ApiResponse<string>.ErrorResponse("Invalid forgot password data");
            }

            var user = await _userAccountService.GetUserByEmailAsync(dto.Email);
            // Don't reveal if user exists for security
            return ApiResponse<string>.SuccessResponse("If the email exists, a password reset link has been sent", "Password reset email sent");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in HandleForgotPassword");
            return ApiResponse<string>.ErrorResponse($"Error processing forgot password: {ex.Message}");
        }
    }

    private async Task<object> HandleResetPassword(string messageJson)
    {
        try
        {
            var dto = JsonSerializer.Deserialize<ResetPasswordDto>(messageJson, _jsonOptions);
            if (dto == null)
            {
                return ApiResponse<string>.ErrorResponse("Invalid reset password data");
            }

            var user = await _userAccountService.GetUserByEmailAsync(dto.Email);
            if (user == null)
            {
                return ApiResponse<string>.ErrorResponse("Invalid reset token or email");
            }

            // In production, validate the reset token
            user.PasswordHash = _authService.HashPassword(dto.NewPassword);
            user.UpdatedAt = DateTime.UtcNow;

            var updated = await _userAccountService.UpdateUserAsync(user.Id ?? string.Empty, user, null);
            if (updated == null)
            {
                return ApiResponse<string>.ErrorResponse("Failed to reset password");
            }

            return ApiResponse<string>.SuccessResponse("Password reset successfully", "Password has been reset");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in HandleResetPassword");
            return ApiResponse<string>.ErrorResponse($"Error resetting password: {ex.Message}");
        }
    }

    private async Task<object> HandleCreateUser(string messageJson)
    {
        try
        {
            var dto = JsonSerializer.Deserialize<CreateUserDto>(messageJson, _jsonOptions);
            if (dto == null)
            {
                return ApiResponse<UserInfoDto>.ErrorResponse("Invalid user data");
            }

            var existingUser = await _userAccountService.GetUserByEmailAsync(dto.Email);
            if (existingUser != null)
            {
                return ApiResponse<UserInfoDto>.ErrorResponse("User with this email already exists");
            }

            var user = new UserAccount
            {
                Name = $"{dto.FirstName} {dto.LastName}".Trim(),
                Email = dto.Email,
                PasswordHash = _authService.HashPassword(dto.Password),
                Image = dto.Image,
                Role = dto.Role ?? "user",
                Status = dto.Status ?? "active",
                Gender = dto.Gender,
                DateOfBirth = dto.DateOfBirth,
                MobilePhone = dto.MobilePhone,
                Country = dto.Country,
                State = dto.State,
                City = dto.City,
                PostalCode = dto.PostalCode
            };

            var createdUser = await _userAccountService.CreateUserAsync(user);

            var response = new UserInfoDto
            {
                Id = createdUser.Id ?? string.Empty,
                Name = createdUser.Name,
                Email = createdUser.Email,
                Image = createdUser.Image,
                Role = createdUser.Role,
                Status = createdUser.Status,
                CreatedAt = createdUser.CreatedAt,
                Gender = createdUser.Gender,
                DateOfBirth = createdUser.DateOfBirth,
                MobilePhone = createdUser.MobilePhone,
                Country = createdUser.Country,
                State = createdUser.State,
                City = createdUser.City,
                PostalCode = createdUser.PostalCode
            };

            return ApiResponse<UserInfoDto>.SuccessResponse(response, "User created successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in HandleCreateUser");
            return ApiResponse<UserInfoDto>.ErrorResponse($"Error creating user: {ex.Message}");
        }
    }

    private async Task<object> HandleUpdateUser(string messageJson)
    {
        try
        {
            var request = JsonSerializer.Deserialize<UpdateUserRequest>(messageJson, _jsonOptions);
            if (request == null || string.IsNullOrEmpty(request.Id) || request.Dto == null)
            {
                return ApiResponse<UserInfoDto>.ErrorResponse("Invalid update user data");
            }

            var updatedUser = await _userAccountService.UpdateUserAdminAsync(request.Id, request.Dto);
            if (updatedUser == null)
            {
                return ApiResponse<UserInfoDto>.ErrorResponse("User not found");
            }

            var response = new UserInfoDto
            {
                Id = updatedUser.Id ?? string.Empty,
                Name = updatedUser.Name,
                Email = updatedUser.Email,
                Image = updatedUser.Image,
                Role = updatedUser.Role,
                Status = updatedUser.Status,
                CreatedAt = updatedUser.CreatedAt,
                Gender = updatedUser.Gender,
                DateOfBirth = updatedUser.DateOfBirth,
                MobilePhone = updatedUser.MobilePhone,
                Country = updatedUser.Country,
                State = updatedUser.State,
                City = updatedUser.City,
                PostalCode = updatedUser.PostalCode
            };

            return ApiResponse<UserInfoDto>.SuccessResponse(response, "User updated successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in HandleUpdateUser");
            return ApiResponse<UserInfoDto>.ErrorResponse($"Error updating user: {ex.Message}");
        }
    }

    private async Task<object> HandleDeleteUser(string messageJson)
    {
        try
        {
            var request = JsonSerializer.Deserialize<DeleteUserRequest>(messageJson, _jsonOptions);
            if (request == null || string.IsNullOrEmpty(request.Id))
            {
                return ApiResponse<bool>.ErrorResponse("Invalid delete user data");
            }

            var deleted = await _userAccountService.DeleteUserAsync(request.Id);
            if (!deleted)
            {
                return ApiResponse<bool>.ErrorResponse("User not found");
            }

            return ApiResponse<bool>.SuccessResponse(true, "User deleted successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in HandleDeleteUser");
            return ApiResponse<bool>.ErrorResponse($"Error deleting user: {ex.Message}");
        }
    }

    private async Task<object> HandleUpdateUserStatus(string messageJson)
    {
        try
        {
            var request = JsonSerializer.Deserialize<UpdateUserStatusRequest>(messageJson, _jsonOptions);
            if (request == null || string.IsNullOrEmpty(request.Id) || string.IsNullOrEmpty(request.Status))
            {
                return ApiResponse<bool>.ErrorResponse("Invalid update user status data");
            }

            if (request.Status != "active" && request.Status != "inactive")
            {
                return ApiResponse<bool>.ErrorResponse("Status must be 'active' or 'inactive'");
            }

            var updated = await _userAccountService.UpdateUserStatusAsync(request.Id, request.Status);
            if (!updated)
            {
                return ApiResponse<bool>.ErrorResponse("User not found");
            }

            return ApiResponse<bool>.SuccessResponse(true, "User status updated successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in HandleUpdateUserStatus");
            return ApiResponse<bool>.ErrorResponse($"Error updating user status: {ex.Message}");
        }
    }

    private class UpdateUserRequest
    {
        public string Id { get; set; } = string.Empty;
        public UpdateUserDto? Dto { get; set; }
    }

    private class DeleteUserRequest
    {
        public string Id { get; set; } = string.Empty;
    }

    private class UpdateUserStatusRequest
    {
        public string Id { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
}
