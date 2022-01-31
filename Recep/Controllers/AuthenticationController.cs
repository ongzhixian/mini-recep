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
using System.Text.Json;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Recep.Controllers;

[AllowAnonymous]
[Route("api/[controller]")]
[ApiController]
public class AuthenticationController : ControllerBase
{
    private readonly IOptionsMonitor<JwtSetting> jwtOptions;
    private readonly ApplicationSetting applicationSetting;

    public AuthenticationController(IOptionsMonitor<JwtSetting> jwtOptionsMonitor,
        IOptions<ApplicationSetting> applicationSettingOptions)
    {
        jwtOptions = jwtOptionsMonitor;

        applicationSetting = applicationSettingOptions.Value;
    }

    // POST api/<AuthenticationController>
    [HttpPost]
    public OkObjectResult Post([FromBody] LoginRequest value)
    {

        return Rsa(value);

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

        string secretKey = EnvironmentHelper.GetVariable(applicationSetting.Name);

        (var scKey, var ecKey) = SecurityKeyHelper.SymmetricSecurityKey(secretKey, HashAlgorithmName.SHA256);

        JwtSecurityTokenHandler? jwtSecurityTokenHandler = new();

        var jwtSecurityToken = jwtSecurityTokenHandler.CreateJwtSecurityToken(
            issuer: jwtSetting.Issuer,
            audience: jwtSetting.Audience,
            subject: new ClaimsIdentity(authClaims),
            notBefore: DateTime.UtcNow,
            expires: DateTime.UtcNow.AddMinutes(jwtSetting.ExpirationMinutes),
            issuedAt: DateTime.UtcNow,
            signingCredentials: new SigningCredentials(scKey, SecurityAlgorithms.HmacSha256, SecurityAlgorithms.Sha256Digest),
            encryptingCredentials: new EncryptingCredentials(ecKey, SecurityAlgorithms.Aes256KW, SecurityAlgorithms.Aes256CbcHmacSha512)
            );

        

        // For a list algorithms applicable to tokens see
        // See: https://datatracker.ietf.org/doc/html/rfc7518
        // See: https://github.com/AzureAD/azure-activedirectory-identitymodel-extensions-for-dotnet/wiki/Supported-Algorithms
        //

        LoginResponse? res = new()
        {
            Jwt = jwtSecurityTokenHandler.WriteToken(jwtSecurityToken),
            ExpiryDateTime = jwtSecurityToken.ValidTo
        };

        return Ok(res);

    }

    public OkObjectResult Rsa(LoginRequest value)
    {
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

        string secretKey = EnvironmentHelper.GetVariable(applicationSetting.Name);

        (var scKey, var ecKey) = SecurityKeyHelper.SymmetricSecurityKey(secretKey, HashAlgorithmName.SHA256);

        try
        {
            using (RSA myRsa = RSA.Create())
            {
                byte[] pvkBytes = System.IO.File.ReadAllBytes(@"D:\src\github\recep\test2.pvk");
                
                myRsa.ImportRSAPublicKey(pvkBytes, out int count);

                var myRsaPbkParams = myRsa.ExportParameters(false);

                AsymmetricSecurityKey key = new RsaSecurityKey(myRsaPbkParams);

                AsymmetricSecurityKey signkey = new RsaSecurityKey(myRsa);

                RsaSecurityKey signkey2;
                using (var rsa = RSA.Create(2048))
                {
                    signkey2 = new RsaSecurityKey(rsa);
                }

                JwtSecurityTokenHandler? jwtSecurityTokenHandler = new();

                var jwtSecurityToken = jwtSecurityTokenHandler.CreateJwtSecurityToken(
                    issuer: jwtSetting.Issuer,
                    audience: jwtSetting.Audience,
                    subject: new ClaimsIdentity(authClaims),
                    notBefore: DateTime.UtcNow,
                    expires: DateTime.UtcNow.AddMinutes(jwtSetting.ExpirationMinutes),
                    issuedAt: DateTime.UtcNow,
                    //signingCredentials: new SigningCredentials(scKey, SecurityAlgorithms.HmacSha256, SecurityAlgorithms.Sha256Digest),
                    signingCredentials: new SigningCredentials(signkey2, SecurityAlgorithms.RsaSsaPssSha256),
                    encryptingCredentials: new EncryptingCredentials(key, SecurityAlgorithms.RsaOAEP, SecurityAlgorithms.Aes256CbcHmacSha512)
                );

                LoginResponse? res = new()
                {
                    Jwt = jwtSecurityTokenHandler.WriteToken(jwtSecurityToken),
                    ExpiryDateTime = jwtSecurityToken.ValidTo
                };

                return Ok(res);
            }
        }
        catch (Exception ex)
        {
            throw;
        }
    }
}
