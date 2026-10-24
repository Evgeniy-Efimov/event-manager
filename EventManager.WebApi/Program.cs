using EventManager.Application.Services;
using EventManager.Domain.DbContexts;
using EventManager.WebApi.Configuration;
using EventManager.WebApi.Handlers;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

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
builder.Services.AddEventManager(builder.Configuration);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options => options.IncludeXmlComments(Assembly.GetExecutingAssembly()));
builder.Services.AddExceptionHandler<ExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        context.Database.Migrate();
    }

    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors("AllowAll");
}
else
{
    app.UseCors("Production");
}

app.UseExceptionHandler();
app.MapControllers();
app.Run();
