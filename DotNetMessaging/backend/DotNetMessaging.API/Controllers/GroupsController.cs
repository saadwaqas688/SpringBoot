using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using DotNetMessaging.API.DTOs;
using DotNetMessaging.API.Services;
using DotNetMessaging.API.Models;

namespace DotNetMessaging.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class GroupsController : ControllerBase
{
    private readonly IGroupService _groupService;

    public GroupsController(IGroupService groupService)
    {
        _groupService = groupService;
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
        var success = await _groupService.RemoveMemberAsync(groupId, memberId, userId);

        if (success)
            return Ok();
        
        return Forbid();
    }

    [HttpPut("{groupId}/members/{memberId}/role")]
    public async Task<ActionResult> UpdateMemberRole(string groupId, string memberId, [FromBody] string role)
    {
        var userId = GetCurrentUserId();
        
        if (!Enum.TryParse<GroupRole>(role, out var groupRole))
            return BadRequest("Invalid role");

        var success = await _groupService.UpdateMemberRoleAsync(groupId, memberId, groupRole, userId);

        if (success)
            return Ok();
        
        return Forbid();
    }

    private string GetCurrentUserId()
    {
        return User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
    }
}
