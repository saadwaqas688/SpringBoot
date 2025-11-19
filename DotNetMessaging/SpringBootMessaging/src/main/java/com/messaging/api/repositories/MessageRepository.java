package com.messaging.api.repositories;

import com.messaging.api.models.Message;
import org.springframework.data.domain.Page;
import org.springframework.data.domain.Pageable;
import org.springframework.data.mongodb.repository.MongoRepository;
import org.springframework.data.mongodb.repository.Query;
import org.springframework.stereotype.Repository;

import java.util.List;

@Repository
public interface MessageRepository extends MongoRepository<Message, String> {
    @Query("{ chatId: ?0, isDeleted: false }")
    Page<Message> findByChatIdOrderByCreatedAtDesc(String chatId, Pageable pageable);
    
    @Query("{ groupId: ?0, isDeleted: false }")
    Page<Message> findByGroupIdOrderByCreatedAtDesc(String groupId, Pageable pageable);
    
    @Query("{ chatId: ?0, senderId: { $ne: ?1 }, isDeleted: false }")
    long countUnreadByChatIdAndUserId(String chatId, String userId);
    
    @Query("{ groupId: ?0, senderId: { $ne: ?1 }, isDeleted: false }")
    long countUnreadByGroupIdAndUserId(String groupId, String userId);
    
    List<Message> findByChatIdAndSenderIdNotAndIsDeletedFalse(String chatId, String userId);
    
    List<Message> findByGroupIdAndSenderIdNotAndIsDeletedFalse(String groupId, String userId);
}

