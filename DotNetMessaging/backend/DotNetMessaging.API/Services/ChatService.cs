using DotNetMessaging.API.DTOs;
using DotNetMessaging.API.Models;
using DotNetMessaging.API.Repositories;

namespace DotNetMessaging.API.Services;

public interface IChatService
{
    Task<List<ChatDto>> GetUserChatsAsync(string userId);
    Task<ChatDto?> GetOrCreateChatAsync(string userId, string otherUserId);
}

public class ChatService : IChatService
{
    private readonly IChatRepository _chatRepository;
    private readonly IUserRepository _userRepository;
    private readonly IMessageRepository _messageRepository;
    private readonly IMessageReactionRepository _reactionRepository;
    private readonly IMessageReadRepository _messageReadRepository;

    public ChatService(
        IChatRepository chatRepository,
        IUserRepository userRepository,
        IMessageRepository messageRepository,
        IMessageReactionRepository reactionRepository,
        IMessageReadRepository messageReadRepository)
    {
        _chatRepository = chatRepository;
        _userRepository = userRepository;
        _messageRepository = messageRepository;
        _reactionRepository = reactionRepository;
        _messageReadRepository = messageReadRepository;
    }

    public async Task<List<ChatDto>> GetUserChatsAsync(string userId)
    {
        var chats = await _chatRepository.GetUserChatsAsync(userId);
        var chatDtos = new List<ChatDto>();

        foreach (var chat in chats)
        {
            var otherUserId = chat.User1Id == userId ? chat.User2Id : chat.User1Id;
            var otherUser = await _userRepository.GetByIdAsync(otherUserId);
            if (otherUser == null) continue;

            var messages = await _messageRepository.GetChatMessagesAsync(chat.Id, 0, 1);
            var lastMessage = messages.FirstOrDefault();
            var readMessageIds = await _messageReadRepository.GetReadMessageIdsAsync(chat.Id, userId);
            var unreadCount = await _messageRepository.GetUnreadCountAsync(chat.Id, userId, readMessageIds);

            var lastMessageDto = lastMessage != null ? await MapMessageToDtoAsync(lastMessage) : null;

            chatDtos.Add(new ChatDto
            {
                Id = chat.Id,
                OtherUser = MapUserToDto(otherUser),
                LastMessage = lastMessageDto,
                UnreadCount = unreadCount,
                CreatedAt = chat.CreatedAt
            });
        }

        return chatDtos;
    }

    public async Task<ChatDto?> GetOrCreateChatAsync(string userId, string otherUserId)
    {
        if (userId == otherUserId)
            return null;

        var chat = await _chatRepository.GetChatBetweenUsersAsync(userId, otherUserId);

        if (chat == null)
        {
            chat = new Chat
            {
                User1Id = userId,
                User2Id = otherUserId,
                CreatedAt = DateTime.UtcNow
            };
            await _chatRepository.CreateAsync(chat);
        }

        var otherUser = await _userRepository.GetByIdAsync(otherUserId);
        if (otherUser == null) return null;

        return new ChatDto
        {
            Id = chat.Id,
            OtherUser = MapUserToDto(otherUser),
            CreatedAt = chat.CreatedAt
        };
    }

    private UserDto MapUserToDto(User user)
    {
        return new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            ProfilePictureUrl = user.ProfilePictureUrl,
            Status = user.Status,
            IsOnline = user.IsOnline,
            LastSeen = user.LastSeen
        };
    }

    private async Task<MessageDto> MapMessageToDtoAsync(Message message)
    {
        var sender = await _userRepository.GetByIdAsync(message.SenderId);
        var reactions = await GetMessageReactionsAsync(message.Id);

        return new MessageDto
        {
            Id = message.Id,
            ChatId = message.ChatId,
            GroupId = message.GroupId,
            SenderId = message.SenderId,
            SenderName = sender?.Username ?? "",
            SenderProfilePicture = sender?.ProfilePictureUrl,
            Content = message.Content,
            Type = message.Type.ToString(),
            MediaUrl = message.MediaUrl,
            MediaType = message.MediaType,
            MediaFileName = message.MediaFileName,
            MediaSize = message.MediaSize,
            ReplyToMessageId = message.ReplyToMessageId,
            Reactions = reactions,
            CreatedAt = message.CreatedAt
        };
    }

    private async Task<List<ReactionDto>> GetMessageReactionsAsync(string messageId)
    {
        var reactions = await _reactionRepository.GetMessageReactionsAsync(messageId);
        var reactionDtos = new List<ReactionDto>();

        foreach (var reaction in reactions)
        {
            var user = await _userRepository.GetByIdAsync(reaction.UserId);
            reactionDtos.Add(new ReactionDto
            {
                Id = reaction.Id,
                UserId = reaction.UserId,
                UserName = user?.Username ?? "",
                Emoji = reaction.Emoji
            });
        }

        return reactionDtos;
    }
}

