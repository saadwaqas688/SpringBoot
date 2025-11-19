package com.messaging.api.dtos;

import lombok.AllArgsConstructor;
import lombok.Data;
import lombok.NoArgsConstructor;

import java.time.LocalDateTime;
import java.util.List;

public class MessageDTOs {
    
    @Data
    @NoArgsConstructor
    @AllArgsConstructor
    public static class MessageDto {
        private String id;
        private String chatId;
        private String groupId;
        private String senderId;
        private String senderName;
        private String senderProfilePicture;
        private String content;
        private String type;
        private String mediaUrl;
        private String mediaType;
        private String mediaFileName;
        private Long mediaSize;
        private String replyToMessageId;
        private MessageDto replyToMessage;
        private List<ReactionDto> reactions;
        private LocalDateTime createdAt;
    }
    
    @Data
    @NoArgsConstructor
    @AllArgsConstructor
    public static class CreateMessageRequest {
        private String chatId;
        private String groupId;
        private String content;
        private String type = "Text";
        private String replyToMessageId;
        private String mediaUrl;
        private String mediaType;
        private String mediaFileName;
        private Long mediaSize;
    }
    
    @Data
    @NoArgsConstructor
    @AllArgsConstructor
    public static class ReactionDto {
        private String id;
        private String userId;
        private String userName;
        private String emoji;
    }
    
    @Data
    @NoArgsConstructor
    @AllArgsConstructor
    public static class AddReactionRequest {
        private String messageId;
        private String emoji;
    }
}

