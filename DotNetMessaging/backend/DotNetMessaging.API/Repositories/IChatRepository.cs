using DotNetMessaging.API.Models;

namespace DotNetMessaging.API.Repositories;

public interface IChatRepository : IRepository<Chat>
{
    Task<Chat?> GetChatBetweenUsersAsync(string user1Id, string user2Id);
    Task<IEnumerable<Chat>> GetUserChatsAsync(string userId);
}

