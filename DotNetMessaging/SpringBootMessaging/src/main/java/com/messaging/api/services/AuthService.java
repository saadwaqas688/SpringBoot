package com.messaging.api.services;

import com.messaging.api.dtos.AuthDTOs;
import com.messaging.api.models.User;
import com.messaging.api.repositories.UserRepository;
import io.jsonwebtoken.Claims;
import io.jsonwebtoken.Jwts;
import io.jsonwebtoken.security.Keys;
import lombok.RequiredArgsConstructor;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.security.crypto.password.PasswordEncoder;
import org.springframework.stereotype.Service;

import javax.crypto.SecretKey;
import java.nio.charset.StandardCharsets;
import java.time.Instant;
import java.time.LocalDateTime;
import java.time.ZoneId;
import java.util.Date;
import java.util.Optional;

@Service
@RequiredArgsConstructor
public class AuthService {
    
    private final UserRepository userRepository;
    private final PasswordEncoder passwordEncoder;
    
    @Value("${jwt.secret:YourSuperSecretKeyThatIsAtLeast32CharactersLong!}")
    private String jwtSecret;
    
    @Value("${jwt.issuer:MessagingApp}")
    private String jwtIssuer;
    
    @Value("${jwt.audience:MessagingAppUsers}")
    private String jwtAudience;
    
    public Optional<AuthDTOs.AuthResponse> register(AuthDTOs.RegisterRequest request) {
        if (userRepository.existsByEmail(request.getEmail()) || 
            userRepository.existsByUsername(request.getUsername())) {
            return Optional.empty();
        }
        
        User user = new User();
        user.setUsername(request.getUsername());
        user.setEmail(request.getEmail());
        user.setPasswordHash(passwordEncoder.encode(request.getPassword()));
        user.setCreatedAt(LocalDateTime.now());
        
        user = userRepository.save(user);
        
        String token = generateJwtToken(user);
        return Optional.of(new AuthDTOs.AuthResponse(token, mapToDto(user)));
    }
    
    public Optional<AuthDTOs.AuthResponse> login(AuthDTOs.LoginRequest request) {
        Optional<User> userOpt = userRepository.findByEmail(request.getEmail());
        if (userOpt.isEmpty() || 
            !passwordEncoder.matches(request.getPassword(), userOpt.get().getPasswordHash())) {
            return Optional.empty();
        }
        
        User user = userOpt.get();
        user.setOnline(true);
        user.setLastSeen(LocalDateTime.now());
        user = userRepository.save(user);
        
        String token = generateJwtToken(user);
        return Optional.of(new AuthDTOs.AuthResponse(token, mapToDto(user)));
    }
    
    public String generateJwtToken(User user) {
        SecretKey key = Keys.hmacShaKeyFor(jwtSecret.getBytes(StandardCharsets.UTF_8));
        
        return Jwts.builder()
                .subject(user.getId())
                .claim("username", user.getUsername())
                .claim("email", user.getEmail())
                .issuer(jwtIssuer)
                .audience().add(jwtAudience).and()
                .issuedAt(Date.from(Instant.now()))
                .expiration(Date.from(Instant.now().plusSeconds(7 * 24 * 60 * 60))) // 7 days
                .signWith(key)
                .compact();
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

