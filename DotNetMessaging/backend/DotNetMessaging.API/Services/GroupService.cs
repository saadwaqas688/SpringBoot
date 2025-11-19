using DotNetMessaging.API.DTOs;
using DotNetMessaging.API.Models;
using DotNetMessaging.API.Repositories;

namespace DotNetMessaging.API.Services;

public interface IGroupService
{
    Task<List<GroupDto>> GetUserGroupsAsync(string userId);
    Task<GroupDto> CreateGroupAsync(CreateGroupRequest request, string userId);
    Task<bool> AddMembersAsync(string groupId, List<string> memberIds, string userId);
    Task<bool> RemoveMemberAsync(string groupId, string memberId, string userId);
    Task<bool> UpdateMemberRoleAsync(string groupId, string memberId, GroupRole role, string userId);
}

public class GroupService : IGroupService
{
    private readonly IGroupRepository _groupRepository;
    private readonly IGroupMemberRepository _groupMemberRepository;
    private readonly IUserRepository _userRepository;
    private readonly IMessageRepository _messageRepository;
    private readonly IMessageReadRepository _messageReadRepository;

    public GroupService(
        IGroupRepository groupRepository,
        IGroupMemberRepository groupMemberRepository,
        IUserRepository userRepository,
        IMessageRepository messageRepository,
        IMessageReadRepository messageReadRepository)
    {
        _groupRepository = groupRepository;
        _groupMemberRepository = groupMemberRepository;
        _userRepository = userRepository;
        _messageRepository = messageRepository;
        _messageReadRepository = messageReadRepository;
    }

    public async Task<List<GroupDto>> GetUserGroupsAsync(string userId)
    {
        var groups = await _groupRepository.GetUserGroupsAsync(userId);
        var groupDtos = new List<GroupDto>();

        foreach (var group in groups)
        {
            var members = await _groupMemberRepository.GetGroupMembersAsync(group.Id);
            var memberDtos = new List<GroupMemberDto>();

            foreach (var member in members)
            {
                var user = await _userRepository.GetByIdAsync(member.UserId);
                if (user != null)
                {
                    memberDtos.Add(new GroupMemberDto
                    {
                        Id = member.Id,
                        User = MapUserToDto(user),
                        Role = member.Role.ToString(),
                        JoinedAt = member.JoinedAt
                    });
                }
            }

            var messages = await _messageRepository.GetGroupMessagesAsync(group.Id, 0, 1);
            var lastMessage = messages.FirstOrDefault();
            var readMessageIds = await _messageReadRepository.GetReadGroupMessageIdsAsync(group.Id, userId);
            var unreadCount = await _messageRepository.GetGroupUnreadCountAsync(group.Id, userId, readMessageIds);

            var lastMessageDto = lastMessage != null ? await MapMessageToDtoAsync(lastMessage) : null;

            groupDtos.Add(new GroupDto
            {
                Id = group.Id,
                Name = group.Name,
                Description = group.Description,
                ProfilePictureUrl = group.ProfilePictureUrl,
                Members = memberDtos,
                LastMessage = lastMessageDto,
                UnreadCount = unreadCount,
                CreatedAt = group.CreatedAt
            });
        }

        return groupDtos;
    }

    public async Task<GroupDto> CreateGroupAsync(CreateGroupRequest request, string userId)
    {
        var group = new Group
        {
            Name = request.Name,
            Description = request.Description,
            CreatedById = userId,
            CreatedAt = DateTime.UtcNow
        };

        await _groupRepository.CreateAsync(group);

        // Add creator as admin
        var creatorMember = new GroupMember
        {
            GroupId = group.Id,
            UserId = userId,
            Role = GroupRole.Admin,
            JoinedAt = DateTime.UtcNow
        };
        await _groupMemberRepository.CreateAsync(creatorMember);

        // Add other members
        foreach (var memberId in request.MemberIds.Where(id => id != userId))
        {
            var member = new GroupMember
            {
                GroupId = group.Id,
                UserId = memberId,
                Role = GroupRole.Member,
                JoinedAt = DateTime.UtcNow
            };
            await _groupMemberRepository.CreateAsync(member);
        }

        return await GetGroupDtoAsync(group);
    }

    public async Task<bool> AddMembersAsync(string groupId, List<string> memberIds, string userId)
    {
        var isAdmin = await _groupMemberRepository.IsAdminAsync(groupId, userId);
        if (!isAdmin)
            return false;

        foreach (var memberId in memberIds)
        {
            var exists = await _groupMemberRepository.IsMemberAsync(groupId, memberId);
            if (!exists)
            {
                var member = new GroupMember
                {
                    GroupId = groupId,
                    UserId = memberId,
                    Role = GroupRole.Member,
                    JoinedAt = DateTime.UtcNow
                };
                await _groupMemberRepository.CreateAsync(member);
            }
        }

        return true;
    }

    public async Task<bool> RemoveMemberAsync(string groupId, string memberId, string userId)
    {
        var isAdmin = await _groupMemberRepository.IsAdminAsync(groupId, userId);
        if (!isAdmin && userId != memberId)
            return false;

        var member = await _groupMemberRepository.GetMemberAsync(groupId, memberId);
        if (member != null)
        {
            await _groupMemberRepository.DeleteAsync(member.Id);
            return true;
        }

        return false;
    }

    public async Task<bool> UpdateMemberRoleAsync(string groupId, string memberId, GroupRole role, string userId)
    {
        var isAdmin = await _groupMemberRepository.IsAdminAsync(groupId, userId);
        if (!isAdmin)
            return false;

        var member = await _groupMemberRepository.GetMemberAsync(groupId, memberId);
        if (member != null)
        {
            member.Role = role;
            await _groupMemberRepository.UpdateAsync(member.Id, member);
            return true;
        }

        return false;
    }

    private async Task<GroupDto> GetGroupDtoAsync(Group group)
    {
        var members = await _groupMemberRepository.GetGroupMembersAsync(group.Id);
        var memberDtos = new List<GroupMemberDto>();

        foreach (var member in members)
        {
            var user = await _userRepository.GetByIdAsync(member.UserId);
            if (user != null)
            {
                memberDtos.Add(new GroupMemberDto
                {
                    Id = member.Id,
                    User = MapUserToDto(user),
                    Role = member.Role.ToString(),
                    JoinedAt = member.JoinedAt
                });
            }
        }

        return new GroupDto
        {
            Id = group.Id,
            Name = group.Name,
            Description = group.Description,
            ProfilePictureUrl = group.ProfilePictureUrl,
            Members = memberDtos,
            CreatedAt = group.CreatedAt
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
            CreatedAt = message.CreatedAt
        };
    }
}

