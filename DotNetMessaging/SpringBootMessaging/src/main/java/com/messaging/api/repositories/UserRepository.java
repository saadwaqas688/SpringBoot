package com.messaging.api.repositories;

import com.messaging.api.models.User;
import org.springframework.data.mongodb.repository.MongoRepository;
import org.springframework.data.mongodb.repository.Query;
import org.springframework.stereotype.Repository;

import java.util.Optional;

@Repository
public interface UserRepository extends MongoRepository<User, String> {
    Optional<User> findByEmail(String email);
    
    Optional<User> findByUsername(String username);
    
    @Query("{ $or: [ { email: ?0 }, { username: ?0 } ] }")
    Optional<User> findByEmailOrUsername(String emailOrUsername);
    
    boolean existsByEmail(String email);
    
    boolean existsByUsername(String username);
}

