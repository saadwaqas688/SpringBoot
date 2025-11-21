using TodoService.Services;
using Shared.Services;
using Shared.Constants;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register services
builder.Services.AddScoped<ITodoService, TodoService.Services.TodoService>();
builder.Services.AddScoped<TodoMessageHandler>();

// Register RabbitMQ Service
var rabbitMQConfig = builder.Configuration.GetSection("RabbitMQ");
builder.Services.AddSingleton<IRabbitMQService>(sp =>
{
    var logger = sp.GetRequiredService<ILogger<RabbitMQService>>();
    return new RabbitMQService(
        rabbitMQConfig["HostName"] ?? "localhost",
        int.Parse(rabbitMQConfig["Port"] ?? "5672"),
        rabbitMQConfig["UserName"] ?? "guest",
        rabbitMQConfig["Password"] ?? "guest",
        logger);
});

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

// Start RabbitMQ listener
var rabbitMQService = app.Services.GetRequiredService<IRabbitMQService>();

rabbitMQService.StartListening(RabbitMQConstants.TodoServiceQueue, async (messageJson, routingKey) =>
{
    // Create a scope for each message to resolve scoped services
    using var scope = app.Services.CreateScope();
    var messageHandler = scope.ServiceProvider.GetRequiredService<TodoMessageHandler>();
    return await messageHandler.HandleMessage(messageJson, routingKey);
});

// Run the application
app.Run();
