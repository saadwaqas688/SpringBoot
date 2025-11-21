using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using DotNetMessaging.API.DTOs;
using DotNetMessaging.API.Services;
using DotNetMessaging.API.Models;
using DotNetMessaging.API.Hubs;
using DotNetMessaging.API.Constants;

namespace DotNetMessaging.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class GroupsController : ControllerBase
{
    private readonly IGroupService _groupService;
    private readonly IHubContext<ChatHub> _hubContext;

    public GroupsController(IGroupService groupService, IHubContext<ChatHub> hubContext)
    {
        _groupService = groupService;
        _hubContext = hubContext;
    }

    [HttpGet]
    public async Task<ActionResult<List<GroupDto>>> GetGroups()
    {
        var userId = GetCurrentUserId();
        var groups = await _groupService.GetUserGroupsAsync(userId);
        return Ok(groups);
    }

    [HttpPost]
    public async Task<ActionResult<GroupDto>> CreateGroup([FromBody] CreateGroupRequest request)
    {
        Console.WriteLine($"[CreateGroup] Request received - Name: {request.Name}, Description: {request.Description}, MemberIds: {string.Join(", ", request.MemberIds ?? new List<string>())}");
        
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            Console.WriteLine("[CreateGroup] ERROR: Group name is required");
            return BadRequest("Group name is required");
        }

        // Allow creating a group even without additional members (creator will be the only member)
        if (request.MemberIds == null)
        {
            request.MemberIds = new List<string>();
        }
        
        Console.WriteLine($"[CreateGroup] MemberIds count: {request.MemberIds.Count}");

        var userId = GetCurrentUserId();
        Console.WriteLine($"[CreateGroup] Current UserId: {userId}");
        
        try
        {
            var group = await _groupService.CreateGroupAsync(request, userId);
            Console.WriteLine($"[CreateGroup] Group created successfully - Id: {group.Id}, Name: {group.Name}");
            
            // Notify all members about the new group via SignalR
            var memberIds = group.Members.Select(m => m.User.Id).ToList();
            foreach (var memberId in memberIds)
            {
                var connectionId = ChatHub.GetConnectionId(memberId);
                if (!string.IsNullOrEmpty(connectionId))
                {
                    await _hubContext.Clients.Client(connectionId)
                        .SendAsync(SignalREvents.GroupCreated, group);
                    Console.WriteLine($"[CreateGroup] Notified member {memberId} about new group");
                }
            }
            
            return Ok(group);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[CreateGroup] ERROR: {ex.Message}");
            Console.WriteLine($"[CreateGroup] StackTrace: {ex.StackTrace}");
            return StatusCode(500, "An error occurred while creating the group");
        }
    }

    [HttpPost("{groupId}/members")]
    public async Task<ActionResult> AddMembers(string groupId, [FromBody] List<string> memberIds)
    {
        var userId = GetCurrentUserId();
        var success = await _groupService.AddMembersAsync(groupId, memberIds, userId);

        if (success)
            return Ok();
        
        return Forbid();
    }

    [HttpDelete("{groupId}/members/{memberId}")]
    public async Task<ActionResult> RemoveMember(string groupId, string memberId)
    {
        var userId = GetCurrentUserId();
        
        // Get all members before removal to notify them
        var allGroups = await _groupService.GetUserGroupsAsync(userId);
        var currentGroup = allGroups.FirstOrDefault(g => g.Id == groupId);
        var allMemberIds = currentGroup?.Members.Select(m => m.User.Id).ToList() ?? new List<string>();
        
        var success = await _groupService.RemoveMemberAsync(groupId, memberId, userId);

        if (success)
        {
            // Notify the removed user via SignalR
            var removedUserConnectionId = ChatHub.GetConnectionId(memberId);
            if (!string.IsNullOrEmpty(removedUserConnectionId))
            {
                await _hubContext.Clients.Client(removedUserConnectionId)
                    .SendAsync(SignalREvents.GroupMemberRemoved, groupId, memberId);
                Console.WriteLine($"[RemoveMember] Notified removed user {memberId} about removal from group {groupId}");
            }
            
            // Notify all members (including the one being removed) about the change
            // This allows remaining members to refresh their group list
            foreach (var memberIdToNotify in allMemberIds)
            {
                var connectionId = ChatHub.GetConnectionId(memberIdToNotify);
                if (!string.IsNullOrEmpty(connectionId))
                {
                    await _hubContext.Clients.Client(connectionId)
                        .SendAsync(SignalREvents.GroupMemberRemoved, groupId, memberId);
                }
            }
            
            return Ok();
        }
        
        return Forbid();
    }

    // ============================================================================
    // UPDATE MEMBER ROLE ENDPOINT
    // ============================================================================
    // HTTP Method: PUT
    // Route: PUT /api/groups/{groupId}/members/{memberId}/role
    // 
    // Purpose: Updates the role of a member in a group (e.g., promote Member to Admin)
    // 
    // Request Flow:
    // 1. Client sends: PUT /api/groups/123/members/456/role
    //    Body: "Admin" or "Member" (as JSON string)
    //    Headers: Authorization: Bearer <JWT_TOKEN>
    // 
    // 2. Middleware validates JWT token and sets HttpContext.User
    // 
    // 3. Controller extracts:
    //    - groupId from route: "123"
    //    - memberId from route: "456"
    //    - role from request body: "Admin" or "Member"
    //    - userId from JWT claims (via GetCurrentUserId())
    // 
    // 4. Validates role string can be parsed to GroupRole enum
    //    - Valid values: "Member", "Admin" (case-sensitive)
    //    - Invalid values return 400 Bad Request
    // 
    // 5. Service layer checks:
    //    - Current user is Admin of the group (authorization)
    //    - Member exists in the group
    //    - Updates member's role in database
    // 
    // 6. Returns:
    //    - 200 OK: Role updated successfully
    //    - 400 Bad Request: Invalid role value
    //    - 403 Forbid: User is not admin or member doesn't exist
    // ============================================================================
    [HttpPut("{groupId}/members/{memberId}/role")]
    public async Task<ActionResult> UpdateMemberRole(
        string groupId,           // Route parameter: ID of the group
        string memberId,          // Route parameter: ID of the member whose role to update
        [FromBody] string role)   // Request body: Role string ("Member" or "Admin")
    {
        // Extract current user's ID from JWT token claims
        // This is the user making the request (must be an admin to update roles)
        var userId = GetCurrentUserId();
        
        // Validate and parse the role string to GroupRole enum
        // Enum.TryParse attempts to convert the string to a GroupRole value
        // - Returns true if successful, false if invalid
        // - Valid values: "Member", "Admin" (must match enum names exactly)
        // - Invalid values like "member", "ADMIN", "Moderator" will fail
        if (!Enum.TryParse<GroupRole>(role, out var groupRole))
        {
            // Return 400 Bad Request if role string is invalid
            // Example: If client sends "Moderator" or "admin" (wrong case)
            return BadRequest("Invalid role");
        }

        // Call service layer to update the member's role
        // Service will:
        // 1. Verify current user (userId) is an Admin of the group
        // 2. Check if the member (memberId) exists in the group
        // 3. Update the member's role in the database
        // Returns: true if successful, false if unauthorized or member not found
        var success = await _groupService.UpdateMemberRoleAsync(groupId, memberId, groupRole, userId);

        // Return appropriate HTTP status code based on operation result
        if (success)
        {
            // 200 OK: Role updated successfully
            return Ok();
        }
        
        // 403 Forbidden: User is not admin OR member doesn't exist
        // This indicates authorization/permission failure, not authentication failure
        return Forbid();
    }

    // ============================================================================
    // GET CURRENT USER ID HELPER METHOD
    // ============================================================================
    // Purpose: Extracts the current authenticated user's ID from JWT token claims
    // 
    // How it works:
    // 1. After JWT authentication middleware validates the token, it populates
    //    HttpContext.User with claims from the token
    // 
    // 2. The JWT token contains a claim with type ClaimTypes.NameIdentifier
    //    (which is "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")
    //    This claim holds the user's ID
    // 
    // 3. User.FindFirst() searches for a claim with the specified type
    // 
    // 4. The ! (null-forgiving operator) tells compiler we're sure it's not null
    //    (because [Authorize] ensures user is authenticated)
    // 
    // 5. .Value extracts the string value of the claim (the user ID)
    // 
    // Example JWT payload:
    // {
    //   "sub": "user123",
    //   "nameid": "user123",  // This is mapped to NameIdentifier claim
    //   "exp": 1234567890
    // }
    // 
    // Returns: The user ID as a string (e.g., "user123")
    // ============================================================================
    private string GetCurrentUserId()
    {
        // User property is inherited from ControllerBase
        // It contains the ClaimsPrincipal with all JWT token claims
        // 
        // FindFirst() searches for a claim by its type
        // ClaimTypes.NameIdentifier is the standard claim type for user ID
        // 
        // The ! operator asserts this will not be null (safe because [Authorize] is on controller)
        // .Value gets the actual string value of the claim
        return User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
    }

   
}
