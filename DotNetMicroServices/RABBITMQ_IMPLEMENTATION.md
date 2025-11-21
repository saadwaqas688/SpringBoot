# RabbitMQ Implementation Guide

This document explains the RabbitMQ implementation in the .NET microservices project, following the same pattern as the NestJS project.

## Overview

RabbitMQ has been integrated to enable asynchronous message-based communication between services, similar to the NestJS implementation using `@nestjs/microservices`.

## Implementation Status

### ✅ Completed

1. **RabbitMQ Docker Service** - Added to docker-compose.yml
2. **RabbitMQ.Client Package** - Added to Shared library
3. **RabbitMQ Constants** - Message patterns defined in `RabbitMQConstants.cs`
4. **RabbitMQ Service** - Core service for sending/receiving messages
5. **TodoService Integration** - RabbitMQ listener implemented

### 🔄 Remaining Tasks

1. **UserService Integration** - Add RabbitMQ listener (similar to TodoService)
2. **Gateway Update** - Update Gateway services to use RabbitMQ instead of HTTP
3. **Message Handler Updates** - Fix message parsing in handlers
4. **Configuration** - Add RabbitMQ config to all appsettings.json files

## Architecture

```
Gateway → RabbitMQ → TodoService
         → RabbitMQ → UserService
```

## Key Files

### Shared Library

- `libs/Shared/Constants/RabbitMQConstants.cs` - Message patterns
- `libs/Shared/Services/IRabbitMQService.cs` - Interface
- `libs/Shared/Services/RabbitMQService.cs` - Implementation

### TodoService

- `src/TodoService/Services/TodoMessageHandler.cs` - Message handler
- `src/TodoService/Program.cs` - RabbitMQ listener setup

## Next Steps

1. Complete UserService RabbitMQ integration
2. Update Gateway to use RabbitMQ
3. Test end-to-end flow
4. Update documentation

## Usage Example

### Sending a Message (Gateway)

```csharp
var response = await _rabbitMQService.SendMessageAsync<ApiResponse<List<Todo>>>(
    RabbitMQConstants.TodoServiceQueue,
    RabbitMQConstants.Todo.GetAll,
    new { UserId = userId }
);
```

### Receiving a Message (TodoService)

Messages are automatically handled by `TodoMessageHandler` based on routing keys.


