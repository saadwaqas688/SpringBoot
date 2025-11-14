// ============================================================================
// PACKAGE DECLARATION
// ============================================================================
// This tells Java that this class belongs to the "com.example.todo.util" package.
// Packages organize related classes together, like folders organize files.
//
// NODE.JS EQUIVALENT: This is like the folder structure in Node.js
//   In Node.js: You'd have: src/util/jwtUtil.js
//   In Java:    com.example.todo.util.JwtUtil
//   The package name matches the folder structure!
package com.example.todo.util;

// ============================================================================
// IMPORTS - These bring in classes from other libraries that we need to use
// ============================================================================
// Claims: Represents the data (claims) stored inside a JWT token (like user info)
// NODE.JS EQUIVALENT: Like importing from 'jsonwebtoken' library
//   const jwt = require('jsonwebtoken');
//   // Claims would be like: jwt.decode(token) - the decoded payload
import io.jsonwebtoken.Claims;

// Jwts: Main class for creating and parsing JWT tokens (JWT = JSON Web Token)
// NODE.JS EQUIVALENT: This is like the 'jsonwebtoken' library itself
//   const jwt = require('jsonwebtoken');
//   // Jwts.builder() is like: jwt.sign(payload, secret)
//   // Jwts.parser() is like: jwt.verify(token, secret)
import io.jsonwebtoken.Jwts;

// Keys: Utility class for creating cryptographic keys used to sign/verify tokens
// NODE.JS EQUIVALENT: Like using crypto module to create keys
//   const crypto = require('crypto');
//   const key = crypto.createHmac('sha256', secret);
import io.jsonwebtoken.security.Keys;

// @Value: Spring annotation to inject values from application.properties file
// NODE.JS EQUIVALENT: Like using process.env or config files
//   // In Node.js you'd do:
//   const secret = process.env.JWT_SECRET || 'defaultSecret';
//   // Or with dotenv:
//   require('dotenv').config();
//   const secret = process.env.JWT_SECRET;
//   // In Java, @Value does this automatically!
import org.springframework.beans.factory.annotation.Value;

// @Component: Spring annotation that marks this class as a Spring component (bean)
// Spring will automatically create an instance of this class and manage it
// NODE.JS EQUIVALENT: This is like exporting a class/object that gets instantiated
//   // In Node.js, you'd manually create instances:
//   class JwtUtil { ... }
//   module.exports = new JwtUtil(); // Singleton pattern
//   // Or in Express with dependency injection:
//   // Spring does this automatically - you don't need 'new' keyword!
import org.springframework.stereotype.Component;

// SecretKey: Represents a secret cryptographic key used for signing tokens
// NODE.JS EQUIVALENT: Like Buffer or the key object from crypto module
//   const crypto = require('crypto');
//   const key = crypto.createSecretKey(Buffer.from(secret, 'utf8'));
import javax.crypto.SecretKey;

// StandardCharsets: Provides standard character encodings (like UTF-8)
// NODE.JS EQUIVALENT: Like specifying encoding in Buffer
//   Buffer.from(secret, 'utf8')  // 'utf8' is the charset
import java.nio.charset.StandardCharsets;

// Date: Represents a specific point in time (for token expiration)
// NODE.JS EQUIVALENT: JavaScript Date object (same concept!)
//   const now = new Date();
//   const expiry = new Date(now.getTime() + expiration);
import java.util.Date;

// Function: A functional interface that takes one input and returns one output
// Used here to extract different types of data from token claims
// NODE.JS EQUIVALENT: Like a callback function or arrow function
//   // In Node.js:
//   function getClaim(token, extractor) {
//     const claims = getAllClaims(token);
//     return extractor(claims);  // extractor is a function
//   }
//   // Usage:
//   getClaim(token, (claims) => claims.email);
import java.util.function.Function;

// ============================================================================
// CLASS DECLARATION
// ============================================================================
// @Component: This annotation tells Spring Framework:
//   - "Hey Spring, this is a component you should manage"
//   - Spring will create ONE instance of this class (singleton pattern)
//   - Other classes can automatically get this instance via dependency injection
//
// NODE.JS EQUIVALENT: 
//   // In Node.js, you'd manually export a singleton:
//   class JwtUtil {
//     constructor() {
//       this.secret = process.env.JWT_SECRET || 'default';
//       this.expiration = parseInt(process.env.JWT_EXPIRATION) || 86400000;
//     }
//   }
//   module.exports = new JwtUtil(); // Single instance
//
//   // Or with dependency injection (like in NestJS):
//   @Injectable()
//   export class JwtUtil { ... }
//
//   // In Spring, @Component does this automatically!
@Component
public class JwtUtil {
    // JwtUtil is a utility class that handles all JWT token operations:
    // - Creating (generating) new tokens
    // - Reading data from tokens
    // - Validating tokens
    // - Checking if tokens are expired

    // ============================================================================
    // CLASS FIELDS (Variables that belong to the class)
    // ============================================================================
    
    // @Value annotation: Injects a value from application.properties file
    // Syntax: ${property.name:defaultValue}
    //   - Looks for "jwt.secret" in application.properties
    //   - If not found, uses the default value after the colon (:)
    //   - The default is a long secret key (needs to be at least 256 bits for security)
    //
    // NODE.JS EQUIVALENT:
    //   // In Node.js you'd do:
    //   const secret = process.env.JWT_SECRET || 'mySecretKeyForJWTTokenGenerationThatShouldBeAtLeast256BitsLong';
    //   
    //   // Or with dotenv:
    //   require('dotenv').config();
    //   const secret = process.env.JWT_SECRET || 'defaultSecret';
    //
    //   // In Java, @Value does this automatically from application.properties!
    @Value("${jwt.secret:mySecretKeyForJWTTokenGenerationThatShouldBeAtLeast256BitsLong}")
    private String secret;  // This secret key is used to sign and verify tokens
                            // It's like a password - must be kept secret!
                            // NODE.JS: Like a class property: this.secret = '...';

    // @Value: Injects token expiration time from application.properties
    // Default: 86400000 milliseconds = 24 hours (24 * 60 * 60 * 1000)
    // This determines how long a token remains valid before it expires
    //
    // NODE.JS EQUIVALENT:
    //   const expiration = parseInt(process.env.JWT_EXPIRATION) || 86400000;
    @Value("${jwt.expiration:86400000}") // 24 hours in milliseconds
    private Long expiration;  // Stores the expiration time in milliseconds
                              // NODE.JS: Like: this.expiration = 86400000;

    // ============================================================================
    // PRIVATE HELPER METHOD
    // ============================================================================
    
    // This method converts the secret string into a cryptographic key
    // that can be used to sign and verify JWT tokens
    //
    // NODE.JS EQUIVALENT:
    //   getSigningKey() {
    //     // In Node.js with jsonwebtoken, you just use the secret string directly
    //     // But if you need to create a key object (like for crypto):
    //     const crypto = require('crypto');
    //     return crypto.createHmac('sha256', this.secret);
    //     // Or with jsonwebtoken library, you'd just use:
    //     // jwt.sign(payload, this.secret) - the library handles key creation
    //   }
    private SecretKey getSigningKey() {
        // Keys.hmacShaKeyFor(): Creates a secret key using HMAC-SHA algorithm
        // NODE.JS: crypto.createHmac('sha256', secret)
        
        // secret.getBytes(): Converts the secret string into bytes (raw data)
        // NODE.JS: Buffer.from(secret, 'utf8')
        
        // StandardCharsets.UTF_8: Specifies UTF-8 encoding (standard text encoding)
        // NODE.JS: 'utf8' encoding in Buffer.from(secret, 'utf8')
        
        // This key is used to "sign" tokens (like a signature) so we can verify
        // later that the token hasn't been tampered with
        return Keys.hmacShaKeyFor(secret.getBytes(StandardCharsets.UTF_8));
        // NODE.JS EQUIVALENT:
        //   const crypto = require('crypto');
        //   return crypto.createHmac('sha256', Buffer.from(this.secret, 'utf8'));
    }

    // ============================================================================
    // PUBLIC METHOD: Generate a new JWT token
    // ============================================================================
    
    // This method creates a new JWT token containing user information
    // Parameters:
    //   - userId: Unique identifier for the user
    //   - email: User's email address
    //   - role: User's role (like "USER" or "ADMIN")
    // Returns: A string containing the JWT token
    //
    // NODE.JS EQUIVALENT:
    //   generateToken(userId, email, role) {
    //     const now = new Date();
    //     const expiryDate = new Date(now.getTime() + this.expiration);
    //     
    //     const payload = {
    //       sub: userId,           // subject
    //       email: email,
    //       role: role,
    //       iat: Math.floor(now.getTime() / 1000),      // issued at (in seconds)
    //       exp: Math.floor(expiryDate.getTime() / 1000) // expiration (in seconds)
    //     };
    //     
    //     return jwt.sign(payload, this.secret);
    //   }
    public String generateToken(String userId, String email, String role) {
        // Create a Date object representing the current time (when token is created)
        // NODE.JS: const now = new Date();
        Date now = new Date();
        
        // Calculate when the token should expire:
        // now.getTime(): Gets current time in milliseconds since Jan 1, 1970
        // + expiration: Adds the expiration duration (e.g., 24 hours)
        // Creates a new Date object for the expiration time
        // NODE.JS: const expiryDate = new Date(now.getTime() + this.expiration);
        Date expiryDate = new Date(now.getTime() + expiration);

        // Jwts.builder(): Starts building a new JWT token
        // This uses the "builder pattern" - we chain methods together
        // NODE.JS: This is like building an object and then calling jwt.sign()
        return Jwts.builder()
                .subject(userId)              // Sets the subject (main identifier) - usually the user ID
                                              // NODE.JS: payload.sub = userId
                .claim("email", email)        // Adds a custom claim (piece of data) for email
                                              // NODE.JS: payload.email = email
                .claim("role", role)          // Adds a custom claim for the user's role
                                              // NODE.JS: payload.role = role
                .issuedAt(now)                // Records when the token was created
                                              // NODE.JS: payload.iat = Math.floor(now.getTime() / 1000)
                .expiration(expiryDate)        // Sets when the token will expire
                                              // NODE.JS: payload.exp = Math.floor(expiryDate.getTime() / 1000)
                .signWith(getSigningKey())     // Signs the token with our secret key
                                              // This ensures the token can't be tampered with
                                              // NODE.JS: jwt.sign(payload, this.secret)
                .compact();                   // Finalizes and converts the token to a compact string
                                              // The token looks like: "xxxxx.yyyyy.zzzzz"
                                              // NODE.JS: jwt.sign() already returns the compact string
    }

    // ============================================================================
    // PUBLIC METHODS: Extract data from tokens
    // ============================================================================
    
    // Gets the user ID from a token
    // The user ID is stored as the "subject" in the token
    //
    // NODE.JS EQUIVALENT:
    //   getUserIdFromToken(token) {
    //     const decoded = jwt.decode(token);
    //     return decoded.sub; // or decoded.userId
    //   }
    public String getUserIdFromToken(String token) {
        // Calls the generic getClaimFromToken method
        // Claims::getSubject is a method reference - it calls getSubject() on the Claims object
        // NODE.JS: This is like passing a function: (claims) => claims.sub
        return getClaimFromToken(token, Claims::getSubject);
    }

    // Gets the email from a token
    //
    // NODE.JS EQUIVALENT:
    //   getEmailFromToken(token) {
    //     const decoded = jwt.decode(token);
    //     return decoded.email;
    //   }
    public String getEmailFromToken(String token) {
        // Uses a lambda expression: claims -> claims.get("email", String.class)
        // This means: "Take the claims object, and call get() to extract the email"
        // "email" is the key, String.class tells it to return a String
        // NODE.JS: This is like an arrow function: (claims) => claims.email
        return getClaimFromToken(token, claims -> claims.get("email", String.class));
    }

    // Gets the role from a token
    //
    // NODE.JS EQUIVALENT:
    //   getRoleFromToken(token) {
    //     const decoded = jwt.decode(token);
    //     return decoded.role;
    //   }
    public String getRoleFromToken(String token) {
        // Similar to getEmailFromToken, but extracts the "role" claim
        // NODE.JS: (claims) => claims.role
        return getClaimFromToken(token, claims -> claims.get("role", String.class));
    }

    // Gets the expiration date from a token
    //
    // NODE.JS EQUIVALENT:
    //   getExpirationDateFromToken(token) {
    //     const decoded = jwt.decode(token);
    //     return new Date(decoded.exp * 1000); // JWT exp is in seconds, Date needs milliseconds
    //   }
    public Date getExpirationDateFromToken(String token) {
        // Claims::getExpiration is a method reference to get the expiration date
        // NODE.JS: (claims) => new Date(claims.exp * 1000)
        return getClaimFromToken(token, Claims::getExpiration);
    }

    // ============================================================================
    // GENERIC HELPER METHOD: Extract any claim from a token
    // ============================================================================
    
    // This is a generic method (uses <T>) that can return any type of data
    // Parameters:
    //   - token: The JWT token string
    //   - claimsResolver: A function that knows how to extract the specific data we want
    // Returns: The extracted data (type T - could be String, Date, etc.)
    //
    // NODE.JS EQUIVALENT:
    //   getClaimFromToken(token, extractor) {
    //     const claims = getAllClaimsFromToken(token);
    //     return extractor(claims);  // extractor is a function like: (claims) => claims.email
    //   }
    //
    //   // Usage:
    //   getClaimFromToken(token, (claims) => claims.sub);  // Get user ID
    //   getClaimFromToken(token, (claims) => claims.email); // Get email
    public <T> T getClaimFromToken(String token, Function<Claims, T> claimsResolver) {
        // final: Means this variable cannot be reassigned (good practice for safety)
        // NODE.JS: const claims = ... (const is like final in Java)
        
        // getAllClaimsFromToken(): Extracts all claims (data) from the token
        // NODE.JS: const claims = jwt.decode(token);
        final Claims claims = getAllClaimsFromToken(token);
        
        // claimsResolver.apply(): Uses the provided function to extract the specific claim
        // For example, if claimsResolver is Claims::getSubject, it will get the subject
        // NODE.JS: return extractor(claims);  // Just call the function directly
        return claimsResolver.apply(claims);
    }

    // ============================================================================
    // PRIVATE HELPER METHOD: Extract all claims from a token
    // ============================================================================
    
    // This method parses the token and extracts all the claims (data) from it
    // It also VERIFIES that the token is valid (not tampered with)
    //
    // NODE.JS EQUIVALENT:
    //   getAllClaimsFromToken(token) {
    //     try {
    //       // jwt.verify() both decodes AND verifies the signature
    //       return jwt.verify(token, this.secret);
    //       // If verification fails, it throws an error
    //     } catch (error) {
    //       throw new Error('Invalid token');
    //     }
    //   }
    //
    //   // Or if you just want to decode without verification:
    //   return jwt.decode(token);  // But this doesn't verify signature!
    private Claims getAllClaimsFromToken(String token) {
        // Jwts.parser(): Starts building a token parser
        // NODE.JS: This is like starting to use jwt.verify()
        return Jwts.parser()
                .verifyWith(getSigningKey())   // Sets the key to verify the token's signature
                                              // If the token was modified, this will fail
                                              // NODE.JS: jwt.verify(token, this.secret)
                .build()                       // Builds the parser
                                              // NODE.JS: Not needed - jwt.verify() does it all
                .parseSignedClaims(token)      // Parses the token and verifies its signature
                                              // Throws exception if token is invalid or tampered
                                              // NODE.JS: jwt.verify() does parsing + verification
                .getPayload();                 // Gets the payload (the actual data/claims)
                                              // Returns a Claims object containing all the data
                                              // NODE.JS: jwt.verify() returns the payload directly
    }

    // ============================================================================
    // PUBLIC METHOD: Check if token is expired
    // ============================================================================
    
    // Checks whether a token has expired (passed its expiration date)
    // Returns: true if expired, false if still valid
    //
    // NODE.JS EQUIVALENT:
    //   isTokenExpired(token) {
    //     const decoded = jwt.decode(token);
    //     const expiration = new Date(decoded.exp * 1000); // exp is in seconds
    //     return expiration < new Date(); // If expiration is before now, it's expired
    //   }
    public Boolean isTokenExpired(String token) {
        // Get the expiration date from the token
        // NODE.JS: const expiration = new Date(decoded.exp * 1000);
        final Date expiration = getExpirationDateFromToken(token);
        
        // expiration.before(new Date()): Checks if expiration date is before current time
        // If expiration is in the past, the token is expired (returns true)
        // NODE.JS: return expiration < new Date();
        return expiration.before(new Date());
    }

    // ============================================================================
    // PUBLIC METHODS: Validate tokens
    // ============================================================================
    
    // Validates a token for a specific user
    // Checks two things:
    //   1. Does the token belong to this user? (userId matches)
    //   2. Is the token still valid? (not expired)
    // Parameters:
    //   - token: The JWT token to validate
    //   - userId: The user ID we expect to be in the token
    // Returns: true if token is valid for this user, false otherwise
    //
    // NODE.JS EQUIVALENT:
    //   validateToken(token, userId) {
    //     try {
    //       const decoded = jwt.verify(token, this.secret);
    //       const tokenUserId = decoded.sub;
    //       return tokenUserId === userId && !this.isTokenExpired(token);
    //     } catch (error) {
    //       return false;
    //     }
    //   }
    public Boolean validateToken(String token, String userId) {
        // Extract the user ID from the token
        // NODE.JS: const tokenUserId = decoded.sub;
        final String tokenUserId = getUserIdFromToken(token);
        
        // Check both conditions:
        //   - tokenUserId.equals(userId): Does the token's user ID match the expected one?
        //     NODE.JS: tokenUserId === userId
        //   - !isTokenExpired(token): Is the token NOT expired? (! means "not")
        //     NODE.JS: !this.isTokenExpired(token)
        // Both must be true for the token to be valid
        // NODE.JS: return tokenUserId === userId && !this.isTokenExpired(token);
        return (tokenUserId.equals(userId) && !isTokenExpired(token));
    }

    // Validates a token without checking a specific user
    // This is a simpler validation - just checks if the token is valid and not expired
    // Parameters:
    //   - token: The JWT token to validate
    // Returns: true if token is valid, false if invalid or expired
    //
    // NODE.JS EQUIVALENT:
    //   validateToken(token) {
    //     try {
    //       jwt.verify(token, this.secret);  // This verifies signature AND expiration
    //       return true;  // If verify succeeds, token is valid
    //     } catch (error) {
    //       return false;  // If verify fails (invalid, expired, tampered), return false
    //     }
    //   }
    public Boolean validateToken(String token) {
        try {
            // Try to extract claims from the token
            // This will throw an exception if:
            //   - Token is malformed (not a valid JWT)
            //   - Token signature is invalid (was tampered with)
            //   - Token is expired
            // NODE.JS: jwt.verify(token, this.secret) throws error if invalid
            getAllClaimsFromToken(token);
            
            // If we got here, the token structure is valid
            // Now check if it's expired
            // NODE.JS: jwt.verify() already checks expiration, but we double-check here
            return !isTokenExpired(token);
        } catch (Exception e) {
            // If any exception occurred (invalid token, tampered, etc.), return false
            // This catches all errors and safely returns false instead of crashing
            // NODE.JS: catch (error) { return false; }
            return false;
        }
    }
}


