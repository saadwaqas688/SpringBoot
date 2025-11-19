package com.messaging.api.models;

import lombok.Data;
import lombok.EqualsAndHashCode;
import org.springframework.data.mongodb.core.mapping.Document;

@Data
@EqualsAndHashCode(callSuper = true)
@Document(collection = "messageReactions")
public class MessageReaction extends BaseEntity {
    private String messageId;
    
    private String userId;
    
    private String emoji;
}

