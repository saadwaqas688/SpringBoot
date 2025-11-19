package com.messaging.api.repositories;

import com.messaging.api.models.GroupMember;
import com.messaging.api.models.GroupRole;
import org.springframework.data.mongodb.repository.MongoRepository;
import org.springframework.data.mongodb.repository.Query;
import org.springframework.stereotype.Repository;

import java.util.List;
import java.util.Optional;

@Repository
public interface GroupMemberRepository extends MongoRepository<GroupMember, String> {
    List<GroupMember> findByGroupId(String groupId);
    
    Optional<GroupMember> findByGroupIdAndUserId(String groupId, String userId);
    
    boolean existsByGroupIdAndUserId(String groupId, String userId);
    
    @Query("{ groupId: ?0, userId: ?1 }")
    boolean isMember(String groupId, String userId);
    
    List<GroupMember> findByUserId(String userId);
    
    @Query("{ groupId: ?0, role: ?1 }")
    List<GroupMember> findByGroupIdAndRole(String groupId, GroupRole role);
}

