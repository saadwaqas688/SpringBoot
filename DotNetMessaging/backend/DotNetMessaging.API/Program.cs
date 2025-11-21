// ============================================================================
// DOTNET MESSAGING API - PROGRAM.CS
// ============================================================================
// This is the main entry point and configuration file for the ASP.NET Core
// Web API application. It sets up dependency injection, middleware pipeline,
// authentication, database connections, and all application services.
//
// ============================================================================
// ENTRY POINT & REQUEST FLOW EXPLANATION
// ============================================================================
// 
// 1. ENTRY POINT:
//    - This file (Program.cs) is the application entry point
//    - Execution starts at line 21: var builder = WebApplication.CreateBuilder(args)
//    - The application builds and starts at line 533: var app = builder.Build()
//    - Server starts listening at line 775: app.Run("http://localhost:5000")
//
// 2. REQUEST FLOW (Example: GET /api/messages/chat/123):
//    
//    ┌─────────────────────────────────────────────────────────────────┐
//    │  CLIENT REQUEST                                                  │
//    │  GET http://localhost:5000/api/messages/chat/123                │
//    │  Headers: Authorization: Bearer <JWT_TOKEN>                     │
//    └──────────────────────┬──────────────────────────────────────────┘
//                           │
//                           ▼
//    ┌─────────────────────────────────────────────────────────────────┐
//    │  MIDDLEWARE PIPELINE (Executed in order)                        │
//    │  ────────────────────────────────────────────────────────────   │
//    │  1. Swagger (if /swagger path)                                   │
//    │  2. Static Files (if /uploads/* path)                           │
//    │  3. CORS - Validates origin, adds CORS headers                  │
//    │  4. Authentication - Extracts & validates JWT token             │
//    │     - Reads Authorization header or query string                │
//     │     - Validates token signature, expiry, issuer, audience      │
//    │     - Sets HttpContext.User with claims (userId, etc.)          │
//    │  5. Authorization - Checks [Authorize] attributes                │
//    │     - Verifies user is authenticated                             │
//    │     - Can check roles/permissions if specified                   │
//    └──────────────────────┬──────────────────────────────────────────┘
//                           │
//                           ▼
//    ┌─────────────────────────────────────────────────────────────────┐
//    │  ROUTING (app.MapControllers() - line 593)                      │
//    │  ────────────────────────────────────────────────────────────   │
//    │  - Matches URL pattern: /api/messages/chat/{chatId}             │
//    │  - Finds: MessagesController.GetChatMessages()                 │
//    │  - Extracts route parameters: chatId = "123"                     │
//    │  - Extracts query parameters: skip, take                        │
//    └──────────────────────┬──────────────────────────────────────────┘
//                           │
//                           ▼
//    ┌─────────────────────────────────────────────────────────────────┐
//    │  CONTROLLER (MessagesController.cs)                             │
//    │  ────────────────────────────────────────────────────────────   │
//    │  [ApiController]                                                 │
//    │  [Route("api/[controller]")]  → /api/messages                   │
//    │  [Authorize]  → Requires authentication                          │
//    │                                                                   │
//    │  [HttpGet("chat/{chatId}")]                                      │
//    │  public async Task<ActionResult> GetChatMessages(                │
//    │      string chatId, int skip, int take)                          │
//    │  {                                                               │
//    │      var userId = GetCurrentUserId(); // From JWT claims         │
//    │      var messages = await _messageService.GetChatMessagesAsync(  │
//    │          chatId, userId, skip, take);                            │
//    │      return Ok(messages);                                        │
//    │  }                                                               │
//    │                                                                   │
//    │  Dependency Injection provides:                                  │
//    │  - IMessageService (MessageService instance)                     │
//    │  - IHubContext<ChatHub> (for SignalR)                            │
//    └──────────────────────┬──────────────────────────────────────────┘
//                           │
//                           ▼
//    ┌─────────────────────────────────────────────────────────────────┐
//    │  SERVICE LAYER (MessageService.cs)                              │
//    │  ────────────────────────────────────────────────────────────   │
//    │  Business Logic & Validation:                                    │
//    │  - Validates user has access to chat                             │
//    │  - Coordinates between repositories                              │
//    │  - Transforms data (Model → DTO)                                 │
//    │                                                                   │
//    │  public async Task<List<MessageDto>> GetChatMessagesAsync(      │
//    │      string chatId, string userId, int skip, int take)           │
//    │  {                                                               │
//    │      // 1. Check user has access                                │
//    │      var chat = await _chatRepository.GetByIdAsync(chatId);     │
//    │      if (chat.User1Id != userId && chat.User2Id != userId)      │
//    │          throw new UnauthorizedAccessException();                │
//    │                                                                   │
//    │      // 2. Get messages from repository                          │
//    │      var messages = await _messageRepository                     │
//    │          .GetChatMessagesAsync(chatId, skip, take);             │
//    │                                                                   │
//    │      // 3. Transform to DTOs                                    │
//    │      return messages.Select(m => new MessageDto { ... }).ToList();│
//    │  }                                                               │
//    │                                                                   │
//    │  Dependency Injection provides:                                  │
//    │  - IMessageRepository                                            │
//    │  - IChatRepository                                               │
//    │  - IUserRepository                                               │
//    │  - etc.                                                          │
//    └──────────────────────┬──────────────────────────────────────────┘
//                           │
//                           ▼
//    ┌─────────────────────────────────────────────────────────────────┐
//    │  REPOSITORY LAYER (MessageRepository.cs)                        │
//    │  ────────────────────────────────────────────────────────────   │
//    │  Data Access - Direct MongoDB operations:                       │
//    │                                                                   │
//    │  public async Task<IEnumerable<Message>>                          │
//    │      GetChatMessagesAsync(string chatId, int skip, int take)    │
//    │  {                                                               │
//    │      var filter = Builders<Message>.Filter.And(                  │
//    │          Builders<Message>.Filter.Eq(m => m.ChatId, chatId),     │
//    │          Builders<Message>.Filter.Eq(m => m.IsDeleted, false)   │
//    │      );                                                          │
//    │                                                                   │
//    │      return await _collection.Find(filter)                       │
//    │          .SortByDescending(m => m.CreatedAt)                     │
//    │          .Skip(skip)                                             │
//    │          .Limit(take)                                             │
//    │          .ToListAsync();                                         │
//    │  }                                                               │
//    │                                                                   │
//    │  Dependency Injection provides:                                  │
//    │  - MongoDbContext (contains IMongoDatabase)                      │
//    └──────────────────────┬──────────────────────────────────────────┘
//                           │
//                           ▼
//    ┌─────────────────────────────────────────────────────────────────┐
//    │  DATABASE (MongoDB)                                             │
//    │  ────────────────────────────────────────────────────────────   │
//    │  - Executes query on "messages" collection                       │
//    │  - Returns matching documents                                    │
//    └──────────────────────┬──────────────────────────────────────────┘
//                           │
//                           ▼
//    ┌─────────────────────────────────────────────────────────────────┐
//    │  RESPONSE FLOW (Reverse order)                                  │
//    │  ────────────────────────────────────────────────────────────   │
//    │  Repository → Service → Controller → Middleware → Client        │
//    │                                                                   │
//    │  1. Repository returns: IEnumerable<Message>                     │
//    │  2. Service transforms to: List<MessageDto>                      │
//    │  3. Controller returns: Ok(messages) → 200 OK                   │
//    │  4. Middleware serializes to JSON (camelCase)                    │
//    │  5. CORS adds headers                                            │
//    │  6. Response sent to client                                      │
//    └─────────────────────────────────────────────────────────────────┘
//
// 3. DEPENDENCY INJECTION LIFECYCLE:
//    - Singleton: Created once, shared across all requests (MongoDB client)
//    - Scoped: Created once per HTTP request (Repositories, Services)
//    - Transient: Created every time it's requested (rarely used)
//
// 4. CONTROLLER DISCOVERY:
//    - Controllers are automatically discovered from Controllers/ folder
//    - Must inherit from ControllerBase and have [ApiController] attribute
//    - Route attributes determine URL patterns
//
// 5. SIGNALR/WEBSOCKET FLOW:
//    - Different path: /chathub (not /api/*)
//    - Bypasses controller routing
//    - Goes through: CORS → Authentication → ChatHub
//    - Token extracted from query string: ?access_token=...
//
// ============================================================================

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
using MongoDB.Driver;
using DotNetMessaging.API.Data;
using DotNetMessaging.API.Hubs;
using DotNetMessaging.API.Services;
using DotNetMessaging.API.Repositories;

// ============================================================================
// APPLICATION ENTRY POINT - EXECUTION STARTS HERE
// ============================================================================
// Create the WebApplication builder which provides access to configuration,
// services, logging, and environment settings
// This is where the application bootstrap begins
var builder = WebApplication.CreateBuilder(args);

// ============================================================================
// NESTJS EQUIVALENT:
// ============================================================================
// import { NestFactory } from '@nestjs/core';
// import { AppModule } from './app.module';
// 
// async function bootstrap() {
//   const app = await NestFactory.create(AppModule);
//   // ... configuration
//   await app.listen(5000);
// }
// bootstrap();
// ============================================================================

// ============================================================================
// CONTROLLER & JSON CONFIGURATION
// ============================================================================
// Configure controllers and JSON serialization options for the API
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // PropertyNameCaseInsensitive: Allows the API to accept JSON properties
        // in both camelCase (from JavaScript clients) and PascalCase (from .NET clients)
        // This provides flexibility when receiving requests from different client types
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
        
        // PropertyNamingPolicy: Ensures all JSON responses are serialized in camelCase
        // This is the standard convention for JavaScript/TypeScript clients (React, etc.)
        // Example: C# property "UserId" becomes "userId" in JSON
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    });

// Add API Explorer for endpoint discovery (used by Swagger to generate documentation)
builder.Services.AddEndpointsApiExplorer();

// Add Swagger generator to create interactive API documentation
// Swagger UI will be available at /swagger endpoint
builder.Services.AddSwaggerGen();

// ============================================================================
// NESTJS EQUIVALENT (in main.ts):
// ============================================================================
// import { ValidationPipe } from '@nestjs/common';
// 
// app.useGlobalPipes(new ValidationPipe({
//   transform: true,                    // Auto-transform payloads to DTOs
//   whitelist: true,                    // Strip unknown properties
//   forbidNonWhitelisted: true          // Reject unknown properties
// }));
// 
// // JSON serialization is camelCase by default in NestJS
// // Swagger setup:
// import { SwaggerModule, DocumentBuilder } from '@nestjs/swagger';
// 
// const config = new DocumentBuilder()
//   .setTitle('Messaging API')
//   .setVersion('1.0')
//   .addBearerAuth()
//   .build();
// const document = SwaggerModule.createDocument(app, config);
// SwaggerModule.setup('swagger', app, document);
// ============================================================================

// ============================================================================
// MONGODB DATABASE CONFIGURATION
// ============================================================================
// Configure MongoDB connection and database access
// MongoDB is a NoSQL document database used to store users, messages, chats, etc.

// Retrieve MongoDB connection string from appsettings.json
// Falls back to default localhost connection if not configured
// Format: "mongodb://hostname:port" or "mongodb://username:password@hostname:port/database"
var mongoConnectionString = builder.Configuration.GetConnectionString("MongoDB") ?? "mongodb://localhost:27017";

// Retrieve database name from configuration
// Falls back to "messagingapp" if not specified in appsettings.json
var mongoDatabaseName = builder.Configuration["MongoDB:DatabaseName"] ?? "messagingapp";

// Create MongoDB client instance (manages connection pooling and server communication)
var mongoClient = new MongoClient(mongoConnectionString);

// Get reference to the specific database within MongoDB
// This doesn't create the database - it's created automatically when first document is inserted
var mongoDatabase = mongoClient.GetDatabase(mongoDatabaseName);

// Register IMongoDatabase as Singleton (single instance shared across all requests)
// This is safe because IMongoDatabase is thread-safe and manages its own connection pooling
builder.Services.AddSingleton<IMongoDatabase>(mongoDatabase);

// Register MongoDbContext as Scoped (new instance per HTTP request)
// This provides a request-scoped context for database operations
builder.Services.AddScoped<MongoDbContext>();

// ============================================================================
// NESTJS EQUIVALENT (in app.module.ts):
// ============================================================================
// import { MongooseModule } from '@nestjs/mongoose';
// 
// @Module({
//   imports: [
//     MongooseModule.forRoot(
//       process.env.MONGODB_URI || 'mongodb://localhost:27017',
//       {
//         dbName: process.env.MONGODB_DATABASE || 'messagingapp',
//       }
//     ),
//     // ... other modules
//   ],
// })
// export class AppModule {}
// 
// // Or using ConfigModule:
// import { ConfigModule, ConfigService } from '@nestjs/config';
// 
// MongooseModule.forRootAsync({
//   imports: [ConfigModule],
//   useFactory: async (configService: ConfigService) => ({
//     uri: configService.get<string>('MONGODB_URI') || 'mongodb://localhost:27017',
//     dbName: configService.get<string>('MONGODB_DATABASE') || 'messagingapp',
//   }),
//   inject: [ConfigService],
// })
// ============================================================================

// ============================================================================
// REPOSITORY LAYER - DATA ACCESS
// ============================================================================
// Register repository interfaces and implementations using Dependency Injection
// Repositories abstract database operations and provide a clean interface for services
// All repositories are registered as Scoped (one instance per HTTP request)

// User repository: Handles CRUD operations for user accounts
builder.Services.AddScoped<IUserRepository, UserRepository>();

// Chat repository: Manages one-on-one chat conversations between users
builder.Services.AddScoped<IChatRepository, ChatRepository>();

// Message repository: Handles storing and retrieving chat messages
builder.Services.AddScoped<IMessageRepository, MessageRepository>();

// Group repository: Manages group chat entities
builder.Services.AddScoped<IGroupRepository, GroupRepository>();

// Group member repository: Handles membership relationships between users and groups
builder.Services.AddScoped<IGroupMemberRepository, GroupMemberRepository>();

// Contact repository: Manages user contact lists and relationships
builder.Services.AddScoped<IContactRepository, ContactRepository>();

// Message reaction repository: Handles emoji reactions on messages (like, love, etc.)
builder.Services.AddScoped<IMessageReactionRepository, MessageReactionRepository>();

// Message read repository: Tracks which users have read which messages (read receipts)
builder.Services.AddScoped<IMessageReadRepository, MessageReadRepository>();

// ============================================================================
// NESTJS EQUIVALENT (in app.module.ts or feature modules):
// ============================================================================
// @Module({
//   providers: [
//     // Repositories (using @Injectable() decorator)
//     UserRepository,
//     ChatRepository,
//     MessageRepository,
//     GroupRepository,
//     GroupMemberRepository,
//     ContactRepository,
//     MessageReactionRepository,
//     MessageReadRepository,
//     // Services
//     AuthService,
//     ChatService,
//     MessageService,
//     GroupService,
//     ContactService,
//     UserService,
//   ],
//   exports: [/* export what other modules need */],
// })
// export class AppModule {}
// 
// // Or in separate feature modules:
// @Module({
//   providers: [UserRepository, UserService],
//   controllers: [UsersController],
//   exports: [UserService],
// })
// export class UsersModule {}
// ============================================================================

// ============================================================================
// SERVICE LAYER - BUSINESS LOGIC
// ============================================================================
// Register service interfaces and implementations
// Services contain business logic and coordinate between repositories and controllers
// All services are registered as Scoped (one instance per HTTP request)

// Authentication service: Handles user registration, login, JWT token generation
builder.Services.AddScoped<IAuthService, AuthService>();

// Chat service: Manages chat creation, retrieval, and user chat lists
builder.Services.AddScoped<IChatService, ChatService>();

// Message service: Handles message sending, retrieval, reactions, and read receipts
builder.Services.AddScoped<IMessageService, MessageService>();

// Group service: Manages group creation, member management, and group operations
builder.Services.AddScoped<IGroupService, GroupService>();

// Contact service: Handles contact list management and user discovery
builder.Services.AddScoped<IContactService, ContactService>();

// User service: Manages user profiles, search, and user-related operations
builder.Services.AddScoped<IUserService, UserService>();

// ============================================================================
// SIGNALR - REAL-TIME COMMUNICATION
// ============================================================================
// SignalR enables real-time bidirectional communication between server and clients
// Used for instant message delivery, typing indicators, online status, etc.
// The ChatHub will handle all SignalR connections and message broadcasting
builder.Services.AddSignalR();

// ============================================================================
// NESTJS EQUIVALENT (using Socket.IO or WebSockets):
// ============================================================================
// // Option 1: Using @nestjs/websockets with Socket.IO
// import { WebSocketGateway, WebSocketServer } from '@nestjs/websockets';
// import { Server } from 'socket.io';
// 
// @WebSocketGateway({
//   cors: {
//     origin: ['http://localhost:3000'],
//     credentials: true,
//   },
//   namespace: '/chathub',
// })
// export class ChatGateway {
//   @WebSocketServer()
//   server: Server;
// }
// 
// // In app.module.ts:
// import { ChatGateway } from './hubs/chat.gateway';
// 
// @Module({
//   providers: [ChatGateway],
// })
// 
// // Option 2: Using native WebSockets
// import { WebSocketGateway } from '@nestjs/websockets';
// ============================================================================

// ============================================================================
// CORS (CROSS-ORIGIN RESOURCE SHARING) CONFIGURATION
// ============================================================================
// CORS allows the React frontend (running on different port) to access this API
// Without CORS, browsers block requests from different origins due to same-origin policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        // Allow requests from React development server (typically runs on port 3000)
        policy.WithOrigins("http://localhost:3000", "http://127.0.0.1:3000")
            // Allow any HTTP headers (Authorization, Content-Type, etc.)
            .AllowAnyHeader()
            // Allow all HTTP methods (GET, POST, PUT, DELETE, etc.)
            .AllowAnyMethod()
            // Allow credentials (cookies, authorization headers) to be sent with requests
            // Required for JWT authentication and SignalR connections
            .AllowCredentials()
            // DEVELOPMENT ONLY: Allow any origin (bypasses origin validation)
            // WARNING: Remove or restrict this in production for security
            .SetIsOriginAllowed(origin => true);
    });
});

// ============================================================================
// NESTJS EQUIVALENT (in main.ts):
// ============================================================================
// app.enableCors({
//   origin: ['http://localhost:3000', 'http://127.0.0.1:3000'],
//   methods: 'GET,HEAD,PUT,PATCH,POST,DELETE,OPTIONS',
//   credentials: true,
//   allowedHeaders: '*',
// });
// 
// // Or using @nestjs/config:
// import { ConfigService } from '@nestjs/config';
// 
// const configService = app.get(ConfigService);
// app.enableCors({
//   origin: configService.get('CORS_ORIGINS')?.split(',') || ['http://localhost:3000'],
//   credentials: true,
// });
// ============================================================================

// ============================================================================
// JWT (JSON WEB TOKEN) AUTHENTICATION CONFIGURATION
// ============================================================================
// JWT is used for stateless authentication - tokens contain user identity and claims
// Tokens are signed with a secret key to prevent tampering

// Retrieve JWT configuration from appsettings.json with fallback defaults
// JWT Key: Secret key used to sign and verify tokens (must be at least 32 characters)
//          In production, use a strong, randomly generated key stored securely
var jwtKey = builder.Configuration["Jwt:Key"] ?? "YourSuperSecretKeyThatIsAtLeast32CharactersLong!";

// JWT Issuer: Identifies who issued the token (typically the application name)
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "MessagingApp";

// JWT Audience: Identifies who the token is intended for (typically the application users)
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "MessagingAppUsers";

// Configure JWT Bearer authentication scheme
// This enables the API to validate JWT tokens sent by clients
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        // Token validation parameters define how tokens are validated
        options.TokenValidationParameters = new TokenValidationParameters
        {
            // Validate that the token was issued by our application
            ValidateIssuer = true,
            
            // Validate that the token is intended for our application
            ValidateAudience = true,
            
            // Validate that the token hasn't expired
            ValidateLifetime = true,
            
            // Validate the token's signature to ensure it hasn't been tampered with
            ValidateIssuerSigningKey = true,
            
            // Set the expected issuer value (must match what's in the token)
            ValidIssuer = jwtIssuer,
            
            // Set the expected audience value (must match what's in the token)
            ValidAudience = jwtAudience,
            
            // Set the secret key used to verify the token's signature
            // The same key must be used when creating tokens (in AuthService)
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            
            // Map the NameIdentifier claim to the user's ID
            // This allows accessing user ID via context.User.FindFirst(ClaimTypes.NameIdentifier)
            NameClaimType = ClaimTypes.NameIdentifier
        };

        // ========================================================================
        // SIGNALR JWT TOKEN EXTRACTION
        // ========================================================================
        // SignalR WebSocket connections cannot use standard Authorization headers
        // Instead, tokens are passed via query string or connection options
        // These events handle token extraction for SignalR connections
        options.Events = new JwtBearerEvents
        {
            // Called when a request is received - extract token from query string or header
            OnMessageReceived = context =>
            {
                // Method 1: Try to get token from query string (common for SignalR)
                // Format: /chathub?access_token=eyJhbGc...
                var accessToken = context.Request.Query["access_token"];
                var path = context.HttpContext.Request.Path;
                
                // Debug logging to help troubleshoot authentication issues
                Console.WriteLine($"[JWT] OnMessageReceived - Path: {path}, HasToken: {!string.IsNullOrEmpty(accessToken)}");
                
                // If token found in query string and path is SignalR hub, use it
                if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/chathub"))
                {
                    context.Token = accessToken;
                    Console.WriteLine($"[JWT] Token extracted from query string for SignalR connection");
                }
                else
                {
                    // Method 2: Fallback - try to get token from Authorization header
                    // Format: Authorization: Bearer eyJhbGc...
                    // This works for regular HTTP requests and some SignalR transports
                    var authHeader = context.Request.Headers["Authorization"].ToString();
                    if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
                    {
                        // Extract token by removing "Bearer " prefix
                        context.Token = authHeader.Substring("Bearer ".Length).Trim();
                        Console.WriteLine($"[JWT] Token extracted from Authorization header");
                    }
                    else
                    {
                        // No token found - authentication will fail
                        Console.WriteLine($"[JWT] WARNING: No token found for SignalR connection");
                    }
                }
                return Task.CompletedTask;
            },
            
            // Called when token validation fails (expired, invalid signature, etc.)
            OnAuthenticationFailed = context =>
            {
                // Log authentication failures for debugging
                Console.WriteLine($"[JWT] Authentication failed: {context.Exception?.Message}");
                return Task.CompletedTask;
            },
            
            // Called when token is successfully validated
            OnTokenValidated = context =>
            {
                // Extract user ID from validated token for logging/debugging
                var userId = context.Principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                Console.WriteLine($"[JWT] Token validated successfully. UserId: {userId}");
                return Task.CompletedTask;
            }
        };
    });

// Add authorization services (required for [Authorize] attributes on controllers)
builder.Services.AddAuthorization();

// ============================================================================
// NESTJS EQUIVALENT (JWT Authentication):
// ============================================================================
// // 1. Install: npm install @nestjs/jwt @nestjs/passport passport passport-jwt
// 
// // 2. In app.module.ts:
// import { JwtModule } from '@nestjs/jwt';
// import { PassportModule } from '@nestjs/passport';
// import { JwtStrategy } from './auth/jwt.strategy';
// 
// @Module({
//   imports: [
//     PassportModule.register({ defaultStrategy: 'jwt' }),
//     JwtModule.registerAsync({
//       imports: [ConfigModule],
//       useFactory: async (configService: ConfigService) => ({
//         secret: configService.get<string>('JWT_KEY') || 
//                'YourSuperSecretKeyThatIsAtLeast32CharactersLong!',
//         signOptions: {
//           issuer: configService.get<string>('JWT_ISSUER') || 'MessagingApp',
//           audience: configService.get<string>('JWT_AUDIENCE') || 'MessagingAppUsers',
//           expiresIn: '7d', // or from config
//         },
//         verifyOptions: {
//           issuer: configService.get<string>('JWT_ISSUER') || 'MessagingApp',
//           audience: configService.get<string>('JWT_AUDIENCE') || 'MessagingAppUsers',
//         },
//       }),
//       inject: [ConfigService],
//     }),
//   ],
//   providers: [JwtStrategy],
// })
// 
// // 3. Create jwt.strategy.ts:
// import { Injectable, UnauthorizedException } from '@nestjs/common';
// import { PassportStrategy } from '@nestjs/passport';
// import { ExtractJwt, Strategy } from 'passport-jwt';
// import { ConfigService } from '@nestjs/config';
// 
// @Injectable()
// export class JwtStrategy extends PassportStrategy(Strategy) {
//   constructor(private configService: ConfigService) {
//     super({
//       jwtFromRequest: ExtractJwt.fromAuthHeaderAsBearerToken(),
//       // For WebSocket/Socket.IO, also extract from query:
//       jwtFromRequest: ExtractJwt.fromExtractors([
//         (request) => {
//           // Try query string first (for WebSocket)
//           if (request?.query?.access_token) {
//             return request.query.access_token;
//           }
//           // Fallback to Authorization header
//           return ExtractJwt.fromAuthHeaderAsBearerToken()(request);
//         },
//       ]),
//       ignoreExpiration: false,
//       secretOrKey: configService.get<string>('JWT_KEY'),
//       issuer: configService.get<string>('JWT_ISSUER'),
//       audience: configService.get<string>('JWT_AUDIENCE'),
//     });
//   }
// 
//   async validate(payload: any) {
//     // Payload contains decoded JWT token
//     // Return user object that will be attached to request.user
//     return { userId: payload.sub || payload.nameid, username: payload.username };
//   }
// }
// 
// // 4. Use in controllers:
// import { UseGuards } from '@nestjs/common';
// import { AuthGuard } from '@nestjs/passport';
// 
// @Controller('messages')
// @UseGuards(AuthGuard('jwt'))
// export class MessagesController {
//   // All routes require JWT authentication
// }
// 
// // Or on specific routes:
// @Get()
// @UseGuards(AuthGuard('jwt'))
// findAll(@Request() req) {
//   const userId = req.user.userId; // From JWT payload
// }
// ============================================================================

// ============================================================================
// BUILD APPLICATION
// ============================================================================
// Build the WebApplication instance from the configured builder
// This creates the application with all registered services and middleware
var app = builder.Build();

// ============================================================================
// MIDDLEWARE PIPELINE CONFIGURATION
// ============================================================================
// Middleware components process HTTP requests in order (top to bottom)
// Order matters! Each middleware can modify the request/response or short-circuit

// Swagger UI - Interactive API documentation
// Access at: http://localhost:5000/swagger
// Only enable in development - disable in production for security
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    // Endpoint where Swagger JSON schema is served
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Messaging API V1");
    // URL path where Swagger UI will be accessible
    c.RoutePrefix = "swagger";
});

// ============================================================================
// NESTJS EQUIVALENT (Swagger is configured in main.ts):
// ============================================================================
// // Already shown above in the JSON configuration section
// // SwaggerModule.setup('swagger', app, document);
// ============================================================================

// Serve static files (images, documents, etc.) from wwwroot folder
// Used for file uploads (media, documents) that users send in messages
app.UseStaticFiles();

// ============================================================================
// NESTJS EQUIVALENT (in main.ts):
// ============================================================================
// import { NestExpressApplication } from '@nestjs/platform-express';
// import { join } from 'path';
// 
// const app = await NestFactory.create<NestExpressApplication>(AppModule);
// app.useStaticAssets(join(__dirname, '..', 'public'), {
//   prefix: '/uploads/', // Serve files at /uploads/* path
// });
// ============================================================================

// CORS middleware - MUST be placed before UseRouting/UseAuthentication
// This allows cross-origin requests from the React frontend
// If placed after authentication, CORS preflight requests will fail
app.UseCors("AllowReactApp");

// Authentication middleware - Validates JWT tokens and sets User identity
// Must be called before UseAuthorization
// This reads the Authorization header or query string and validates the JWT
app.UseAuthentication();

// Authorization middleware - Enforces [Authorize] attributes on controllers/actions
// Must be called after UseAuthentication
// This checks if the authenticated user has permission to access the resource
app.UseAuthorization();

// ============================================================================
// CONTROLLER ROUTING - MAPS URL PATTERNS TO CONTROLLER ACTIONS
// ============================================================================
// This discovers and maps all controllers from the Controllers/ folder
// 
// How routing works:
// 1. Controllers must have [ApiController] attribute
// 2. [Route("api/[controller]")] sets base path (e.g., /api/messages)
// 3. Action methods have [HttpGet], [HttpPost], etc. with paths
// 4. Example: MessagesController with [HttpGet("chat/{chatId}")]
//    → Maps to: GET /api/messages/chat/{chatId}
//
// Request matching process:
// - URL: GET /api/messages/chat/123?skip=0&take=50
// - Matches: MessagesController.GetChatMessages(string chatId, int skip, int take)
// - Route params: chatId = "123"
// - Query params: skip = 0, take = 50
// - Dependency injection provides: IMessageService, IHubContext<ChatHub>
//
// Controllers discovered automatically:
// - Controllers/AuthController.cs → /api/auth/*
// - Controllers/MessagesController.cs → /api/messages/*
// - Controllers/ChatsController.cs → /api/chats/*
// - Controllers/GroupsController.cs → /api/groups/*
// - Controllers/UsersController.cs → /api/users/*
// - Controllers/MediaController.cs → /api/media/*
app.MapControllers();

// ============================================================================
// SIGNALR HUB MAPPING
// ============================================================================
// Map the ChatHub SignalR endpoint
// Clients connect to: ws://localhost:5000/chathub
// .RequireAuthorization() ensures only authenticated users can connect
// The JWT token must be provided in query string or connection options
app.MapHub<ChatHub>("/chathub")
    .RequireAuthorization();

// ============================================================================
// NESTJS EQUIVALENT (Socket.IO Gateway with Authentication):
// ============================================================================
// // In chat.gateway.ts:
// import { WebSocketGateway, WebSocketServer, OnGatewayConnection } from '@nestjs/websockets';
// import { UseGuards } from '@nestjs/common';
// import { JwtService } from '@nestjs/jwt';
// import { Server, Socket } from 'socket.io';
// 
// @WebSocketGateway({
//   namespace: '/chathub',
//   cors: {
//     origin: ['http://localhost:3000'],
//     credentials: true,
//   },
// })
// export class ChatGateway implements OnGatewayConnection {
//   @WebSocketServer()
//   server: Server;
// 
//   constructor(private jwtService: JwtService) {}
// 
//   async handleConnection(client: Socket) {
//     try {
//       // Extract token from query string or handshake auth
//       const token = client.handshake.query.access_token || 
//                    client.handshake.auth?.token ||
//                    client.handshake.headers.authorization?.replace('Bearer ', '');
// 
//       if (!token) {
//         client.disconnect();
//         return;
//       }
// 
//       // Verify JWT token
//       const payload = await this.jwtService.verifyAsync(token, {
//         secret: process.env.JWT_KEY,
//         issuer: process.env.JWT_ISSUER,
//         audience: process.env.JWT_AUDIENCE,
//       });
// 
//       // Attach user info to socket
//       client.data.userId = payload.sub || payload.nameid;
//       console.log(`[Socket] User connected: ${client.data.userId}`);
//     } catch (error) {
//       console.log(`[Socket] Authentication failed: ${error.message}`);
//       client.disconnect();
//     }
//   }
// 
//   @SubscribeMessage('sendMessage')
//   handleMessage(client: Socket, payload: any) {
//     // client.data.userId is available from handleConnection
//     this.server.emit('newMessage', payload);
//   }
// }
// ============================================================================

// ============================================================================
// START APPLICATION - SERVER BEGINS LISTENING HERE
// ============================================================================
// This is where the HTTP server starts listening for incoming requests
// All middleware and routing is now active and ready to process requests
// 
// Execution flow:
// 1. Application builds (line 533: var app = builder.Build())
// 2. Middleware pipeline configured (lines 544-593)
// 3. Controllers mapped (line 593: app.MapControllers())
// 4. SignalR hub mapped (line 602: app.MapHub<ChatHub>())
// 5. Server starts listening (THIS LINE) - blocks until shutdown
//
// The application will run until stopped (Ctrl+C or shutdown signal)
// In production, use environment variables or configuration for the URL
// 
// Once this line executes, the server is live and accepting connections:
// - HTTP requests → http://localhost:5000/api/*
// - WebSocket/SignalR → ws://localhost:5000/chathub
// - Swagger UI → http://localhost:5000/swagger
app.Run("http://localhost:5000");

// ============================================================================
// NESTJS EQUIVALENT (in main.ts):
// ============================================================================
// import { NestFactory } from '@nestjs/core';
// import { AppModule } from './app.module';
// import { ValidationPipe } from '@nestjs/common';
// import { SwaggerModule, DocumentBuilder } from '@nestjs/swagger';
// 
// async function bootstrap() {
//   const app = await NestFactory.create(AppModule);
// 
//   // Global validation pipe
//   app.useGlobalPipes(new ValidationPipe({
//     transform: true,
//     whitelist: true,
//   }));
// 
//   // CORS
//   app.enableCors({
//     origin: ['http://localhost:3000'],
//     credentials: true,
//   });
// 
//   // Swagger
//   const config = new DocumentBuilder()
//     .setTitle('Messaging API')
//     .setVersion('1.0')
//     .addBearerAuth()
//     .build();
//   const document = SwaggerModule.createDocument(app, config);
//   SwaggerModule.setup('swagger', app, document);
// 
//   // Static files
//   app.useStaticAssets(join(__dirname, '..', 'public'), {
//     prefix: '/uploads/',
//   });
// 
//   // Start server
//   const port = process.env.PORT || 5000;
//   await app.listen(port);
//   console.log(`Application is running on: http://localhost:${port}`);
// }
// 
// bootstrap();
// 
// // ============================================================================
// // PLAIN NODE.JS/EXPRESS EQUIVALENT:
// // ============================================================================
// // const express = require('express');
// // const cors = require('cors');
// // const mongoose = require('mongoose');
// // const jwt = require('jsonwebtoken');
// // const { Server } = require('socket.io');
// // const http = require('http');
// // 
// // const app = express();
// // const server = http.createServer(app);
// // const io = new Server(server, {
// //   cors: { origin: ['http://localhost:3000'], credentials: true }
// // });
// // 
// // // Middleware
// // app.use(cors({ origin: 'http://localhost:3000', credentials: true }));
// // app.use(express.json());
// // app.use('/uploads', express.static('public/uploads'));
// // 
// // // MongoDB
// // mongoose.connect(process.env.MONGODB_URI || 'mongodb://localhost:27017/messagingapp');
// // 
// // // JWT Middleware
// // const authenticateToken = (req, res, next) => {
// //   const authHeader = req.headers['authorization'];
// //   const token = authHeader && authHeader.split(' ')[1];
// //   if (!token) return res.sendStatus(401);
// //   jwt.verify(token, process.env.JWT_KEY, (err, user) => {
// //     if (err) return res.sendStatus(403);
// //     req.user = user;
// //     next();
// //   });
// // };
// // 
// // // Socket.IO Authentication
// // io.use((socket, next) => {
// //   const token = socket.handshake.query.access_token;
// //   if (!token) return next(new Error('Authentication error'));
// //   jwt.verify(token, process.env.JWT_KEY, (err, decoded) => {
// //     if (err) return next(new Error('Authentication error'));
// //     socket.userId = decoded.sub || decoded.nameid;
// //     next();
// // });
// // 
// // io.on('connection', (socket) => {
// //   console.log('User connected:', socket.userId);
// // });
// // 
// // // Routes
// // app.get('/api/messages', authenticateToken, async (req, res) => {
// //   // Handle request
// // });
// // 
// // server.listen(5000, () => {
// //   console.log('Server running on http://localhost:5000');
// // });
// ============================================================================
