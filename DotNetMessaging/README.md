# WhatsApp-like Messaging App

A full-stack messaging application built with .NET Core REST APIs and React, featuring real-time communication using SignalR WebSockets.

## Features

- ✅ User Authentication (Register/Login with JWT)
- ✅ Real-time messaging using SignalR
- ✅ One-on-one chats
- ✅ Group chats with member management
- ✅ Online/Offline status
- ✅ Last seen timestamps
- ✅ Message replies
- ✅ Message reactions (emojis)
- ✅ Media sharing (images, videos, audio, documents)
- ✅ Typing indicators
- ✅ Contacts management
- ✅ Message deletion
- ✅ Unread message counts

## Tech Stack

### Backend

- .NET 8.0
- ASP.NET Core Web API
- Entity Framework Core
- SQL Server
- SignalR (WebSockets)
- JWT Authentication
- BCrypt for password hashing

### Frontend

- React 18
- React Router
- SignalR Client
- Axios for HTTP requests
- React Icons
- Date-fns for date formatting

## Project Structure

```
DotNetMessaging/
├── backend/
│   └── DotNetMessaging.API/
│       ├── Controllers/      # REST API controllers
│       ├── Models/           # Database models
│       ├── DTOs/             # Data transfer objects
│       ├── Data/             # DbContext
│       ├── Hubs/             # SignalR hubs
│       ├── Services/         # Business logic services
│       └── Program.cs        # Application entry point
└── frontend/
    └── src/
        ├── components/       # React components
        ├── contexts/        # React contexts
        ├── services/        # API and SignalR services
        └── App.js           # Main app component
```

## Setup Instructions

### Prerequisites

- .NET 8.0 SDK
- Node.js 16+ and npm
- SQL Server (LocalDB or full instance)
- Visual Studio 2022 or VS Code (optional)

### Backend Setup

1. Navigate to the backend directory:

```bash
cd backend/DotNetMessaging.API
```

2. Restore packages:

```bash
dotnet restore
```

3. Update the connection string in `appsettings.json` if needed:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=MessagingApp;Trusted_Connection=True;MultipleActiveResultSets=true"
  }
}
```

4. Run the application:

```bash
dotnet run
```

The API will be available at `http://localhost:5000`

### Frontend Setup

1. Navigate to the frontend directory:

```bash
cd frontend
```

2. Install dependencies:

```bash
npm install
```

3. Create a `.env` file (optional, defaults to localhost:5000):

```env
REACT_APP_API_URL=http://localhost:5000/api
```

4. Start the development server:

```bash
npm start
```

The app will be available at `http://localhost:3000`

## API Endpoints

### Authentication

- `POST /api/auth/register` - Register a new user
- `POST /api/auth/login` - Login user

### Users

- `GET /api/users/me` - Get current user
- `GET /api/users/search?query={query}` - Search users
- `GET /api/users/contacts` - Get user's contacts
- `POST /api/users/contacts/{userId}` - Add contact
- `PUT /api/users/me/status` - Update status

### Chats

- `GET /api/chats` - Get all chats
- `POST /api/chats/with/{userId}` - Get or create chat with user

### Messages

- `GET /api/messages/chat/{chatId}` - Get chat messages
- `GET /api/messages/group/{groupId}` - Get group messages
- `POST /api/messages` - Send message
- `POST /api/messages/{messageId}/reaction` - Add reaction
- `DELETE /api/messages/{messageId}` - Delete message

### Groups

- `GET /api/groups` - Get all groups
- `POST /api/groups` - Create group
- `POST /api/groups/{groupId}/members` - Add members
- `DELETE /api/groups/{groupId}/members/{memberId}` - Remove member

### Media

- `POST /api/media/upload?chatId={id}` - Upload media for chat
- `POST /api/media/upload?groupId={id}` - Upload media for group
- `GET /api/media/download/{fileName}` - Download media

## SignalR Events

### Client → Server

- `JoinChat(chatId)` - Join a chat room
- `LeaveChat(chatId)` - Leave a chat room
- `JoinGroup(groupId)` - Join a group room
- `LeaveGroup(groupId)` - Leave a group room
- `SendTyping(chatId, isTyping)` - Send typing indicator
- `SendGroupTyping(groupId, isTyping)` - Send typing indicator in group

### Server → Client

- `NewMessage(message)` - New message received
- `NewGroupMessage(message)` - New group message received
- `UserOnline(userId)` - User came online
- `UserOffline(userId)` - User went offline
- `UserTyping(chatId, userId, isTyping)` - User typing indicator
- `UserTypingGroup(groupId, userId, isTyping)` - User typing in group
- `MessageReactionUpdated(message)` - Message reaction updated
- `MessageDeleted(messageId)` - Message deleted

## Database Schema

- **Users** - User accounts
- **Chats** - One-on-one conversations
- **Messages** - Chat messages
- **Groups** - Group conversations
- **GroupMembers** - Group membership
- **MessageReactions** - Message reactions
- **Contacts** - User contacts

## Usage

1. Register a new account or login
2. Search for users and add them to contacts
3. Start a chat by clicking on a contact
4. Create groups by clicking "Create Group" button
5. Send messages, reply to messages, add reactions
6. Share media files (images, videos, documents)
7. See online status and last seen timestamps

## Development Notes

- The database is automatically created on first run using `EnsureCreated()`
- JWT tokens expire after 7 days
- Media files are stored in the `wwwroot/uploads` directory
- SignalR connection requires JWT token in query string or header

## License

This project is open source and available for educational purposes.

