package com.messaging.api.repositories;

import com.messaging.api.models.MessageReaction;
import org.springframework.data.mongodb.repository.MongoRepository;
import org.springframework.data.mongodb.repository.Query;
import org.springframework.stereotype.Repository;

import java.util.List;
import java.util.Optional;

@Repository
public interface MessageReactionRepository extends MongoRepository<MessageReaction, String> {
    List<MessageReaction> findByMessageId(String messageId);
    
    Optional<MessageReaction> findByMessageIdAndUserId(String messageId, String userId);
    
    @Query("{ messageId: ?0, userId: ?1, emoji: ?2 }")
    Optional<MessageReaction> findByMessageIdAndUserIdAndEmoji(String messageId, String userId, String emoji);
    
    void deleteByMessageIdAndUserIdAndEmoji(String messageId, String userId, String emoji);
}

