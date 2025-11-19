package com.messaging.api.websocket;

import com.messaging.api.models.Message;
import com.messaging.api.services.UserService;
import lombok.RequiredArgsConstructor;
import org.springframework.messaging.handler.annotation.MessageMapping;
import org.springframework.messaging.handler.annotation.Payload;
import org.springframework.messaging.simp.SimpMessagingTemplate;
import org.springframework.stereotype.Controller;

import java.security.Principal;

@Controller
@RequiredArgsConstructor
public class ChatWebSocketHandler {
    
    private final SimpMessagingTemplate messagingTemplate;
    private final UserService userService;
    
    @MessageMapping("/chat/join")
    public void joinChat(@Payload String chatId, Principal principal) {
        String userId = principal.getName();
        messagingTemplate.convertAndSend("/topic/chat/" + chatId + "/joined", userId);
    }
    
    @MessageMapping("/chat/leave")
    public void leaveChat(@Payload String chatId, Principal principal) {
        String userId = principal.getName();
        messagingTemplate.convertAndSend("/topic/chat/" + chatId + "/left", userId);
    }
    
    @MessageMapping("/group/join")
    public void joinGroup(@Payload String groupId, Principal principal) {
        String userId = principal.getName();
        messagingTemplate.convertAndSend("/topic/group/" + groupId + "/joined", userId);
    }
    
    @MessageMapping("/group/leave")
    public void leaveGroup(@Payload String groupId, Principal principal) {
        String userId = principal.getName();
        messagingTemplate.convertAndSend("/topic/group/" + groupId + "/left", userId);
    }
    
    @MessageMapping("/chat/typing")
    public void sendTyping(@Payload TypingMessage message, Principal principal) {
        String userId = principal.getName();
        var user = userService.getUserById(userId);
        String username = user != null ? user.getUsername() : "User";
        
        messagingTemplate.convertAndSend("/topic/chat/" + message.getChatId() + "/typing", 
                new TypingNotification(userId, username, message.isTyping()));
    }
    
    @MessageMapping("/group/typing")
    public void sendGroupTyping(@Payload TypingMessage message, Principal principal) {
        String userId = principal.getName();
        var user = userService.getUserById(userId);
        String username = user != null ? user.getUsername() : "User";
        
        messagingTemplate.convertAndSend("/topic/group/" + message.getGroupId() + "/typing",
                new TypingNotification(userId, username, message.isTyping()));
    }
    
    // Helper method to broadcast new message
    public void broadcastNewMessage(Message message) {
        if (message.getChatId() != null) {
            messagingTemplate.convertAndSend("/topic/chat/" + message.getChatId() + "/message", message);
        } else if (message.getGroupId() != null) {
            messagingTemplate.convertAndSend("/topic/group/" + message.getGroupId() + "/message", message);
        }
    }
    
    // Helper method to broadcast user online/offline status
    public void broadcastUserStatus(String userId, boolean isOnline) {
        messagingTemplate.convertAndSend("/topic/user/" + userId + "/status", isOnline);
    }
    
    @lombok.Data
    @lombok.AllArgsConstructor
    @lombok.NoArgsConstructor
    public static class TypingMessage {
        private String chatId;
        private String groupId;
        private boolean typing;
    }
    
    @lombok.Data
    @lombok.AllArgsConstructor
    @lombok.NoArgsConstructor
    public static class TypingNotification {
        private String userId;
        private String userName;
        private boolean typing;
    }
}

