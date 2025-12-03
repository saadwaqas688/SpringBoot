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
            _logger.LogInformation("Sending signup request for email: {Email}", dto.Email);
            
            var response = await _rabbitMQService.SendMessageAsync<ApiResponse<AuthResponseDto>>(
                RabbitMQConstants.UserAccountServiceQueue,
                RabbitMQConstants.UserAccount.SignUp,
                dto);
            
            if (response == null)
            {
                _logger.LogWarning("Null response from UserAccountService for signup. Email: {Email}. User may have been created but response was lost.", dto.Email);
                return ApiResponse<AuthResponseDto>.ErrorResponse("Failed to sign up. The user may have been created, but we didn't receive a confirmation. Please try signing in.");
            }
            
            _logger.LogInformation("Signup successful for email: {Email}", dto.Email);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling UserAccountService for signup. Email: {Email}", dto.Email);
            return ApiResponse<AuthResponseDto>.ErrorResponse($"An error occurred during signup: {ex.Message}");
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
            
            if (response == null)
            {
                _logger.LogWarning("Null response from UserAccountService for signin. Email: {Email}", dto.Email);
                return ApiResponse<AuthResponseDto>.ErrorResponse("Failed to sign in. Please check if UserAccountService is running and RabbitMQ is connected.");
            }
            
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling UserAccountService for signin. Email: {Email}, Exception Type: {ExceptionType}", 
                dto.Email, ex.GetType().Name);
            
            // Check if it's a RabbitMQ connection issue
            var errorMessage = ex.Message.Contains("BrokerUnreachable") || 
                              ex.Message.Contains("RabbitMQ") || 
                              ex.Message.Contains("Connection") ||
                              ex.GetType().Name.Contains("Broker") ||
                              ex.GetType().Name.Contains("RabbitMQ")
                ? "Service temporarily unavailable. RabbitMQ connection failed. Please check if RabbitMQ is running on localhost:5672."
                : $"An error occurred during signin: {ex.Message}";
            
            return ApiResponse<AuthResponseDto>.ErrorResponse(errorMessage);
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

    public async Task<ApiResponse<PagedResponse<UserInfoDto>>> GetAllUsersAsync(int page, int pageSize, string? searchTerm = null)
    {
        try
        {
            var message = new { Page = page, PageSize = pageSize, SearchTerm = searchTerm };
            var response = await _rabbitMQService.SendMessageAsync<ApiResponse<PagedResponse<UserInfoDto>>>(
                RabbitMQConstants.UserAccountServiceQueue,
                RabbitMQConstants.UserAccount.GetAllUsers,
                message);
            return response ?? ApiResponse<PagedResponse<UserInfoDto>>.ErrorResponse("Failed to get users");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling UserAccountService to get all users");
            return ApiResponse<PagedResponse<UserInfoDto>>.ErrorResponse("An error occurred while retrieving users");
        }
    }

    public async Task<ApiResponse<AuthResponseDto>> RefreshTokenAsync(RefreshTokenDto dto)
    {
        try
        {
            var response = await _rabbitMQService.SendMessageAsync<ApiResponse<AuthResponseDto>>(
                RabbitMQConstants.UserAccountServiceQueue,
                RabbitMQConstants.UserAccount.RefreshToken,
                dto);
            return response ?? ApiResponse<AuthResponseDto>.ErrorResponse("Failed to refresh token");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling UserAccountService for refresh token");
            return ApiResponse<AuthResponseDto>.ErrorResponse("An error occurred during token refresh");
        }
    }

    public async Task<ApiResponse<string>> ForgotPasswordAsync(ForgotPasswordDto dto)
    {
        try
        {
            var response = await _rabbitMQService.SendMessageAsync<ApiResponse<string>>(
                RabbitMQConstants.UserAccountServiceQueue,
                RabbitMQConstants.UserAccount.ForgotPassword,
                dto);
            return response ?? ApiResponse<string>.ErrorResponse("Failed to process forgot password");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling UserAccountService for forgot password");
            return ApiResponse<string>.ErrorResponse("An error occurred during forgot password");
        }
    }

    public async Task<ApiResponse<string>> ResetPasswordAsync(ResetPasswordDto dto)
    {
        try
        {
            var response = await _rabbitMQService.SendMessageAsync<ApiResponse<string>>(
                RabbitMQConstants.UserAccountServiceQueue,
                RabbitMQConstants.UserAccount.ResetPassword,
                dto);
            return response ?? ApiResponse<string>.ErrorResponse("Failed to reset password");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling UserAccountService for reset password");
            return ApiResponse<string>.ErrorResponse("An error occurred during password reset");
        }
    }
}

