namespace DotNetMessaging.API.DTOs;

public class MessageDto
{
    public string Id { get; set; } = string.Empty;
    public string? ChatId { get; set; }
    public string? GroupId { get; set; }
    public string SenderId { get; set; } = string.Empty;
    public string SenderName { get; set; } = string.Empty;
    public string? SenderProfilePicture { get; set; }
    public string Content { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string? MediaUrl { get; set; }
    public string? MediaType { get; set; }
    public string? MediaFileName { get; set; }
    public long? MediaSize { get; set; }
    public string? ReplyToMessageId { get; set; }
    public MessageDto? ReplyToMessage { get; set; }
    public List<ReactionDto> Reactions { get; set; } = new();
    public DateTime CreatedAt { get; set; }
}

public class CreateMessageRequest
{
    public string? ChatId { get; set; }
    public string? GroupId { get; set; }
    public string Content { get; set; } = string.Empty;
    public string Type { get; set; } = "Text";
    public string? ReplyToMessageId { get; set; }
    public string? MediaUrl { get; set; }
    public string? MediaType { get; set; }
    public string? MediaFileName { get; set; }
    public long? MediaSize { get; set; }
}

public class ReactionDto
{
    public string Id { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Emoji { get; set; } = string.Empty;
}

public class AddReactionRequest
{
    public string MessageId { get; set; } = string.Empty;
    public string Emoji { get; set; } = string.Empty;
}


