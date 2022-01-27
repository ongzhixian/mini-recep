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
        // 

        LoginResponse? res = new()
        {
            Jwt = jwtSecurityTokenHandler.WriteToken(jwtSecurityToken),
            ExpiryDateTime = jwtSecurityToken.ValidTo
        };

        return Ok(res);

    }
}
