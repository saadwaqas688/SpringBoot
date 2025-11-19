using DotNetMessaging.API.Models;

namespace DotNetMessaging.API.Repositories;

public interface IMessageReactionRepository : IRepository<MessageReaction>
{
    Task<IEnumerable<MessageReaction>> GetMessageReactionsAsync(string messageId);
    Task<MessageReaction?> GetReactionAsync(string messageId, string userId, string emoji);
}

