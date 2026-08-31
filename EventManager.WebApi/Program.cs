using EventManager.Application.Services;
using EventManager.WebApi.Configuration;
using EventManager.WebApi.Extensions;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment())
{
    builder.Host.UseDefaultServiceProvider(options =>
    {
        options.ValidateScopes = true;
        options.ValidateOnBuild = true;
    });
}

var corsSettings = builder.Configuration.GetSection(CorsSettings.SectionName).Get<CorsSettings>();
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
        policy.WithOrigins(corsSettings?.Origins?.Split(',') ?? [])
              .WithMethods(corsSettings?.Methods?.Split(',') ?? [])
              .WithHeaders(corsSettings?.Headers?.Split(',') ?? [])
              .AllowCredentials();
    });
});
builder.Services.AddEventManager();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseErrorHandling();

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

app.MapControllers();
app.Run();
