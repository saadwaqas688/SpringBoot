using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using DotNetMessaging.API.Services;

namespace DotNetMessaging.API.Hubs;

public class ChatHub : Hub
{
    private readonly IUserService _userService;
    private static readonly Dictionary<string, string> _userConnections = new();

    public ChatHub(IUserService userService)
    {
        _userService = userService;
    }

    public override async Task OnConnectedAsync()
    {
        var userId = GetUserId();
        if (!string.IsNullOrEmpty(userId))
        {
            _userConnections[userId] = Context.ConnectionId;
            await UpdateUserOnlineStatus(userId, true);
            await Clients.All.SendAsync("UserOnline", userId);
        }
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = GetUserId();
        if (!string.IsNullOrEmpty(userId))
        {
            _userConnections.Remove(userId);
            await UpdateUserOnlineStatus(userId, false);
            await Clients.All.SendAsync("UserOffline", userId);
        }
        await base.OnDisconnectedAsync(exception);
    }

    public async Task JoinChat(string chatId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"Chat_{chatId}");
    }

    public async Task LeaveChat(string chatId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Chat_{chatId}");
    }

    public async Task JoinGroup(string groupId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"Group_{groupId}");
    }

    public async Task LeaveGroup(string groupId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Group_{groupId}");
    }

    public async Task SendTyping(string chatId, bool isTyping)
    {
        var userId = GetUserId();
        if (!string.IsNullOrEmpty(userId))
        {
            var user = await _userService.GetUserByIdAsync(userId);
            await Clients.GroupExcept($"Chat_{chatId}", Context.ConnectionId)
                .SendAsync("UserTyping", chatId, userId, user?.Username ?? "User", isTyping);
        }
    }

    public async Task SendGroupTyping(string groupId, bool isTyping)
    {
        var userId = GetUserId();
        if (!string.IsNullOrEmpty(userId))
        {
            var user = await _userService.GetUserByIdAsync(userId);
            await Clients.GroupExcept($"Group_{groupId}", Context.ConnectionId)
                .SendAsync("UserTypingGroup", groupId, userId, user?.Username ?? "User", isTyping);
        }
    }

    public static string? GetConnectionId(string userId)
    {
        return _userConnections.TryGetValue(userId, out var connectionId) ? connectionId : null;
    }

    private string? GetUserId()
    {
        return Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    }

    private async Task UpdateUserOnlineStatus(string userId, bool isOnline)
    {
        await _userService.UpdateOnlineStatusAsync(userId, isOnline);
    }
}
