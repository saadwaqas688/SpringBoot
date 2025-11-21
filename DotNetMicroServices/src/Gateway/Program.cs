using Gateway.Services;
using Shared.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

// Register Gateway services
builder.Services.AddScoped<ITodoGatewayService, TodoGatewayService>();
builder.Services.AddScoped<IUserGatewayService, UserGatewayService>();

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
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseRouting();
app.UseCors("AllowAll");
app.UseAuthorization();

// Swagger configuration - after routing
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Gateway API v1");
    c.RoutePrefix = "swagger"; // Swagger UI at /swagger
});

// Map endpoints
app.MapGet("/", () => Results.Redirect("/swagger"));
app.MapControllers();

app.Run();
