namespace DotNetMessaging.API.Constants;

/// <summary>
/// SignalR event names used for real-time communication between server and clients.
/// These constants ensure consistency across the application and prevent typos.
/// </summary>
public static class SignalREvents
{
    // ============================================================================
    // SERVER → CLIENT EVENTS (Sent by server to notify clients)
    // ============================================================================
    
    /// <summary>
    /// Event sent when a new message is created in a one-on-one chat.
    /// Payload: MessageDto
    /// </summary>
    public const string NewMessage = "NewMessage";
    
    /// <summary>
    /// Event sent when a new message is created in a group chat.
    /// Payload: MessageDto
    /// </summary>
    public const string NewGroupMessage = "NewGroupMessage";
    
    /// <summary>
    /// Event sent when a user comes online.
    /// Payload: string (userId)
    /// </summary>
    public const string UserOnline = "UserOnline";
    
    /// <summary>
    /// Event sent when a user goes offline.
    /// Payload: string (userId)
    /// </summary>
    public const string UserOffline = "UserOffline";
    
    /// <summary>
    /// Event sent when a user starts/stops typing in a one-on-one chat.
    /// Payload: (chatId: string, userId: string, username: string, isTyping: bool)
    /// </summary>
    public const string UserTyping = "UserTyping";
    
    /// <summary>
    /// Event sent when a user starts/stops typing in a group chat.
    /// Payload: (groupId: string, userId: string, username: string, isTyping: bool)
    /// </summary>
    public const string UserTypingGroup = "UserTypingGroup";
    
    /// <summary>
    /// Event sent when a message reaction is added, updated, or removed.
    /// Payload: MessageDto (updated message with reactions)
    /// </summary>
    public const string MessageReactionUpdated = "MessageReactionUpdated";
    
    /// <summary>
    /// Event sent when a message is deleted.
    /// Payload: string (messageId)
    /// </summary>
    public const string MessageDeleted = "MessageDeleted";
    
    /// <summary>
    /// Event sent when a new group is created.
    /// Payload: GroupDto
    /// </summary>
    public const string GroupCreated = "GroupCreated";
    
    /// <summary>
    /// Event sent when a user is removed from a group.
    /// Payload: (groupId: string, removedUserId: string)
    /// </summary>
    public const string GroupMemberRemoved = "GroupMemberRemoved";
    
    // ============================================================================
    // CLIENT → SERVER METHODS (Invoked by clients)
    // ============================================================================
    
    /// <summary>
    /// Method invoked by client to join a one-on-one chat room.
    /// Parameters: (chatId: string)
    /// </summary>
    public const string JoinChat = "JoinChat";
    
    /// <summary>
    /// Method invoked by client to leave a one-on-one chat room.
    /// Parameters: (chatId: string)
    /// </summary>
    public const string LeaveChat = "LeaveChat";
    
    /// <summary>
    /// Method invoked by client to join a group chat room.
    /// Parameters: (groupId: string)
    /// </summary>
    public const string JoinGroup = "JoinGroup";
    
    /// <summary>
    /// Method invoked by client to leave a group chat room.
    /// Parameters: (groupId: string)
    /// </summary>
    public const string LeaveGroup = "LeaveGroup";
    
    /// <summary>
    /// Method invoked by client to send typing indicator in a one-on-one chat.
    /// Parameters: (chatId: string, isTyping: bool)
    /// </summary>
    public const string SendTyping = "SendTyping";
    
    /// <summary>
    /// Method invoked by client to send typing indicator in a group chat.
    /// Parameters: (groupId: string, isTyping: bool)
    /// </summary>
    public const string SendGroupTyping = "SendGroupTyping";
    
    // ============================================================================
    // GROUP NAME PREFIXES (Used for SignalR groups)
    // ============================================================================
    
    /// <summary>
    /// Prefix for chat group names.
    /// Format: "Chat_{chatId}"
    /// </summary>
    public const string ChatGroupPrefix = "Chat_";
    
    /// <summary>
    /// Prefix for group chat group names.
    /// Format: "Group_{groupId}"
    /// </summary>
    public const string GroupChatPrefix = "Group_";
    
    // ============================================================================
    // HELPER METHODS
    // ============================================================================
    
    /// <summary>
    /// Gets the SignalR group name for a chat.
    /// </summary>
    /// <param name="chatId">The chat ID</param>
    /// <returns>Group name in format "Chat_{chatId}"</returns>
    public static string GetChatGroupName(string chatId) => $"{ChatGroupPrefix}{chatId}";
    
    /// <summary>
    /// Gets the SignalR group name for a group chat.
    /// </summary>
    /// <param name="groupId">The group ID</param>
    /// <returns>Group name in format "Group_{groupId}"</returns>
    public static string GetGroupChatName(string groupId) => $"{GroupChatPrefix}{groupId}";
}

