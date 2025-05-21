using Application;
using Infrastructure;
using Infrastructure.Configurations.Filters;
using Infrastructure.Configurations.Middleware;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// 1. Swagger & Controllers
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Conexa Challenge API",
        Version = "v1"
    });
    c.EnableAnnotations();
});

builder.Services
    .AddControllers(options =>
    {
        options.Filters.Add<SuccessResponseFilter>();
    })
    .ConfigureApiBehaviorOptions(options =>
    {
        options.SuppressModelStateInvalidFilter = true;
    })
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

// 2. Infrastructure (EF, Repos, UoW, Authentication & Authorization)
builder.Services.AddInfrastructure(builder.Configuration.GetConnectionString("Default"), builder.Configuration);

// 3. Application (MediatR & PasswordHasher)
builder.Services.AddApplicationLayer();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Conexa API v1");
});

app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Apply any pending migrations
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.Run();
