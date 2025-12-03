# Service Communication Troubleshooting Guide

## Architecture Overview

Your microservices communicate via **RabbitMQ** using a message queue pattern:

```
Client → Gateway (Port 5000) → RabbitMQ → UserAccountService (Port 5003)
                                  ↓
                            CoursesService (Port 5004)
```

## Common Issues and Solutions

### 1. **RabbitMQ Not Running**

**Check:**

```powershell
docker ps | findstr rabbitmq
```

**Solution:**

```powershell
docker-compose up rabbitmq -d
```

**Verify:** Open http://localhost:15672 (guest/guest) to see RabbitMQ Management UI

---

### 2. **Services Not Starting RabbitMQ Listeners**

**Symptoms:**

- Services start but show warnings about RabbitMQ
- Messages timeout (30 seconds)
- No responses from services

**Check Logs:**
Look for these messages in your service logs:

- ✅ `"RabbitMQ connection established"` - Good!
- ✅ `"Started listening on queue {QueueName}"` - Good!
- ❌ `"Failed to start RabbitMQ listener"` - Problem!

**Solution:**
We've already fixed this with try-catch blocks, but ensure services are running:

- Gateway: Port 5000
- UserAccountService: Port 5003
- CoursesService: Port 5004

---

### 3. **Queue Bindings Not Matching**

**How It Works:**

- Gateway sends messages with routing keys like `useraccount.signup`, `courses.health-check`
- Services bind their queues to routing key patterns like `useraccount.*`, `courses.*`

**Verify in RabbitMQ UI:**

1. Open http://localhost:15672
2. Go to **Exchanges** → `microservices_exchange`
3. Check **Bindings** - you should see:
   - `USER_ACCOUNT_SERVICE_QUEUE` bound to `useraccount.*`
   - `COURSES_SERVICE_QUEUE` bound to `courses.*`, `lessons.*`, etc.

---

### 4. **Services Not Running**

**Check if services are running:**

```powershell
# Check if ports are in use
netstat -ano | findstr ":5000"
netstat -ano | findstr ":5003"
netstat -ano | findstr ":5004"
```

**Start services:**

```powershell
# In separate terminals or using dotnet watch
cd src/Gateway && dotnet run
cd src/UserAccountService && dotnet run
cd src/CoursesService && dotnet run
```

---

### 5. **Message Timeout Issues**

**Default timeout:** 30 seconds

**Symptoms:**

- Gateway sends message but gets `null` response
- Logs show "Timeout waiting for response"

**Possible Causes:**

1. **Service not listening** - Check service logs for listener startup
2. **Message handler error** - Check service logs for exceptions
3. **Routing key mismatch** - Verify routing keys match constants

**Debug:**
Check service logs when sending a message:

```csharp
// In UserAccountService or CoursesService logs, you should see:
"Handling message with routing key: useraccount.signup"
```

---

### 6. **Configuration Issues**

**Verify RabbitMQ Configuration:**

All services should have in `appsettings.json`:

```json
{
  "RabbitMQ": {
    "HostName": "localhost",
    "Port": "5672",
    "UserName": "guest",
    "Password": "guest"
  }
}
```

**If using Docker Compose:**

- Services in Docker should use `rabbitmq` as hostname
- Services running locally should use `localhost`

---

## Step-by-Step Diagnostic Process

### Step 1: Verify RabbitMQ is Running

```powershell
docker ps --filter "name=rabbitmq"
```

Should show `rabbitmq` container running.

### Step 2: Check RabbitMQ Management UI

1. Open http://localhost:15672
2. Login: `guest` / `guest`
3. Go to **Queues** tab
4. You should see:
   - `USER_ACCOUNT_SERVICE_QUEUE`
   - `COURSES_SERVICE_QUEUE`

### Step 3: Verify Services Are Running

Check each service:

- Gateway: http://localhost:5000/swagger
- UserAccountService: http://localhost:5003/swagger
- CoursesService: http://localhost:5004/swagger

### Step 4: Test Communication

**Test UserAccountService:**

```bash
# From Gateway Swagger, try:
POST /api/auth/signup
{
  "name": "Test User",
  "email": "test@example.com",
  "password": "Test123!"
}
```

**Check Logs:**

- Gateway logs should show: "Message sent to queue..."
- UserAccountService logs should show: "Handling message with routing key: useraccount.signup"

### Step 5: Check for Errors

**In Gateway logs, look for:**

- `"Error calling UserAccountService for signup"` - Service not responding
- `"Timeout waiting for response"` - Service not processing message

**In Service logs, look for:**

- `"Error handling message"` - Handler error
- `"Failed to start RabbitMQ listener"` - Connection issue

---

## Quick Fixes

### Fix 1: Restart RabbitMQ

```powershell
docker restart rabbitmq
```

### Fix 2: Restart All Services

Stop and restart all .NET services to re-establish connections.

### Fix 3: Clear RabbitMQ Queues

1. Open RabbitMQ Management UI
2. Go to **Queues**
3. Delete and recreate queues (services will recreate them)

### Fix 4: Check Service Logs

Enable detailed logging:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Shared.Services": "Debug"
    }
  }
}
```

---

## Testing Communication

### Test 1: Health Check (Simplest)

```bash
# From Gateway Swagger
GET /api/courses/health-check
```

Should return: `"Health check successful"`

### Test 2: Sign Up

```bash
POST /api/auth/signup
{
  "name": "Test",
  "email": "test@test.com",
  "password": "Test123!"
}
```

### Test 3: Check RabbitMQ UI

1. Go to **Queues** → Select a queue
2. Click **Get messages**
3. You should see message flow

---

## Expected Behavior

### When Working Correctly:

1. **Gateway sends message:**

   ```
   Gateway → RabbitMQ Exchange (microservices_exchange)
            → Routing Key: useraccount.signup
   ```

2. **RabbitMQ routes message:**

   ```
   Exchange → USER_ACCOUNT_SERVICE_QUEUE (bound to useraccount.*)
   ```

3. **Service receives and processes:**

   ```
   UserAccountService → HandleMessage() → HandleSignUp()
   ```

4. **Service sends response:**

   ```
   UserAccountService → Reply Queue → Gateway
   ```

5. **Gateway receives response:**
   ```
   Gateway → Returns to client
   ```

---

## Still Not Working?

1. **Check all service logs simultaneously**
2. **Verify RabbitMQ Management UI shows queues and bindings**
3. **Test with a simple health check first**
4. **Ensure all services are using the same RabbitMQ instance**
5. **Check firewall/network issues**

---

## Contact Points

- **RabbitMQ Management:** http://localhost:15672
- **Gateway Swagger:** http://localhost:5000/swagger
- **UserAccountService Swagger:** http://localhost:5003/swagger
- **CoursesService Swagger:** http://localhost:5004/swagger
