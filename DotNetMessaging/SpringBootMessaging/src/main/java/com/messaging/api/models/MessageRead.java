package com.messaging.api.models;

import lombok.Data;
import lombok.EqualsAndHashCode;
import org.springframework.data.mongodb.core.mapping.Document;

import java.time.LocalDateTime;

@Data
@EqualsAndHashCode(callSuper = true)
@Document(collection = "messageReads")
public class MessageRead extends BaseEntity {
    private String messageId;
    
    private String userId;
    
    private LocalDateTime readAt = LocalDateTime.now();
}

