using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using System.Diagnostics.CodeAnalysis;
using Mini.Common.Settings;
using Mini.Common.Helpers;
using Mini.Common.Extensions;
using System.Security.Cryptography;
using Recep.Models;
using Microsoft.IdentityModel.Tokens;

namespace Recep.Services;

[ExcludeFromCodeCoverage]
static internal class AppStartup
{
    static internal void ConfigureWebHost(ConfigureWebHostBuilder webHost)
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

    static internal void SetupOptions(ConfigurationManager configuration, IServiceCollection services)
    {
        services.Configure<RsaKeySetting>(RsaKeyName.SigningKey, configuration.GetSection(RsaKeyName.SigningKey));

        //services.Configure<RsaKeySetting2>(RsaKeyName.SigningKey, option =>
        //{
        //    var s = configuration.GetSection(RsaKeyName.SigningKey);
        //    s.Bind("", opt =>
        //    {
                
        //    });
        //    //configuration.GetSection(RsaKeyName.SigningKey));

        //});

        services.Configure<RsaKeySetting>(RsaKeyName.EncryptingKey, configuration.GetSection(RsaKeyName.EncryptingKey));

        services.Configure<ApplicationSetting>(configuration.GetSection("Application"));

        foreach (var item in configuration.GetSectionChildren("Jwt"))
            services.Configure<JwtSetting>(item.Key, item); // Example: item.Key = "Conso"
    }

    static internal void SetupAuthentication(ConfigurationManager configuration, IServiceCollection services)
    {
        var jwtSetting = configuration.GetSectionAs<JwtSetting>("Jwt:Conso");

        var applicationSetting = configuration.GetSectionAs<ApplicationSetting>("Application");

        string secretKey = EnvironmentHelper.GetVariable(applicationSetting.Name);

        (var scKey, var ecKey) = SecurityKeyHelper.SymmetricSecurityKey(secretKey, HashAlgorithmName.SHA256);

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
        {
            options.Events = new JwtBearerEvents
            {
                OnAuthenticationFailed = ctx =>
                {
                    ctx.NoResult();
                    ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return Task.CompletedTask;
                },
            };

            // Signing key needs to have private key
            //var signingCredentialSecurityKey = rsaSigningKeySetting.GetRsaSecurityKey(true);

            //var securityCredential = value.Encrypting.Value;

            //using RSA encryptingKeyRsa = RSA.Create();

            //encryptingKeyRsa.FromXmlString(securityCredential.Xml);

            //var encryptingCredentialSecurityKey = new RsaSecurityKey(encryptingKeyRsa);

            options.TokenValidationParameters = new()
            {
                RequireSignedTokens = true,
                RequireExpirationTime = true,
                RequireAudience = true,

                //ValidateIssuerSigningKey = true,
                ValidateLifetime = true,
                ValidateIssuer = true,
                ValidateAudience = true,

                // TokenValidator can accept multiple issuers, and audiences via ValidIssuers and ValidAudiences.
                // If we want more dynamicity, we should use the respective validator delegates.
                // ValidIssuers = new string[] { "https://localhost:5001" },
                // ValidAudiences = new string[] { "mini-console-app", "mini-console-app2" },
                // For simplicity, we go with a single Issuer and Audience

                ValidIssuer = jwtSetting.Issuer,
                ValidAudience = jwtSetting.Audience,
                IssuerSigningKey = scKey,
                TokenDecryptionKey = ecKey,
                //TokenDecryptionKeyResolver = (token, securityToken, kid, validationParameters) =>
                //{
                //    // string token, SecurityToken securityToken, string kid, TokenValidationParameters validationParameters
                //    List<SecurityKey> keys = new List<SecurityKey>();
                //    keys.Add(new RsaSecurityKey(RSA.Create()));
                //    return keys;
                //}
            };
        });
    }

    static internal void SetupAuthorization(IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.FallbackPolicy = new AuthorizationPolicyBuilder()
            .AddAuthenticationSchemes(
                //CookieAuthenticationDefaults.AuthenticationScheme,
                JwtBearerDefaults.AuthenticationScheme)
            .RequireAuthenticatedUser()
            .Build();
        });
    }

    static internal void SetupServices(IServiceCollection services)
    {
        // Add services to the container.
        services.AddControllers();

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

    }
}

public static class RsaKeyName
{
    public const string SigningKey = "RsaKeys:SigningKey";
    public const string EncryptingKey = "RsaKeys:EncryptingKey";
}

