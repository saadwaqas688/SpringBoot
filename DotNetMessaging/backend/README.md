# WhatsApp-like Messaging App - Backend API

A WhatsApp-like messaging application built with ASP.NET Core, MongoDB, and SignalR.

## Features

- ✅ User authentication (JWT)
- ✅ One-on-one chat
- ✅ Group chat
- ✅ Contact management
- ✅ Message replies
- ✅ Media uploads (images, videos, documents)
- ✅ Message reactions
- ✅ Group administration (add/remove members, admin roles)
- ✅ Real-time messaging with SignalR
- ✅ Online/offline status

## Prerequisites

1. **.NET 8.0 SDK** - [Download here](https://dotnet.microsoft.com/download/dotnet/8.0)
2. **MongoDB** - [Download here](https://www.mongodb.com/try/download/community) or use MongoDB Atlas (cloud)

## Setup Instructions

### Option 1: Local MongoDB

1. **Install MongoDB Community Server**

   - Download and install from [MongoDB Download Center](https://www.mongodb.com/try/download/community)
   - Follow installation wizard
   - MongoDB will run as a service on port 27017 by default

2. **Verify MongoDB is running**

   ```bash
   # Windows (PowerShell)
   Get-Service MongoDB

   # Or check if MongoDB is listening on port 27017
   netstat -an | findstr 27017
   ```

### Option 2: MongoDB Atlas (Cloud - Recommended for quick start)

1. Create a free account at [MongoDB Atlas](https://www.mongodb.com/cloud/atlas)
2. Create a cluster (free tier available)
3. Get your connection string
4. Update `appsettings.json` with your connection string

## Running the Application

### Step 1: Navigate to the project directory

```bash
cd DotNetMessaging.API
```

### Step 2: Restore dependencies

```bash
dotnet restore
```

### Step 3: Configure MongoDB connection (if needed)

Edit `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "MongoDB": "mongodb://localhost:27017"
  },
  "MongoDB": {
    "DatabaseName": "messagingapp"
  }
}
```

For MongoDB Atlas, use:

```json
{
  "ConnectionStrings": {
    "MongoDB": "mongodb+srv://username:password@cluster.mongodb.net/?retryWrites=true&w=majority"
  }
}
```

### Step 4: Create uploads directory (for media files)

```bash
# Windows (PowerShell)
mkdir wwwroot\uploads\image
mkdir wwwroot\uploads\video
mkdir wwwroot\uploads\audio
mkdir wwwroot\uploads\document

# Linux/Mac
mkdir -p wwwroot/uploads/{image,video,audio,document}
```

### Step 5: Run the application

```bash
dotnet run
```

The API will start on `http://localhost:5000`

## Testing the API

### Swagger UI

When running in Development mode, Swagger UI is available at:

- **Swagger UI**: `http://localhost:5000/swagger`

### API Endpoints

#### Authentication

- `POST /api/auth/register` - Register a new user
- `POST /api/auth/login` - Login and get JWT token

#### Chats

- `GET /api/chats` - Get all user chats
- `POST /api/chats/with/{userId}` - Get or create a chat with a user

#### Messages

- `GET /api/messages/chat/{chatId}` - Get chat messages
- `GET /api/messages/group/{groupId}` - Get group messages
- `POST /api/messages` - Send a message
- `POST /api/messages/{messageId}/reaction` - Add/remove reaction
- `DELETE /api/messages/{messageId}` - Delete a message

#### Groups

- `GET /api/groups` - Get user's groups
- `POST /api/groups` - Create a group
- `POST /api/groups/{groupId}/members` - Add members
- `DELETE /api/groups/{groupId}/members/{memberId}` - Remove member
- `PUT /api/groups/{groupId}/members/{memberId}/role` - Update member role

#### Users

- `GET /api/users/me` - Get current user
- `GET /api/users/search?query=...` - Search users
- `GET /api/users/contacts` - Get contacts
- `POST /api/users/contacts/{contactUserId}` - Add contact
- `PUT /api/users/me/status` - Update status

#### Media

- `POST /api/media/upload` - Upload media file (multipart/form-data)

### SignalR Hub

- **Hub URL**: `http://localhost:5000/chathub`
- Connect with JWT token in query string: `?access_token=YOUR_TOKEN`

## Example API Usage

### 1. Register a user

```bash
curl -X POST http://localhost:5000/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "username": "john_doe",
    "email": "john@example.com",
    "password": "password123"
  }'
```

### 2. Login

```bash
curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "john@example.com",
    "password": "password123"
  }'
```

### 3. Get chats (with JWT token)

```bash
curl -X GET http://localhost:5000/api/chats \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"
```

## Configuration

Key configuration in `appsettings.json`:

- `ConnectionStrings:MongoDB` - MongoDB connection string
- `MongoDB:DatabaseName` - Database name (default: "messagingapp")
- `Jwt:Key` - Secret key for JWT tokens (should be at least 32 characters)
- `Jwt:Issuer` - JWT issuer
- `Jwt:Audience` - JWT audience

## CORS

The API is configured to allow requests from `http://localhost:3000` (React app). To change this, update the CORS policy in `Program.cs`.

## Project Structure

```
DotNetMessaging.API/
├── Controllers/          # API Controllers
├── Services/            # Business logic layer
├── Repositories/        # Data access layer
├── Models/              # Domain models
├── DTOs/                # Data transfer objects
├── Data/                # MongoDB context
├── Hubs/                # SignalR hubs
└── wwwroot/            # Static files (media uploads)
```

## Architecture

- **Clean Architecture**: Controllers → Services → Repositories → MongoDB
- **Base Repository**: All repositories extend base repository with pagination and aggregation
- **MongoDB Driver**: Using official .NET MongoDB driver for all database operations

## Troubleshooting

### MongoDB Connection Issues

- Ensure MongoDB is running: `mongosh` or check MongoDB service
- Verify connection string in `appsettings.json`
- Check firewall settings if using remote MongoDB

### Port Already in Use

- Change the port in `Program.cs`: `app.Run("http://localhost:YOUR_PORT");`

### Media Upload Fails

- Ensure `wwwroot/uploads` directory exists with write permissions
- Check file size (max 50MB)

## License

This project is for educational purposes.
