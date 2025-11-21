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
            // Ensure user is in the chat group before sending typing indicator
            var groupName = $"Chat_{chatId}";
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            
            var user = await _userService.GetUserByIdAsync(userId);
            if (user != null)
            {
                Console.WriteLine($"[SendTyping] Sending typing indicator - ChatId: {chatId}, UserId: {userId}, Username: {user.Username}, IsTyping: {isTyping}, Group: {groupName}, ConnectionId: {Context.ConnectionId}");
                
                // Send to all in the chat group except the sender
                await Clients.GroupExcept(groupName, Context.ConnectionId)
                    .SendAsync("UserTyping", chatId, userId, user.Username, isTyping);
                
                Console.WriteLine($"[SendTyping] Event sent successfully");
            }
            else
            {
                Console.WriteLine($"[SendTyping] ERROR: User not found for userId: {userId}");
            }
        }
        else
        {
            Console.WriteLine($"[SendTyping] ERROR: UserId is null or empty");
        }
    }

    public async Task SendGroupTyping(string groupId, bool isTyping)
    {
        var userId = GetUserId();
        if (!string.IsNullOrEmpty(userId))
        {
            // Ensure user is in the group before sending typing indicator
            var groupName = $"Group_{groupId}";
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            
            var user = await _userService.GetUserByIdAsync(userId);
            if (user != null)
            {
                Console.WriteLine($"[SendGroupTyping] Sending typing indicator - GroupId: {groupId}, UserId: {userId}, Username: {user.Username}, IsTyping: {isTyping}, Group: {groupName}, ConnectionId: {Context.ConnectionId}");
                
                await Clients.GroupExcept(groupName, Context.ConnectionId)
                    .SendAsync("UserTypingGroup", groupId, userId, user.Username, isTyping);
                
                Console.WriteLine($"[SendGroupTyping] Event sent successfully");
            }
            else
            {
                Console.WriteLine($"[SendGroupTyping] ERROR: User not found for userId: {userId}");
            }
        }
        else
        {
            Console.WriteLine($"[SendGroupTyping] ERROR: UserId is null or empty");
        }
    }

    public static string? GetConnectionId(string userId)
    {
        return _userConnections.TryGetValue(userId, out var connectionId) ? connectionId : null;
    }

    private string? GetUserId()
    {
        // Debug: Check if user is authenticated
        if (Context.User == null)
        {
            Console.WriteLine($"[GetUserId] ERROR: Context.User is null. ConnectionId: {Context.ConnectionId}");
            Console.WriteLine($"[GetUserId] Request Path: {Context.GetHttpContext()?.Request?.Path}");
            Console.WriteLine($"[GetUserId] Query String: {Context.GetHttpContext()?.Request?.QueryString}");
            
            // Try to get token from query string as fallback
            var queryString = Context.GetHttpContext()?.Request?.Query;
            var token = queryString?["access_token"].ToString();
            if (!string.IsNullOrEmpty(token))
            {
                Console.WriteLine($"[GetUserId] Token found in query string, but user not authenticated");
            }
            return null;
        }

        // Check if user has claims
        if (!Context.User.Claims.Any())
        {
            Console.WriteLine($"[GetUserId] ERROR: Context.User has no claims. ConnectionId: {Context.ConnectionId}");
            return null;
        }

        // Try to get user ID from claims
        var userId = Context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            Console.WriteLine($"[GetUserId] ERROR: ClaimTypes.NameIdentifier not found. ConnectionId: {Context.ConnectionId}");
            Console.WriteLine($"[GetUserId] Available claims:");
            foreach (var claim in Context.User.Claims)
            {
                Console.WriteLine($"[GetUserId]   - {claim.Type}: {claim.Value}");
            }
        }
        else
        {
            Console.WriteLine($"[GetUserId] UserId found: {userId}");
        }

        return userId;
    }

    private async Task UpdateUserOnlineStatus(string userId, bool isOnline)
    {
        await _userService.UpdateOnlineStatusAsync(userId, isOnline);
    }
}
