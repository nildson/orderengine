using System.Text;
using System.Text.Json.Serialization;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OrderEngine.Application;
using OrderEngine.Infrastructure;

namespace OrderEngine.Api.Extensions;

public static class BuilderExtensions
{
    public static WebApplicationBuilder AddOrderEngine(this WebApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=OrderEngine.db";
        var jwtKey = builder.Configuration["Jwt:Key"]
            ?? Environment.GetEnvironmentVariable("ORDERENGINE_JWT_KEY")
            ?? throw new InvalidOperationException("ORDERENGINE_JWT_KEY is not configured.");
        var issuer = builder.Configuration["Jwt:Issuer"]
            ?? Environment.GetEnvironmentVariable("ORDERENGINE_JWT_ISSUER")
            ?? "OrderEngine";
        var audience = builder.Configuration["Jwt:Audience"]
            ?? Environment.GetEnvironmentVariable("ORDERENGINE_JWT_AUDIENCE")
            ?? "OrderEngineAudience";

        builder.Services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });

        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = issuer,
                ValidAudience = audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
                ClockSkew = TimeSpan.FromMinutes(1)
            };
        });

        builder.Services.AddAuthorization();
        builder.Services.AddDbContext<OrderDbContext>(options =>
            options.UseSqlite(connectionString));

        builder.Services.AddOpenTelemetry()
            .ConfigureResource(resource => resource.AddService(serviceName: "OrderEngine.Api", serviceVersion: "1.0.0"))
            .WithTracing(tracing => tracing
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddConsoleExporter())
            .WithMetrics(metrics => metrics
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddConsoleExporter());

        builder.Services.AddOpenApi();
        builder.Services.AddValidatorsFromAssembly(typeof(CreateOrderCommand).Assembly);
        builder.Services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(CreateOrderCommand).Assembly);
            cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });
        builder.Services.AddScoped<IOrderRepository, EfCoreOrderRepository>();
        builder.Services.AddScoped<IOrderService, OrderService>();

        return builder;
    }
}
