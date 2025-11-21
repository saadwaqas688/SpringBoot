using Shared.Models;
using Shared.DTOs;
using Shared.Common;
using Shared.Constants;
using Shared.Services;

namespace Gateway.Services;

public class UserGatewayService : IUserGatewayService
{
    private readonly IRabbitMQService _rabbitMQService;
    private readonly ILogger<UserGatewayService> _logger;

    public UserGatewayService(
        IRabbitMQService rabbitMQService,
        ILogger<UserGatewayService> logger)
    {
        _rabbitMQService = rabbitMQService;
        _logger = logger;
    }

    public async Task<ApiResponse<List<User>>> GetAllUsersAsync()
    {
        try
        {
            var message = new { };
            var response = await _rabbitMQService.SendMessageAsync<ApiResponse<List<User>>>(
                RabbitMQConstants.UserServiceQueue,
                RabbitMQConstants.User.GetAll,
                message);
            return response ?? ApiResponse<List<User>>.ErrorResponse("Failed to retrieve users");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling UserService");
            return ApiResponse<List<User>>.ErrorResponse("An error occurred while retrieving users");
        }
    }

    public async Task<ApiResponse<User>> GetUserByIdAsync(Guid id)
    {
        try
        {
            var message = new { Id = id };
            var response = await _rabbitMQService.SendMessageAsync<ApiResponse<User>>(
                RabbitMQConstants.UserServiceQueue,
                RabbitMQConstants.User.GetById,
                message);
            return response ?? ApiResponse<User>.ErrorResponse("Failed to retrieve user");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling UserService for user {UserId}", id);
            return ApiResponse<User>.ErrorResponse("An error occurred while retrieving user");
        }
    }

    public async Task<ApiResponse<User>> GetUserByEmailAsync(string email)
    {
        try
        {
            var message = new { Email = email };
            var response = await _rabbitMQService.SendMessageAsync<ApiResponse<User>>(
                RabbitMQConstants.UserServiceQueue,
                RabbitMQConstants.User.GetByEmail,
                message);
            return response ?? ApiResponse<User>.ErrorResponse("Failed to retrieve user");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling UserService for email {Email}", email);
            return ApiResponse<User>.ErrorResponse("An error occurred while retrieving user");
        }
    }

    public async Task<ApiResponse<User>> CreateUserAsync(CreateUserDto dto)
    {
        try
        {
            var response = await _rabbitMQService.SendMessageAsync<ApiResponse<User>>(
                RabbitMQConstants.UserServiceQueue,
                RabbitMQConstants.User.Create,
                dto);
            return response ?? ApiResponse<User>.ErrorResponse("Failed to create user");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling UserService to create user");
            return ApiResponse<User>.ErrorResponse("An error occurred while creating user");
        }
    }

    public async Task<ApiResponse<User>> UpdateUserAsync(Guid id, UpdateUserDto dto)
    {
        try
        {
            var message = new { Id = id, Dto = dto };
            var response = await _rabbitMQService.SendMessageAsync<ApiResponse<User>>(
                RabbitMQConstants.UserServiceQueue,
                RabbitMQConstants.User.Update,
                message);
            return response ?? ApiResponse<User>.ErrorResponse("Failed to update user");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling UserService to update user {UserId}", id);
            return ApiResponse<User>.ErrorResponse("An error occurred while updating user");
        }
    }

    public async Task<ApiResponse<bool>> DeleteUserAsync(Guid id)
    {
        try
        {
            var message = new { Id = id };
            var response = await _rabbitMQService.SendMessageAsync<ApiResponse<bool>>(
                RabbitMQConstants.UserServiceQueue,
                RabbitMQConstants.User.Delete,
                message);
            return response ?? ApiResponse<bool>.ErrorResponse("Failed to delete user");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling UserService to delete user {UserId}", id);
            return ApiResponse<bool>.ErrorResponse("An error occurred while deleting user");
        }
    }
}

