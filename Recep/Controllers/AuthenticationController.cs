using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Mini.Common.Helpers;
using Mini.Common.Requests;
using Mini.Common.Responses;
using Mini.Common.Settings;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Recep.Controllers;

[AllowAnonymous]
[Route("api/[controller]")]
[ApiController]
public class AuthenticationController : ControllerBase
{
    private readonly IOptionsMonitor<JwtSetting> jwtOptions;
    private readonly ApplicationSetting applicationSetting;
    private readonly RsaKeySetting rsaSigningKeySetting;

    public AuthenticationController(
        IOptionsMonitor<JwtSetting> jwtOptionsMonitor
        , IOptions<ApplicationSetting> applicationSettingOptions
        , IOptions<RsaKeySetting> rsaKeySettingOptions)
    {
        jwtOptions = jwtOptionsMonitor;

        applicationSetting = applicationSettingOptions.Value;

        rsaSigningKeySetting = rsaKeySettingOptions.Value;
    }

    // POST api/<AuthenticationController>
    [HttpPost]
    public OkObjectResult Post([FromBody] LoginRequest value)
    {
        LoginResponse response;

        SecurityKey signingCredentialSecurityKey;
        SecurityKey encryptingCredentialSecurityKey;

        JwtSecurityTokenHandler? jwtSecurityTokenHandler = new();
        JwtSecurityToken? jwtSecurityToken;

        // Get Issuer and Audience

        var jwtSetting = jwtOptions.Get("Conso");
        jwtSetting.EnsureIsValid();

        // Setup claims

        List<Claim>? authClaims = new()
        {
            // JwtRegisteredClaimNames contains a list of valid Jwt claim names

            new Claim(ClaimTypes.Name, value.Username),
            new Claim(ClaimTypes.Role, "Admin"),
            new Claim(ClaimTypes.Role, "Developer"),
            new Claim(ClaimTypes.Role, "Tester")
        };
        

        if (value.Encrypting == null)
        {
            // Use Symmetric encryption

            string secretKey = EnvironmentHelper.GetVariable(applicationSetting.Name);

            (signingCredentialSecurityKey, encryptingCredentialSecurityKey) = 
                SecurityKeyHelper.SymmetricSecurityKey(secretKey, HashAlgorithmName.SHA256);

            jwtSecurityToken = jwtSecurityTokenHandler.CreateJwtSecurityToken(
                issuer: jwtSetting.Issuer,
                audience: jwtSetting.Audience,
                subject: new ClaimsIdentity(authClaims),
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddMinutes(jwtSetting.ExpirationMinutes),
                issuedAt: DateTime.UtcNow,
                signingCredentials: new SigningCredentials(
                    signingCredentialSecurityKey
                    , SecurityAlgorithms.HmacSha256
                    , SecurityAlgorithms.Sha256Digest),
                encryptingCredentials: new EncryptingCredentials(
                    encryptingCredentialSecurityKey
                    , SecurityAlgorithms.Aes256KW
                    , SecurityAlgorithms.Aes256CbcHmacSha512)
                );

            response = new()
            {
                Jwt = jwtSecurityTokenHandler.WriteToken(jwtSecurityToken),
                ExpiryDateTime = jwtSecurityToken.ValidTo
            };
        }
        else
        {
            // Use Asymmetric encryption

            // Signing key needs to have private key
            signingCredentialSecurityKey = rsaSigningKeySetting.GetRsaSecurityKey(true);

            var securityCredential = value.Encrypting.Value;

            using RSA encryptingKeyRsa = RSA.Create();
            
            encryptingKeyRsa.FromXmlString(securityCredential.Xml);

            encryptingCredentialSecurityKey = new RsaSecurityKey(encryptingKeyRsa);

            jwtSecurityToken = jwtSecurityTokenHandler.CreateJwtSecurityToken(
                issuer: jwtSetting.Issuer,
                audience: jwtSetting.Audience,
                subject: new ClaimsIdentity(authClaims),
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddMinutes(jwtSetting.ExpirationMinutes),
                issuedAt: DateTime.UtcNow,
                signingCredentials: new SigningCredentials(
                    signingCredentialSecurityKey
                    , SecurityAlgorithms.RsaSsaPssSha256
                    , SecurityAlgorithms.RsaSsaPssSha256Signature),
                encryptingCredentials: new EncryptingCredentials(
                    encryptingCredentialSecurityKey
                    , securityCredential.SecurityAlgorithm
                    , securityCredential.SecurityDigest)
                );

            response = new()
            {
                Jwt = jwtSecurityTokenHandler.WriteToken(jwtSecurityToken),
                ExpiryDateTime = jwtSecurityToken.ValidTo
            };

        }

        // For a list algorithms applicable to tokens see
        // See: https://datatracker.ietf.org/doc/html/rfc7518
        // See: https://github.com/AzureAD/azure-activedirectory-identitymodel-extensions-for-dotnet/wiki/Supported-Algorithms
        //

        return Ok(response);

    }
    
}
