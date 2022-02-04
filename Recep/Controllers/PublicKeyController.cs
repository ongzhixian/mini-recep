using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Mini.Common.Settings;
using Recep.Services;
using System.Net.Mime;

namespace Recep.Controllers;

[AllowAnonymous]
[Route("api/[controller]")]
[ApiController]
public class PublicKeyController : ControllerBase
{
    private readonly RsaKeySetting rsaSigningKeySetting;
    private readonly RsaKeySetting rsaEncryptingKeySetting;

    public PublicKeyController(IOptionsMonitor<RsaKeySetting> rsaKeySettingOptions)
    {
        rsaSigningKeySetting = rsaKeySettingOptions.Get(RsaKeyName.SigningKey);

        rsaSigningKeySetting.EnsureIsValid();

        rsaEncryptingKeySetting = rsaKeySettingOptions.Get(RsaKeyName.EncryptingKey);

        rsaEncryptingKeySetting.EnsureIsValid();
    }

    // GET api/<PublicKeyController>/5
    [HttpGet("{purpose}")]
    public async Task<IActionResult> GetAsync(string purpose)
    {
        if (purpose == "encrypt")
            return new ContentResult
            {
                Content =  await rsaEncryptingKeySetting.GetRsaSecurityKeyXmlAsync(false),
                ContentType = MediaTypeNames.Text.Plain,
                StatusCode = StatusCodes.Status200OK
            };

        if (purpose == "sign")
            return new ContentResult
            {
                Content = await rsaSigningKeySetting.GetRsaSecurityKeyXmlAsync(false),
                ContentType = MediaTypeNames.Text.Plain,
                StatusCode = StatusCodes.Status200OK
            };

        // Note: Using ContentResult to force return of plain/text type.
        //       If we use OkObjectResult, the output will be a JSON

        return new BadRequestObjectResult($"Invalid purpose {purpose}");
    }
}
