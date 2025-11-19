package com.messaging.api.models;

import lombok.Data;
import lombok.EqualsAndHashCode;
import org.springframework.data.mongodb.core.mapping.Document;

@Data
@EqualsAndHashCode(callSuper = true)
@Document(collection = "contacts")
public class Contact extends BaseEntity {
    private String userId;
    
    private String contactUserId;
    
    private String displayName;
}

