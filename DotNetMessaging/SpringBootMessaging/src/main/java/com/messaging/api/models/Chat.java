package com.messaging.api.models;

import lombok.Data;
import lombok.EqualsAndHashCode;
import org.springframework.data.mongodb.core.mapping.Document;

import java.time.LocalDateTime;

@Data
@EqualsAndHashCode(callSuper = true)
@Document(collection = "chats")
public class Chat extends BaseEntity {
    private String user1Id;
    
    private String user2Id;
    
    private LocalDateTime lastMessageAt;
}

