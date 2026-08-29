using EventManager.Application.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEventManager();
builder.Services.AddControllers();

var app = builder.Build();

app.UseAuthorization();
app.MapControllers();
app.Run();
