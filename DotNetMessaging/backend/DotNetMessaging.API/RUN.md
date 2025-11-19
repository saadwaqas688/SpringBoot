# Quick Start Guide

## Prerequisites Check

1. **Check .NET SDK version**

   ```bash
   dotnet --version
   ```

   Should be 8.0 or higher

2. **Check MongoDB**

   ```bash
   # Windows
   mongosh --version

   # Or check if service is running
   Get-Service MongoDB
   ```

## Quick Start (5 minutes)

### 1. Start MongoDB (if using local)

**Windows:**

```powershell
# MongoDB should start automatically as a service
# If not, start it manually:
net start MongoDB
```

**Linux/Mac:**

```bash
# Start MongoDB service
sudo systemctl start mongod
# or
brew services start mongodb-community
```

### 2. Navigate to project

```bash
cd DotNetMessaging.API
```

### 3. Create uploads folder

```powershell
# Windows PowerShell
New-Item -ItemType Directory -Force -Path "wwwroot\uploads\image"
New-Item -ItemType Directory -Force -Path "wwwroot\uploads\video"
New-Item -ItemType Directory -Force -Path "wwwroot\uploads\audio"
New-Item -ItemType Directory -Force -Path "wwwroot\uploads\document"
```

### 4. Run the application

```bash
dotnet run
```

### 5. Open Swagger

Navigate to: http://localhost:5000/swagger

## First Steps

1. **Register a user** via `/api/auth/register`
2. **Login** via `/api/auth/login` to get JWT token
3. **Use the token** in Authorization header: `Bearer YOUR_TOKEN`
4. **Test endpoints** via Swagger UI

## Common Issues

**"MongoDB connection failed"**

- Make sure MongoDB is running
- Check connection string in `appsettings.json`
- Try: `mongosh` to test connection

**"Port 5000 already in use"**

- Change port in `Program.cs` last line
- Or kill the process using port 5000

**"Upload directory not found"**

- Create the `wwwroot/uploads` directories manually
- Or the app will create them on first upload
