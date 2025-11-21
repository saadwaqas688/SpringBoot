using UserService.Models;
using UserService.Services;
using Shared.Common;
using Shared.Constants;
using Shared.DTOs;
using System.Text.Json;

namespace UserService.Services;

public class UserMessageHandler
{
    private readonly IUserService _userService;
    private readonly ILogger<UserMessageHandler> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public UserMessageHandler(IUserService userService, ILogger<UserMessageHandler> logger)
    {
        _userService = userService;
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
                RabbitMQConstants.User.HealthCheck => HandleHealthCheck(),
                RabbitMQConstants.User.GetAll => await HandleGetAll(),
                RabbitMQConstants.User.GetById => await HandleGetById(messageJson),
                RabbitMQConstants.User.GetByEmail => await HandleGetByEmail(messageJson),
                RabbitMQConstants.User.Create => await HandleCreate(messageJson),
                RabbitMQConstants.User.Update => await HandleUpdate(messageJson),
                RabbitMQConstants.User.Delete => await HandleDelete(messageJson),
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
        return ApiResponse<string>.SuccessResponse("UserService is healthy", "Health check successful");
    }

    private async Task<object> HandleGetAll()
    {
        try
        {
            var users = await _userService.GetAllUsersAsync();

            // Convert to Shared.Models.User
            var sharedUsers = users.Select(u => new Shared.Models.User
            {
                Id = u.Id,
                Username = u.Username,
                Email = u.Email,
                FirstName = u.FirstName,
                LastName = u.LastName,
                CreatedAt = u.CreatedAt,
                UpdatedAt = u.UpdatedAt
            }).ToList();

            return ApiResponse<List<Shared.Models.User>>.SuccessResponse(sharedUsers, "Users retrieved successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in HandleGetAll");
            return ApiResponse<List<Shared.Models.User>>.ErrorResponse($"Error retrieving users: {ex.Message}");
        }
    }

    private async Task<object> HandleGetById(string messageJson)
    {
        try
        {
            var request = JsonSerializer.Deserialize<GetByIdRequest>(messageJson, _jsonOptions);
            
            if (request?.Id == null || !Guid.TryParse(request.Id.ToString(), out var id))
            {
                return ApiResponse<Shared.Models.User>.ErrorResponse("Invalid ID provided");
            }

            var user = await _userService.GetUserByIdAsync(id);
            
            if (user == null)
            {
                return ApiResponse<Shared.Models.User>.ErrorResponse("User not found");
            }

            var sharedUser = new Shared.Models.User
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            };

            return ApiResponse<Shared.Models.User>.SuccessResponse(sharedUser, "User retrieved successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in HandleGetById");
            return ApiResponse<Shared.Models.User>.ErrorResponse($"Error retrieving user: {ex.Message}");
        }
    }

    private async Task<object> HandleGetByEmail(string messageJson)
    {
        try
        {
            var request = JsonSerializer.Deserialize<GetByEmailRequest>(messageJson, _jsonOptions);
            
            if (string.IsNullOrEmpty(request?.Email))
            {
                return ApiResponse<Shared.Models.User>.ErrorResponse("Invalid email provided");
            }

            var user = await _userService.GetUserByEmailAsync(request.Email);
            
            if (user == null)
            {
                return ApiResponse<Shared.Models.User>.ErrorResponse("User not found");
            }

            var sharedUser = new Shared.Models.User
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            };

            return ApiResponse<Shared.Models.User>.SuccessResponse(sharedUser, "User retrieved successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in HandleGetByEmail");
            return ApiResponse<Shared.Models.User>.ErrorResponse($"Error retrieving user: {ex.Message}");
        }
    }

    private async Task<object> HandleCreate(string messageJson)
    {
        try
        {
            var dto = JsonSerializer.Deserialize<CreateUserDto>(messageJson, _jsonOptions);
            
            if (dto == null)
            {
                return ApiResponse<Shared.Models.User>.ErrorResponse("Invalid create user data");
            }

            var user = new User
            {
                Username = dto.Username,
                Email = dto.Email,
                FirstName = dto.FirstName,
                LastName = dto.LastName
            };

            var createdUser = await _userService.CreateUserAsync(user);

            var sharedUser = new Shared.Models.User
            {
                Id = createdUser.Id,
                Username = createdUser.Username,
                Email = createdUser.Email,
                FirstName = createdUser.FirstName,
                LastName = createdUser.LastName,
                CreatedAt = createdUser.CreatedAt,
                UpdatedAt = createdUser.UpdatedAt
            };

            return ApiResponse<Shared.Models.User>.SuccessResponse(sharedUser, "User created successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in HandleCreate");
            return ApiResponse<Shared.Models.User>.ErrorResponse($"Error creating user: {ex.Message}");
        }
    }

    private async Task<object> HandleUpdate(string messageJson)
    {
        try
        {
            var request = JsonSerializer.Deserialize<UpdateUserRequest>(messageJson, _jsonOptions);
            
            if (request?.Id == null || !Guid.TryParse(request.Id.ToString(), out var id))
            {
                return ApiResponse<Shared.Models.User>.ErrorResponse("Invalid ID provided");
            }

            var existingUser = await _userService.GetUserByIdAsync(id);
            if (existingUser == null)
            {
                return ApiResponse<Shared.Models.User>.ErrorResponse("User not found");
            }

            // Update fields from DTO
            if (request.Dto != null)
            {
                if (request.Dto.Username != null) existingUser.Username = request.Dto.Username;
                if (request.Dto.Email != null) existingUser.Email = request.Dto.Email;
                if (request.Dto.FirstName != null) existingUser.FirstName = request.Dto.FirstName;
                if (request.Dto.LastName != null) existingUser.LastName = request.Dto.LastName;
            }

            var updatedUser = await _userService.UpdateUserAsync(id, existingUser);
            
            if (updatedUser == null)
            {
                return ApiResponse<Shared.Models.User>.ErrorResponse("Failed to update user");
            }

            var sharedUser = new Shared.Models.User
            {
                Id = updatedUser.Id,
                Username = updatedUser.Username,
                Email = updatedUser.Email,
                FirstName = updatedUser.FirstName,
                LastName = updatedUser.LastName,
                CreatedAt = updatedUser.CreatedAt,
                UpdatedAt = updatedUser.UpdatedAt
            };

            return ApiResponse<Shared.Models.User>.SuccessResponse(sharedUser, "User updated successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in HandleUpdate");
            return ApiResponse<Shared.Models.User>.ErrorResponse($"Error updating user: {ex.Message}");
        }
    }

    private async Task<object> HandleDelete(string messageJson)
    {
        try
        {
            var request = JsonSerializer.Deserialize<GetByIdRequest>(messageJson, _jsonOptions);
            
            if (request?.Id == null || !Guid.TryParse(request.Id.ToString(), out var id))
            {
                return ApiResponse<bool>.ErrorResponse("Invalid ID provided");
            }

            var deleted = await _userService.DeleteUserAsync(id);
            
            if (!deleted)
            {
                return ApiResponse<bool>.ErrorResponse("User not found");
            }

            return ApiResponse<bool>.SuccessResponse(true, "User deleted successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in HandleDelete");
            return ApiResponse<bool>.ErrorResponse($"Error deleting user: {ex.Message}");
        }
    }

    // Helper classes for deserializing requests
    private class GetByIdRequest
    {
        public object? Id { get; set; }
    }

    private class GetByEmailRequest
    {
        public string? Email { get; set; }
    }

    private class UpdateUserRequest
    {
        public object? Id { get; set; }
        public UpdateUserDto? Dto { get; set; }
    }
}
