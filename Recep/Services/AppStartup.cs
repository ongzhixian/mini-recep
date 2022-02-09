using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using System.Diagnostics.CodeAnalysis;
using Mini.Common.Settings;
using Mini.Common.Helpers;
using Mini.Common.Extensions;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using Mini.Common.Services;
using Mini.Wms.MongoDbImplementation.Services;
using Mini.Wms.Abstraction.Services;
using MongoDB.Driver;
using Mini.Wms.Abstraction.Models;
using MongoDB.Bson.Serialization;
using Mini.Wms.MongoDbImplementation.Models;

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

        services.Configure<RsaKeySetting>(RsaKeyName.EncryptingKey, configuration.GetSection(RsaKeyName.EncryptingKey));

        services.Configure<ApplicationSetting>(configuration.GetSection("Application"));

        foreach (var item in configuration.GetSectionChildren("Jwt"))
            services.Configure<JwtSetting>(item.Key, item); // Example: item.Key = "Conso"
    }

    static internal async Task SetupAuthenticationAsync(ConfigurationManager configuration, IServiceCollection services)
    {
        var jwtSetting = configuration.GetSectionAs<JwtSetting>("Jwt:Conso");

        var applicationSetting = configuration.GetSectionAs<ApplicationSetting>("Application");

        string secretKey = EnvironmentHelper.GetVariable(applicationSetting.Name);

        (var symmetricSigningKey, var symmetricEncryptingKey) = SecurityKeyHelper.SymmetricSecurityKey(secretKey, HashAlgorithmName.SHA256);

        // Note: Signing key needs to be private key

        var rsaSigningSecurityKey = await configuration.GetSectionAs<RsaKeySetting>(RsaKeyName.SigningKey)
            .GetRsaSecurityKeyAsync(true);

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

            // Signing key needs to have private key

            options.TokenValidationParameters = new()
            {
                RequireSignedTokens = true,
                RequireExpirationTime = true,
                RequireAudience = true,

                ValidateIssuerSigningKey = true,
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

                // IssuerSigningKey and TokenDecryptionKey only accept a single SecurityKey
                // IssuerSigningKey = scKey,
                // TokenDecryptionKey = ecKey,
                // To configure for multiple keys use: IssuerSigningKeys, TokenDecryptionKeys

                IssuerSigningKeys = new List<SecurityKey>()
                {
                    rsaSigningSecurityKey, 
                    symmetricSigningKey
                }

#pragma warning disable S125 // Sections of code should not be commented out

                // If we do not want to use a fixed list of IssuerSigningKeys/TokenDecryptionKeys
                // We can make use IssuerSigningKeyResolver/ TokenDecryptionKeyResolver

                //IssuerSigningKeyResolver = (token, securityToken, kid, validationParameters) =>
                //{
                //    JwtSecurityToken? jwtSecurityToken = securityToken as JwtSecurityToken;
                //    // jwtSecurityToken.SignatureAlgorithm
                //    IList<SecurityKey> keys = new List<SecurityKey>();
                //    keys.Add(rsaSigningSecurityKey);
                //    return keys;
                //}

                //TokenDecryptionKeyResolver = (token, securityToken, kid, validationParameters) =>
                //{
                //    // string token, SecurityToken securityToken, string kid, TokenValidationParameters validationParameters
                //    List<SecurityKey> keys = new List<SecurityKey>();
                //    keys.Add(new RsaSecurityKey(RSA.Create()));
                //    return keys;
                //}

#pragma warning restore S125 // Sections of code should not be commented out

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

    static internal void SetupServices(ConfigurationManager configuration, IServiceCollection services)
    {
        // Add services to the container.
        services.AddControllers();

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        services.AddScoped<IPkedService, PkedService>();

        services.AddScoped<IUserService<string, User>, UserService>();

        string miniToolsConnectionString = configuration.GetValue<string>("mongodb:minitools:ConnectionString");

        SetupMongoDbServices(services, miniToolsConnectionString);

    }

    private static void SetupMongoDbServices(IServiceCollection services, string miniToolsConnectionString)
    {
        // Registering MongoDb model discriminators (do it before you access DB)
        BsonClassMap.RegisterClassMap<User>();

        services.AddSingleton<IMongoDatabase>(sp =>
        {
            IMongoClient mongoClient = new MongoClient(miniToolsConnectionString);
            string databaseName = MongoUrl.Create(miniToolsConnectionString).DatabaseName;
            return mongoClient.GetDatabase(databaseName);
        });

        services.AddSingleton(sp => sp.GetRequiredService<IMongoDatabase>().GetCollection<User>(typeof(User).Name));
        //builder.Services.AddSingleton<IMongoCollection<Bookmark>>(sp => sp.GetRequiredService<IMongoDatabase>().GetCollection<Bookmark>("bookmark"))
        //builder.Services.AddSingleton<IMongoCollection<Note>>(sp => sp.GetRequiredService<IMongoDatabase>().GetCollection<Note>("note"))
    }
}

public static class RsaKeyName
{
    public const string SigningKey = "RsaKeys:SigningKey";
    public const string EncryptingKey = "RsaKeys:EncryptingKey";
}
