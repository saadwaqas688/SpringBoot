using DotNetMessaging.API.DTOs;
using DotNetMessaging.API.Models;
using DotNetMessaging.API.Repositories;

namespace DotNetMessaging.API.Services;

public interface IMessageService
{
    Task<List<MessageDto>> GetChatMessagesAsync(string chatId, string userId, int skip = 0, int take = 50);
    Task<List<MessageDto>> GetGroupMessagesAsync(string groupId, string userId, int skip = 0, int take = 50);
    Task<MessageDto> CreateMessageAsync(CreateMessageRequest request, string userId);
    Task<MessageDto?> GetMessageByIdAsync(string messageId);
    Task<bool> AddReactionAsync(string messageId, string userId, string emoji);
    Task<bool> DeleteMessageAsync(string messageId, string userId);
    Task MarkChatAsReadAsync(string chatId, string userId);
    Task MarkGroupAsReadAsync(string groupId, string userId);
}

public class MessageService : IMessageService
{
    private readonly IMessageRepository _messageRepository;
    private readonly IUserRepository _userRepository;
    private readonly IChatRepository _chatRepository;
    private readonly IGroupRepository _groupRepository;
    private readonly IGroupMemberRepository _groupMemberRepository;
    private readonly IMessageReactionRepository _reactionRepository;
    private readonly IMessageReadRepository _messageReadRepository;

    public MessageService(
        IMessageRepository messageRepository,
        IUserRepository userRepository,
        IChatRepository chatRepository,
        IGroupRepository groupRepository,
        IGroupMemberRepository groupMemberRepository,
        IMessageReactionRepository reactionRepository,
        IMessageReadRepository messageReadRepository)
    {
        _messageRepository = messageRepository;
        _userRepository = userRepository;
        _chatRepository = chatRepository;
        _groupRepository = groupRepository;
        _groupMemberRepository = groupMemberRepository;
        _reactionRepository = reactionRepository;
        _messageReadRepository = messageReadRepository;
    }

    public async Task<List<MessageDto>> GetChatMessagesAsync(string chatId, string userId, int skip = 0, int take = 50)
    {
        var chat = await _chatRepository.GetByIdAsync(chatId);
        if (chat == null || (chat.User1Id != userId && chat.User2Id != userId))
            throw new UnauthorizedAccessException("You don't have access to this chat");

        var messages = await _messageRepository.GetChatMessagesAsync(chatId, skip, take);
        var messageDtos = new List<MessageDto>();

        foreach (var message in messages.Reverse())
        {
            messageDtos.Add(await MapMessageToDtoAsync(message));
        }

        return messageDtos;
    }

    public async Task<List<MessageDto>> GetGroupMessagesAsync(string groupId, string userId, int skip = 0, int take = 50)
    {
        var isMember = await _groupMemberRepository.IsMemberAsync(groupId, userId);
        if (!isMember)
            throw new UnauthorizedAccessException("You are not a member of this group");

        var messages = await _messageRepository.GetGroupMessagesAsync(groupId, skip, take);
        var messageDtos = new List<MessageDto>();

        foreach (var message in messages.Reverse())
        {
            messageDtos.Add(await MapMessageToDtoAsync(message));
        }

        return messageDtos;
    }

    public async Task<MessageDto> CreateMessageAsync(CreateMessageRequest request, string userId)
    {
        if (request.ChatId != null)
        {
            var chat = await _chatRepository.GetByIdAsync(request.ChatId);
            if (chat == null || (chat.User1Id != userId && chat.User2Id != userId))
                throw new UnauthorizedAccessException("You don't have access to this chat");
        }
        else if (request.GroupId != null)
        {
            var isMember = await _groupMemberRepository.IsMemberAsync(request.GroupId, userId);
            if (!isMember)
                throw new UnauthorizedAccessException("You are not a member of this group");
        }
        else
        {
            throw new ArgumentException("Either ChatId or GroupId must be provided");
        }

        var message = new Message
        {
            ChatId = request.ChatId,
            GroupId = request.GroupId,
            SenderId = userId,
            Content = request.Content,
            Type = Enum.Parse<MessageType>(request.Type),
            ReplyToMessageId = request.ReplyToMessageId,
            MediaUrl = request.MediaUrl,
            MediaType = request.MediaType,
            MediaFileName = request.MediaFileName,
            MediaSize = request.MediaSize,
            CreatedAt = DateTime.UtcNow
        };

        await _messageRepository.CreateAsync(message);

        // Update last message time
        if (request.ChatId != null)
        {
            var chat = await _chatRepository.GetByIdAsync(request.ChatId);
            if (chat != null)
            {
                chat.LastMessageAt = DateTime.UtcNow;
                await _chatRepository.UpdateAsync(chat.Id, chat);
            }
        }
        else if (request.GroupId != null)
        {
            var group = await _groupRepository.GetByIdAsync(request.GroupId);
            if (group != null)
            {
                group.LastMessageAt = DateTime.UtcNow;
                await _groupRepository.UpdateAsync(group.Id, group);
            }
        }

        return await MapMessageToDtoAsync(message);
    }

    public async Task<MessageDto?> GetMessageByIdAsync(string messageId)
    {
        var message = await _messageRepository.GetByIdAsync(messageId);
        if (message == null) return null;
        return await MapMessageToDtoAsync(message);
    }

    public async Task<bool> AddReactionAsync(string messageId, string userId, string emoji)
    {
        var message = await _messageRepository.GetByIdAsync(messageId);
        if (message == null) return false;

        var existingReaction = await _reactionRepository.GetReactionAsync(messageId, userId, emoji);

        if (existingReaction != null)
        {
            await _reactionRepository.DeleteAsync(existingReaction.Id);
        }
        else
        {
            var reaction = new MessageReaction
            {
                MessageId = messageId,
                UserId = userId,
                Emoji = emoji,
                CreatedAt = DateTime.UtcNow
            };
            await _reactionRepository.CreateAsync(reaction);
        }

        return true;
    }

    public async Task<bool> DeleteMessageAsync(string messageId, string userId)
    {
        var message = await _messageRepository.GetByIdAsync(messageId);
        if (message == null || message.SenderId != userId)
            return false;

        message.IsDeleted = true;
        await _messageRepository.UpdateAsync(messageId, message);
        return true;
    }

    public async Task MarkChatAsReadAsync(string chatId, string userId)
    {
        var chat = await _chatRepository.GetByIdAsync(chatId);
        if (chat == null || (chat.User1Id != userId && chat.User2Id != userId))
            throw new UnauthorizedAccessException("You don't have access to this chat");

        await _messageReadRepository.MarkMessagesAsReadAsync(chatId, userId);
    }

    public async Task MarkGroupAsReadAsync(string groupId, string userId)
    {
        var isMember = await _groupMemberRepository.IsMemberAsync(groupId, userId);
        if (!isMember)
            throw new UnauthorizedAccessException("You are not a member of this group");

        await _messageReadRepository.MarkGroupMessagesAsReadAsync(groupId, userId);
    }

    private async Task<MessageDto> MapMessageToDtoAsync(Message message)
    {
        var sender = await _userRepository.GetByIdAsync(message.SenderId);
        var reactions = await _reactionRepository.GetMessageReactionsAsync(message.Id);
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

        MessageDto? replyToMessageDto = null;
        if (!string.IsNullOrEmpty(message.ReplyToMessageId))
        {
            var replyToMessage = await _messageRepository.GetByIdAsync(message.ReplyToMessageId);
            if (replyToMessage != null)
            {
                replyToMessageDto = await MapMessageToDtoAsync(replyToMessage);
            }
        }

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
            ReplyToMessage = replyToMessageDto,
            Reactions = reactionDtos,
            CreatedAt = message.CreatedAt
        };
    }
}

