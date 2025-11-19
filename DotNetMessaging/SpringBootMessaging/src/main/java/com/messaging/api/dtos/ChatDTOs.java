package com.messaging.api.dtos;

import lombok.AllArgsConstructor;
import lombok.Data;
import lombok.NoArgsConstructor;

import java.time.LocalDateTime;
import java.util.List;

public class ChatDTOs {
    
    @Data
    @NoArgsConstructor
    @AllArgsConstructor
    public static class ChatDto {
        private String id;
        private AuthDTOs.UserDto otherUser;
        private MessageDto lastMessage;
        private long unreadCount;
        private LocalDateTime createdAt;
    }
    
    @Data
    @NoArgsConstructor
    @AllArgsConstructor
    public static class GroupDto {
        private String id;
        private String name;
        private String description;
        private String profilePictureUrl;
        private List<GroupMemberDto> members;
        private MessageDto lastMessage;
        private long unreadCount;
        private LocalDateTime createdAt;
    }
    
    @Data
    @NoArgsConstructor
    @AllArgsConstructor
    public static class GroupMemberDto {
        private String id;
        private AuthDTOs.UserDto user;
        private String role;
        private LocalDateTime joinedAt;
    }
    
    @Data
    @NoArgsConstructor
    @AllArgsConstructor
    public static class CreateGroupRequest {
        private String name;
        private String description;
        private List<String> memberIds;
    }
}

