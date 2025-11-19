package com.messaging.api.repositories;

import com.messaging.api.models.Chat;
import org.springframework.data.mongodb.repository.MongoRepository;
import org.springframework.data.mongodb.repository.Query;
import org.springframework.stereotype.Repository;

import java.util.List;
import java.util.Optional;

@Repository
public interface ChatRepository extends MongoRepository<Chat, String> {
    @Query("{ $or: [ { user1Id: ?0, user2Id: ?1 }, { user1Id: ?1, user2Id: ?0 } ] }")
    Optional<Chat> findByUserIds(String user1Id, String user2Id);
    
    @Query("{ $or: [ { user1Id: ?0 }, { user2Id: ?0 } ] }")
    List<Chat> findByUserId(String userId);
}

