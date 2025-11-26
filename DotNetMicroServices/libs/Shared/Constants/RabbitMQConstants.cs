namespace Shared.Constants;

public static class RabbitMQConstants
{
    // Queue Names
    public const string TodoServiceQueue = "TODO_SERVICE_QUEUE";
    public const string UserServiceQueue = "USER_SERVICE_QUEUE";
    public const string UserAccountServiceQueue = "USER_ACCOUNT_SERVICE_QUEUE";
    
    // Message Patterns - Todo Service
    public static class Todo
    {
        public const string HealthCheck = "todo.health-check";
        public const string GetAll = "todo.get-all";
        public const string GetById = "todo.get-by-id";
        public const string Create = "todo.create";
        public const string Update = "todo.update";
        public const string Delete = "todo.delete";
    }
    
    // Message Patterns - User Service
    public static class User
    {
        public const string HealthCheck = "user.health-check";
        public const string GetAll = "user.get-all";
        public const string GetById = "user.get-by-id";
        public const string GetByEmail = "user.get-by-email";
        public const string Create = "user.create";
        public const string Update = "user.update";
        public const string Delete = "user.delete";
    }
    
    // Message Patterns - UserAccount Service
    public static class UserAccount
    {
        public const string HealthCheck = "useraccount.health-check";
        public const string SignUp = "useraccount.signup";
        public const string SignIn = "useraccount.signin";
        public const string UpdateProfile = "useraccount.update-profile";
        public const string GetCurrentUser = "useraccount.get-current-user";
    }
}



