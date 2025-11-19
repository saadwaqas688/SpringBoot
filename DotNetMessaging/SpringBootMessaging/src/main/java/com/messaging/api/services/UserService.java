package com.messaging.api.services;

import com.messaging.api.dtos.AuthDTOs;
import com.messaging.api.models.User;
import com.messaging.api.repositories.UserRepository;
import lombok.RequiredArgsConstructor;
import org.springframework.stereotype.Service;

import java.time.LocalDateTime;
import java.util.Optional;

@Service
@RequiredArgsConstructor
public class UserService {
    
    private final UserRepository userRepository;
    
    public Optional<AuthDTOs.UserDto> getUserById(String userId) {
        return userRepository.findById(userId)
                .map(this::mapToDto);
    }
    
    public void updateOnlineStatus(String userId, boolean isOnline) {
        userRepository.findById(userId).ifPresent(user -> {
            user.setOnline(isOnline);
            user.setLastSeen(LocalDateTime.now());
            userRepository.save(user);
        });
    }
    
    private AuthDTOs.UserDto mapToDto(User user) {
        return new AuthDTOs.UserDto(
                user.getId(),
                user.getUsername(),
                user.getEmail(),
                user.getProfilePictureUrl(),
                user.getStatus(),
                user.isOnline(),
                user.getLastSeen()
        );
    }
}

