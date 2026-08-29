using EventManager.Application.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
    });
    options.AddPolicy("Production", policy =>
    {
        policy.WithOrigins("https://event-manager.com")
              .WithMethods("GET", "POST", "PUT", "DELETE")
              .WithHeaders("Content-Type", "Authorization", "X-Requested-With")
              .AllowCredentials();
    });
});
builder.Services.AddEventManager();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors("AllowAll");
}
else
{
    app.UseCors("Production");
}

app.UseAuthorization();
app.MapControllers();
app.Run();
