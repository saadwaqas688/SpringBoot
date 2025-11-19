package com.messaging.api.models;

import lombok.Data;
import lombok.EqualsAndHashCode;
import org.springframework.data.mongodb.core.mapping.Document;

import java.time.LocalDateTime;

@Data
@EqualsAndHashCode(callSuper = true)
@Document(collection = "groupMembers")
public class GroupMember extends BaseEntity {
    private String groupId;
    
    private String userId;
    
    private GroupRole role = GroupRole.Member;
    
    private LocalDateTime joinedAt = LocalDateTime.now();
}

