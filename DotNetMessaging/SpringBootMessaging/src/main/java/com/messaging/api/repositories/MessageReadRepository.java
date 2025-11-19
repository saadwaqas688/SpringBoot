package com.messaging.api.repositories;

import com.messaging.api.models.MessageRead;
import org.springframework.data.mongodb.repository.MongoRepository;
import org.springframework.data.mongodb.repository.Query;
import org.springframework.stereotype.Repository;

import java.util.List;
import java.util.Set;
import java.util.stream.Collectors;

@Repository
public interface MessageReadRepository extends MongoRepository<MessageRead, String> {
    List<MessageRead> findByUserIdAndMessageIdIn(String userId, List<String> messageIds);
    
    @Query("{ userId: ?0, messageId: { $in: ?1 } }")
    List<MessageRead> findByUserIdAndMessageIds(String userId, List<String> messageIds);
    
    boolean existsByMessageIdAndUserId(String messageId, String userId);
    
    default Set<String> getReadMessageIds(String userId, List<String> messageIds) {
        return findByUserIdAndMessageIds(userId, messageIds)
                .stream()
                .map(MessageRead::getMessageId)
                .collect(Collectors.toSet());
    }
}

