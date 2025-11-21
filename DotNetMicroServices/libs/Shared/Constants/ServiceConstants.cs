namespace Shared.Constants;

public static class ServiceConstants
{
    // Default values for local development
    public const string DefaultTodoServiceUrl = "http://localhost:5001";
    public const string DefaultUserServiceUrl = "http://localhost:5002";
    
    public const string TodoServiceName = "TodoService";
    public const string UserServiceName = "UserService";
    public const string GatewayServiceName = "Gateway";
    
    // Configuration keys
    public const string TodoServiceUrlKey = "ServiceUrls:TodoService";
    public const string UserServiceUrlKey = "ServiceUrls:UserService";
}

