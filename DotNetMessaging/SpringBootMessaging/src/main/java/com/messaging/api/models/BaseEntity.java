package com.messaging.api.models;

import lombok.Data;
import org.springframework.data.annotation.CreatedDate;
import org.springframework.data.annotation.Id;
import org.springframework.data.annotation.LastModifiedDate;
import org.springframework.data.mongodb.core.mapping.Document;

import java.time.LocalDateTime;

@Data
public abstract class BaseEntity {
    @Id
    private String id;
    
    @CreatedDate
    private LocalDateTime createdAt = LocalDateTime.now();
    
    @LastModifiedDate
    private LocalDateTime updatedAt;
}

