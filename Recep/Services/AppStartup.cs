using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Recep.Services;

[ExcludeFromCodeCoverage]
internal static class AppStartup
{

    internal static void SetupServices(IServiceCollection services)
    {
        // Add services to the container.
        services.AddControllers();
        
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

    }

    internal static void SetupAuthentication(ConfigurationManager configuration, IServiceCollection services)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
        {
            options.Events = new JwtBearerEvents
            {
                OnAuthenticationFailed = ctx =>
                {
                    ctx.NoResult();
                    ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return Task.CompletedTask;
                }
            };

            //options.Authority = $"https://{Configuration["Auth0:Domain"]}/";
            //options.Audience = Configuration["Auth0:Audience"];

            //options.TokenValidationParameters = new()
            //{
            //    ValidateIssuer = true,
            //    ValidateAudience = true,
            //    ValidateLifetime = true,
            //    ValidateIssuerSigningKey = true,
            //    ValidIssuer = configuration["Jwt:ValidIssuer"],
            //    ValidAudience = configuration["Jwt:ValidIssuer"],
            //    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:SecretKey"]))
            //};
        });
    }

    internal static void ConfigureWebHost(ConfigureWebHostBuilder webHost)
    {
        webHost.ConfigureKestrel(options =>
        {
            // Not all KestrelServerOptions can be configured from appsettings.json
            // Unfortunately, AddServerHeader is one of them.
            // The rationale is that KestrelServerOptions configurable from appsettings.json
            // are settings that makes more sense to be configured during runtime.
            options.AddServerHeader = false;
        });
    }
}
