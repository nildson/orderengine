using System.Text.Json.Serialization;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OrderEngine.Api.Auth;
using OrderEngine.Api.Endpoints;
using OrderEngine.Api.Extensions;
using OrderEngine.Infrastructure;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog();
builder.AddOrderEngine();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<OrderDbContext>();
    dbContext.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapPost("/api/auth/login", (LoginRequest request) =>
{
    if (!JwtTokenService.ValidateCredentials(request.Email, request.Password))
    {
        return Results.Unauthorized();
    }

    var token = JwtTokenService.CreateToken(
        request.Email,
        builder.Configuration["Jwt:Issuer"]
            ?? Environment.GetEnvironmentVariable("ORDERENGINE_JWT_ISSUER")
            ?? "OrderEngine",
        builder.Configuration["Jwt:Audience"]
            ?? Environment.GetEnvironmentVariable("ORDERENGINE_JWT_AUDIENCE")
            ?? "OrderEngineAudience",
        builder.Configuration["Jwt:Key"]
            ?? Environment.GetEnvironmentVariable("ORDERENGINE_JWT_KEY")
            ?? throw new InvalidOperationException("ORDERENGINE_JWT_KEY is not configured."));

    return Results.Ok(new { token });
}).AllowAnonymous();

app.MapOrderEndpoints();

app.Run();

public partial class Program { }

public record LoginRequest(
    [property: JsonPropertyName("email")] string Email,
    [property: JsonPropertyName("password")] string Password);
