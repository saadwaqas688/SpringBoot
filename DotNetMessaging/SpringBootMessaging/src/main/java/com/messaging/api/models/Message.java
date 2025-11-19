package com.messaging.api.models;

import lombok.Data;
import lombok.EqualsAndHashCode;
import org.springframework.data.mongodb.core.mapping.Document;

@Data
@EqualsAndHashCode(callSuper = true)
@Document(collection = "messages")
public class Message extends BaseEntity {
    private String chatId;
    
    private String groupId;
    
    private String senderId;
    
    private String content;
    
    private MessageType type = MessageType.Text;
    
    private String mediaUrl;
    
    private String mediaType;
    
    private String mediaFileName;
    
    private Long mediaSize;
    
    private String replyToMessageId;
    
    private boolean isDeleted = false;
}

