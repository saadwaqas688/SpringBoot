using DotNetMessaging.API.Models;

namespace DotNetMessaging.API.Repositories;

public interface IMessageRepository : IRepository<Message>
{
    Task<IEnumerable<Message>> GetChatMessagesAsync(string chatId, int skip = 0, int take = 50);
    Task<IEnumerable<Message>> GetGroupMessagesAsync(string groupId, int skip = 0, int take = 50);
    Task<long> GetUnreadCountAsync(string chatId, string userId, HashSet<string>? readMessageIds = null);
    Task<long> GetGroupUnreadCountAsync(string groupId, string userId, HashSet<string>? readMessageIds = null);
}

