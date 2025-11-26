using Gateway.DTOs;
using Shared.Common;
using Shared.Constants;
using Shared.Services;

namespace Gateway.Services;

public class UserAccountGatewayService : IUserAccountGatewayService
{
    private readonly IRabbitMQService _rabbitMQService;
    private readonly ILogger<UserAccountGatewayService> _logger;

    public UserAccountGatewayService(
        IRabbitMQService rabbitMQService,
        ILogger<UserAccountGatewayService> logger)
    {
        _rabbitMQService = rabbitMQService;
        _logger = logger;
    }

    public async Task<ApiResponse<AuthResponseDto>> SignUpAsync(SignUpDto dto)
    {
        try
        {
            var response = await _rabbitMQService.SendMessageAsync<ApiResponse<AuthResponseDto>>(
                RabbitMQConstants.UserAccountServiceQueue,
                RabbitMQConstants.UserAccount.SignUp,
                dto);
            return response ?? ApiResponse<AuthResponseDto>.ErrorResponse("Failed to sign up");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling UserAccountService for signup");
            return ApiResponse<AuthResponseDto>.ErrorResponse("An error occurred during signup");
        }
    }

    public async Task<ApiResponse<AuthResponseDto>> SignInAsync(SignInDto dto)
    {
        try
        {
            var response = await _rabbitMQService.SendMessageAsync<ApiResponse<AuthResponseDto>>(
                RabbitMQConstants.UserAccountServiceQueue,
                RabbitMQConstants.UserAccount.SignIn,
                dto);
            return response ?? ApiResponse<AuthResponseDto>.ErrorResponse("Failed to sign in");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling UserAccountService for signin");
            return ApiResponse<AuthResponseDto>.ErrorResponse("An error occurred during signin");
        }
    }

    public async Task<ApiResponse<UserInfoDto>> UpdateProfileAsync(string token, UpdateProfileDto dto)
    {
        try
        {
            var message = new { Token = token, Dto = dto };
            var response = await _rabbitMQService.SendMessageAsync<ApiResponse<UserInfoDto>>(
                RabbitMQConstants.UserAccountServiceQueue,
                RabbitMQConstants.UserAccount.UpdateProfile,
                message);
            return response ?? ApiResponse<UserInfoDto>.ErrorResponse("Failed to update profile");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling UserAccountService to update profile");
            return ApiResponse<UserInfoDto>.ErrorResponse("An error occurred while updating profile");
        }
    }

    public async Task<ApiResponse<UserInfoDto>> GetCurrentUserAsync(string token)
    {
        try
        {
            var message = new { Token = token };
            var response = await _rabbitMQService.SendMessageAsync<ApiResponse<UserInfoDto>>(
                RabbitMQConstants.UserAccountServiceQueue,
                RabbitMQConstants.UserAccount.GetCurrentUser,
                message);
            return response ?? ApiResponse<UserInfoDto>.ErrorResponse("Failed to get current user");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling UserAccountService to get current user");
            return ApiResponse<UserInfoDto>.ErrorResponse("An error occurred while retrieving user");
        }
    }
}

