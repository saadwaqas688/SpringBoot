package com.messaging.api.repositories;

import com.messaging.api.models.Contact;
import org.springframework.data.mongodb.repository.MongoRepository;
import org.springframework.data.mongodb.repository.Query;
import org.springframework.stereotype.Repository;

import java.util.List;
import java.util.Optional;

@Repository
public interface ContactRepository extends MongoRepository<Contact, String> {
    List<Contact> findByUserId(String userId);
    
    Optional<Contact> findByUserIdAndContactUserId(String userId, String contactUserId);
    
    boolean existsByUserIdAndContactUserId(String userId, String contactUserId);
}

