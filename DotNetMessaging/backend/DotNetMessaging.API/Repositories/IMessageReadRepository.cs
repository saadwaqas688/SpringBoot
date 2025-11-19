using DotNetMessaging.API.Models;

namespace DotNetMessaging.API.Repositories;

public interface IMessageReadRepository : IRepository<MessageRead>
{
    Task MarkMessagesAsReadAsync(string chatId, string userId);
    Task MarkGroupMessagesAsReadAsync(string groupId, string userId);
    Task<bool> IsMessageReadAsync(string messageId, string userId);
    Task<HashSet<string>> GetReadMessageIdsAsync(string chatId, string userId);
    Task<HashSet<string>> GetReadGroupMessageIdsAsync(string groupId, string userId);
}

