package com.messaging.api.models;

import lombok.Data;
import lombok.EqualsAndHashCode;
import org.springframework.data.mongodb.core.mapping.Document;

import java.time.LocalDateTime;

@Data
@EqualsAndHashCode(callSuper = true)
@Document(collection = "groups")
public class Group extends BaseEntity {
    private String name;
    
    private String description;
    
    private String profilePictureUrl;
    
    private String createdById;
    
    private LocalDateTime lastMessageAt;
}

