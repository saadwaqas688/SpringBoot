# Spring Boot Messaging API

A WhatsApp-like messaging application built with Spring Boot, MongoDB, and WebSocket for real-time communication.

## Features

- User authentication (JWT)
- One-on-one chat
- Group chat
- Message reactions
- Message replies
- Media uploads (images, videos, documents)
- Online/offline status
- Unread message count
- Contact management
- Group administration (add/remove members, admin roles)

## Tech Stack

- **Spring Boot 3.2.0**
- **MongoDB** (Spring Data MongoDB)
- **Spring WebSocket** (for real-time messaging)
- **Spring Security** (JWT authentication)
- **JJWT** (JWT library)
- **Lombok** (reduces boilerplate)
- **Swagger/OpenAPI** (API documentation)

## Project Structure

```
src/main/java/com/messaging/api/
├── models/          # MongoDB entities
├── repositories/    # Data access layer
├── services/        # Business logic
├── controllers/     # REST API endpoints
├── dtos/            # Data transfer objects
├── config/          # Configuration classes
├── security/        # Security configuration
└── websocket/       # WebSocket handlers
```

## Setup Instructions

### Prerequisites

- Java 17 or higher
- Maven 3.6+
- MongoDB (running on localhost:27017)

### Installation

1. Clone the repository
2. Ensure MongoDB is running
3. Update `application.properties` if needed (MongoDB connection, JWT secret)
4. Build the project:
   ```bash
   mvn clean install
   ```
5. Run the application:
   ```bash
   mvn spring-boot:run
   ```

The API will be available at `http://localhost:8080`

## API Documentation

Once the application is running, access Swagger UI at:

- `http://localhost:8080/swagger-ui.html`

## Configuration

### MongoDB

- Default connection: `mongodb://localhost:27017`
- Database name: `messagingapp`

### JWT

- Secret key: Configure in `application.properties`
- Token expiration: 7 days

### File Uploads

- Max file size: 50MB
- Upload directory: `src/main/resources/static/uploads/`

## API Endpoints

### Authentication

- `POST /api/auth/register` - Register a new user
- `POST /api/auth/login` - Login and get JWT token

### Chats

- `GET /api/chats` - Get user's chats
- `POST /api/chats` - Create or get chat with another user

### Messages

- `GET /api/messages/chat/{chatId}` - Get chat messages
- `GET /api/messages/group/{groupId}` - Get group messages
- `POST /api/messages` - Send a message
- `DELETE /api/messages/{messageId}` - Delete a message
- `POST /api/messages/{messageId}/reaction` - Add/remove reaction

### Groups

- `GET /api/groups` - Get user's groups
- `POST /api/groups` - Create a group
- `GET /api/groups/{groupId}` - Get group details
- `POST /api/groups/{groupId}/members` - Add members
- `DELETE /api/groups/{groupId}/members/{memberId}` - Remove member

### Media

- `POST /api/media/upload` - Upload media file

### Users

- `GET /api/users` - Search users
- `GET /api/users/{userId}` - Get user details
- `PUT /api/users/profile` - Update profile
- `PUT /api/users/online-status` - Update online status

### Contacts

- `GET /api/contacts` - Get user's contacts
- `POST /api/contacts` - Add contact
- `DELETE /api/contacts/{contactId}` - Remove contact

## WebSocket

WebSocket endpoint: `/ws/chat`

### Events

**Client to Server:**

- `JoinChat` - Join a chat room
- `LeaveChat` - Leave a chat room
- `JoinGroup` - Join a group room
- `LeaveGroup` - Leave a group room
- `SendTyping` - Send typing indicator
- `SendGroupTyping` - Send typing indicator in group

**Server to Client:**

- `NewMessage` - New message received
- `NewGroupMessage` - New group message received
- `MessageDeleted` - Message deleted
- `MessageReactionUpdated` - Message reaction updated
- `UserTyping` - User is typing
- `UserTypingGroup` - User is typing in group
- `UserOnline` - User came online
- `UserOffline` - User went offline

## Notes

This is a Spring Boot implementation matching the structure and features of the .NET Core version. Some service implementations may need to be completed based on specific business requirements.
