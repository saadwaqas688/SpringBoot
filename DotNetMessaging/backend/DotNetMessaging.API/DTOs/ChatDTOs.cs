namespace DotNetMessaging.API.DTOs;

public class ChatDto
{
    public string Id { get; set; } = string.Empty;
    public UserDto OtherUser { get; set; } = null!;
    public MessageDto? LastMessage { get; set; }
    public long UnreadCount { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class GroupDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public List<GroupMemberDto> Members { get; set; } = new();
    public MessageDto? LastMessage { get; set; }
    public long UnreadCount { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class GroupMemberDto
{
    public string Id { get; set; } = string.Empty;
    public UserDto User { get; set; } = null!;
    public string Role { get; set; } = string.Empty;
    public DateTime JoinedAt { get; set; }
}

public class CreateGroupRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public List<string> MemberIds { get; set; } = new();
}


