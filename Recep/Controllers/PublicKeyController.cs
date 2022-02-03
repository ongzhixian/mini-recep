using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Mini.Common.Settings;
using Recep.Services;

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

        //rsaSigningKeySetting.EnsureIsValid();

        rsaEncryptingKeySetting = rsaKeySettingOptions.Get(RsaKeyName.EncryptingKey);

        //rsaEncryptingKeySetting.EnsureIsValid();

        //TODO:
        // Maybe RsaKeySetting.RsaKeyDataSource.Default is better Unknown
        // Improvement for RsaKeySetting in Mini.Common
        // if rsaSigningKeySetting.SourceType == RsaKeySetting.RsaKeyDataSource.Default
        //    // throw exception
        // To use Ensure-pattern?
    }

    // GET api/<PublicKeyController>/5
    [HttpGet("{purpose}")]
    public IActionResult Get(string purpose)
    {
        if (purpose == "encrypt")
            return new OkObjectResult(rsaEncryptingKeySetting.GetRsaSecurityKeyXml(false));

        if (purpose == "sign")
            return new OkObjectResult(rsaSigningKeySetting.GetRsaSecurityKeyXml(false));

        return new BadRequestObjectResult($"Invalid purpose {purpose}");
    }
}
