using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mini.Common.Models;
using Mini.Common.Requests;
using Mini.Common.Responses;
using Mini.Common.Services;
using Mini.Common.Settings;
using Moq;
using Recep.Controllers;
using Recep.Services;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace Recep.Tests.Controllers;

[TestClass()]
public class AuthenticationControllerTests
{
    private Mock<IOptionsMonitor<JwtSetting>> mockJwtOptionsMonitor = new();
    private Mock<IOptions<ApplicationSetting>> mockApplicationSettingOptions = new();
    private Mock<IOptionsSnapshot<RsaKeySetting>> mockRsaKeySettingOptions = new();
    private Mock<IPkedService> mockPkedService = new();

    private readonly string signingKeyPrivateXml = @"
<RSAKeyValue>
<Modulus>nkPbFyg9VmgJBJhVxpBe27dzwxnXPMACnfXIPNagvwXJktLhxllydsGrHrQoffSPYRpB9lWF65Jl36/MSqBCy5RprovFDE+Bwjja7AEt5FX7bCajvY4o6PtwFNSrkyjOUH3Kz27FuWRJ/ONB+0UB82QU6tyNPuxLGizJ0QeusvVNTB8NaYXy+JsgoOK4C4fbp9ZlHZqQqWJIE26GUrcYFLv1dW2vTw7riYHpLtn5RMHUuyAMynQrbMJHJV7gg8ShBNWegr7jWmtKfHjMo+hch8nQA8k51+AqVWYPigtpVYv6H2eKrkSCR0+lGoKGiXLXkysfEKcDsDFKmS/ZvcYKRQ==</Modulus>
<Exponent>AQAB</Exponent>
<P>yONfvsvQsLnJJ543tk7vM9Rp4hW0yITs4KOJbBLKfNmf7SGhgvRc/LNXLn+Zs2DGrelKeVc026VpTcj9TaYUpq0ajEGuaQ54oYwRORxZb4+UEGkBLS9XlA8WcSRvnaKIZzzNz/qkK5M0srRVHSCJ8hnoJjQ1wLF53xHq1tzT1I8=</P>
<Q>ya8B5B+9X1PqIvYRKK/oTpX891MtPZs94R/gJFtmdQL8zCOi3sI/firavVUG3KFVg0miiP49kehIC/TTW8DUez54+kmntYR5HD7GTS4mBRt86/GeWkkSygBa8wAQRE4YXaNkcFe6mt1pAauKv6zgZxAm0mLQRH/6cMi8fTXy5es=</Q>
<DP>K4n8KmIKLQNXMZr9ACpDdbxH5pRFxl/o5xcKpb283Spdmwt68eL8dFh6QL2vFk2XQgIZ/gqEjkZfeFhjbfzc5PMHmfZyUpqsUncFFJesWp2HvbbEZoKoPJK4jnUTK9saLSXkhXsRMdCJz2F+QP3YZ/xtcDpInCba/xnnCAJ8dsk=</DP>
<DQ>HSx7kYwl0IxK6hO+F6yaZgO8O+qEOM0j/lSmD5xJDdQIzV66SI7AsHdyfMC35WJRBFS6diaQsXJwOYqIqFUUZ196EHpnEBLBkhxigveR+Fspjte8mELJvP6gKVZxA/BoBeOEu9t02sH8VomB4IgZTc9ATl6M0dvRVk8zNMmBAVE=</DQ>
<InverseQ>NUuxJnzOEisfB8yGAXGJDY0sO6TJETCVeiuJSc8Yp0xnJgqra54NrrpxsOrokhe1OX5iGYzM4alLHDL/NKjmwD65ymzsiUZ6y6z/Vpc+NHuksI/ZT6ftMYk5Jbqk4vuQM82+D4Sz5sRwRkFOgSjLEcKUXgLlZDcy03ZE2wBY7FU=</InverseQ>
<D>O3FTDbyQZS7IDFDHDsBU0INz2FiRTgD0Oq4A2lDmqTicQCgyopQf+Ni+09Zoyv4bCCabldEDbrpaxww5V+IflGUW2HcouHn4S6GdID1FJljhCPruTxjg5rjhnQFPvAL8kqDcltKNMxem7nPQ6XY+yFLTxG64ImUPxDIETx1qkU4X2QF6yPtc4EvZSnxSWIC30+pcd7FEX0taiB2E2aNFIZkhuxkYSuTYyo9NoiCkF2MtK8+34sCXtvudn2J78rvCXhND+yn/lshhfqoLSTOsums1HHlLxKzy32eJJKemX9Iz+pyiZCfCSF4nA4Jq6/PovsPS86XzUFjceMnAIryoFQ==</D>
</RSAKeyValue>";

    private readonly string encryptingKeyPublicXml = @"
<RSAKeyValue>
<Modulus>1GRNR0E6q83gpXWMsS8Np5AkjcIu8zFmP4LYxwrJDSiftI2DolNyRkf1fPdC6BnEWgL9ZDkGwbS8uPvqSuOfat5NpAoozsDoGbnqnsgxS4f9esIX01HlD1FElPEZEJMlrMYdUdo5kOSG6chNPIk8MLJxoCoSDGf98KYr+CmREw6itUkULELK0RcqRkYaJfe636Koi4cUbICfqsLvOnnJYsm8GNIQbm4tjrxt/lYGsyGHAE2bvDYlSV9rPml/AM7uNc74MW6Xs9Vxs2gIybttC6NfS/i9o+whi4+kRnQUIonMIVgSGTdQO2CitoZ+jswaPDH8g6JmvKmTyTHZaIppyQ==</Modulus>
<Exponent>AQAB</Exponent>
</RSAKeyValue>";


    [TestInitialize()]
    public void BeforeEachTest()
    {
        mockJwtOptionsMonitor = new Mock<IOptionsMonitor<JwtSetting>>();
        mockApplicationSettingOptions = new Mock<IOptions<ApplicationSetting>>();
        mockRsaKeySettingOptions = new Mock<IOptionsSnapshot<RsaKeySetting>>();
        mockPkedService = new Mock<IPkedService>();
    }

    [TestMethod()]
    public void AuthenticationControllerTest()
    {
        AuthenticationController? controller = new(
            mockJwtOptionsMonitor.Object
            , mockApplicationSettingOptions.Object
            , mockRsaKeySettingOptions.Object
            , mockPkedService.Object);

        Assert.IsNotNull(controller);
    }

    [TestMethod()]
    public async Task PostSymmetricTestAsync()
    {
        mockJwtOptionsMonitor.Setup(m => m.Get(It.IsAny<string>())).Returns(new JwtSetting
        {
            Issuer = "someIssuer",
            Audience = "someAudience"
        });

        mockApplicationSettingOptions.Setup(m => m.Value).Returns(new ApplicationSetting
        {
            Name = "someName",
            Version = "someVersion"
        });

        Environment.SetEnvironmentVariable("someName", "SOME_SALT|SOME_PASSWORD|11970|16180");

        AuthenticationController? controller = new(
            mockJwtOptionsMonitor.Object
            , mockApplicationSettingOptions.Object
            , mockRsaKeySettingOptions.Object
            , mockPkedService.Object);

        var encryptedMessage = JsonSerializer.Deserialize<EncryptedMessage>(
            "{\"IV\":\"UelZxbTAbykJw2HnHZuMbQ==\",\"EncryptedSessionKey\":\"F7PdjU0C0QJ4QPgGgdgj2DvSyoDD8LtyNgHXbJgoC4sxi+zaOicB8t5Qs0woniImxP/HFpbAO21cYeK475afGWcNV809QZEuLPSi321oEKN58YU0PSd1BEf2FLhecXNWj1+9Mfv/K4BL7dxAu3UkR0u42yunAOW9lVGXoxpxD2LrtqRIsXataQeIgtkmJ6wGhcq139t0j1c/JkhfsMF5hhSCHsWFfsVi0IRNr5VOEluoASQ3MOVN3QcYlxJPFxJ0P9YpiiTwCCGLP/vl0V12jelGAgaEdD5sw9lnVA24W1u9QZCTDS5/L9SA/IuFTAcWktnPMj86lM7uAykHpX3DUA==\",\"EncryptedMessageBytes\":\"D30g7YjK9Ds3EOJa7B9T57QQ89ORtyayMkrSgpm0BnUG37yf3+I2KDObLLGjdzsIf2a7dnvOznqwa46Unm9due3cjJqTnXm7UgLy0vsr9NpYXt0detqWLQJSk5uxRFJexn4sswZkKtAqzVPB7bb6Q68TVsEj7Ih+efGgBgRmVWa5g/oYdYmmr9EQHi0m7nabk7obwfCNG4K4g9ihWOVxFSIfjnNfSyD8mrFELpjcanj4srMgcdABQrB6ZNOlBVimUh/c4YZvgqFfGNFs/H/VxtpFvh/uraExLxYI1GyvRpJVLTqJyknJNcCJcrjOG5lB/7muKukpTL4dBM0WVXQDmLeCfhEvgnF+/dEaOfwPineYrleF08r6oH+JWQ+zG6wHC4QeKLOMWWITWQR8Z4v4k8y5kBVUERzraS3L/sD46KOk3x85McDSPuEViXCT0R4t8iYXWxfbJnm8dos8LWbGSTlTxAO9vbbF7wVv4KLClIdiQLEPcOwYD22vwadIeSjsKm3x61S0GPsYrDSfM6w7fgeMupzljfef/H04wQ+NaN66oNWDZGVab0V/jHeo7+8JEXwbibFc9TroaDd7A8YvZiFE+xk5h9ROHzexwKGOwz5AQDWD3Py6GnpruHHPE5XRAYrJXZH3pmaVYIWzaaqgk+oS/Ld3pcNc2Sylm1FBoObtgzBi7Me6AeneL6UNW7wnOaqpoUzygw1pY1N2ZZ76un0Q+GcaJ7CE5L7oBoUAG+YhohMrXmgMtR4AU/3CJYwXJgvWdur2p7vGXJRPXiOmyiSWuy44wdrEdhd9vMu6xDLO6rdt4h896B1nhjMUEkbaxMPxwqKklk0VNohQ7a+2//EgRNZIj/bnddxH3i+jJ7M=\"}");

        var result = await controller.PostAsync(encryptedMessage);

        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);
        Assert.IsInstanceOfType(result.Value, typeof(LoginResponse));
    }

    [TestMethod()]
    public async Task PostAsymmetricTestAsync()
    {
        Environment.SetEnvironmentVariable("UNIT_TEST_SIGNING_KEY_PRIVATE", signingKeyPrivateXml);

        mockJwtOptionsMonitor.Setup(m => m.Get(It.IsAny<string>())).Returns(new JwtSetting
        {
            Issuer = "someIssuer",
            Audience = "someAudience"
        });

        mockApplicationSettingOptions.Setup(m => m.Value).Returns(new ApplicationSetting
        {
            Name = "someName",
            Version = "someVersion"
        });

        mockRsaKeySettingOptions.Setup(m => m.Get(RsaKeyName.SigningKey)).Returns(new RsaKeySetting
        {
            SourceType = RsaKeySetting.RsaKeyDataSource.EnvironmentVariable,
            Source = "UNIT_TEST_SIGNING_KEY_PRIVATE"

        });

        mockPkedService.Setup(m => m.DecryptAsync<LoginRequest>(It.IsAny<EncryptedMessage>(), It.IsAny<string>())).ReturnsAsync(
            new LoginRequest()
            {
                Username = "someUsername",
                Password = "somePassword",
                Encrypting = new SecurityCredential
                {
                    SecurityAlgorithm = SecurityAlgorithms.RsaOAEP,
                    SecurityDigest = SecurityAlgorithms.Aes256CbcHmacSha512,
                    Xml = encryptingKeyPublicXml
                }
            });

        AuthenticationController? controller = new(
            mockJwtOptionsMonitor.Object
            , mockApplicationSettingOptions.Object
            , mockRsaKeySettingOptions.Object
            , mockPkedService.Object);

        var encryptedMessage = JsonSerializer.Deserialize<EncryptedMessage>(
            "{\"IV\":\"UelZxbTAbykJw2HnHZuMbQ==\",\"EncryptedSessionKey\":\"F7PdjU0C0QJ4QPgGgdgj2DvSyoDD8LtyNgHXbJgoC4sxi+zaOicB8t5Qs0woniImxP/HFpbAO21cYeK475afGWcNV809QZEuLPSi321oEKN58YU0PSd1BEf2FLhecXNWj1+9Mfv/K4BL7dxAu3UkR0u42yunAOW9lVGXoxpxD2LrtqRIsXataQeIgtkmJ6wGhcq139t0j1c/JkhfsMF5hhSCHsWFfsVi0IRNr5VOEluoASQ3MOVN3QcYlxJPFxJ0P9YpiiTwCCGLP/vl0V12jelGAgaEdD5sw9lnVA24W1u9QZCTDS5/L9SA/IuFTAcWktnPMj86lM7uAykHpX3DUA==\",\"EncryptedMessageBytes\":\"D30g7YjK9Ds3EOJa7B9T57QQ89ORtyayMkrSgpm0BnUG37yf3+I2KDObLLGjdzsIf2a7dnvOznqwa46Unm9due3cjJqTnXm7UgLy0vsr9NpYXt0detqWLQJSk5uxRFJexn4sswZkKtAqzVPB7bb6Q68TVsEj7Ih+efGgBgRmVWa5g/oYdYmmr9EQHi0m7nabk7obwfCNG4K4g9ihWOVxFSIfjnNfSyD8mrFELpjcanj4srMgcdABQrB6ZNOlBVimUh/c4YZvgqFfGNFs/H/VxtpFvh/uraExLxYI1GyvRpJVLTqJyknJNcCJcrjOG5lB/7muKukpTL4dBM0WVXQDmLeCfhEvgnF+/dEaOfwPineYrleF08r6oH+JWQ+zG6wHC4QeKLOMWWITWQR8Z4v4k8y5kBVUERzraS3L/sD46KOk3x85McDSPuEViXCT0R4t8iYXWxfbJnm8dos8LWbGSTlTxAO9vbbF7wVv4KLClIdiQLEPcOwYD22vwadIeSjsKm3x61S0GPsYrDSfM6w7fgeMupzljfef/H04wQ+NaN66oNWDZGVab0V/jHeo7+8JEXwbibFc9TroaDd7A8YvZiFE+xk5h9ROHzexwKGOwz5AQDWD3Py6GnpruHHPE5XRAYrJXZH3pmaVYIWzaaqgk+oS/Ld3pcNc2Sylm1FBoObtgzBi7Me6AeneL6UNW7wnOaqpoUzygw1pY1N2ZZ76un0Q+GcaJ7CE5L7oBoUAG+YhohMrXmgMtR4AU/3CJYwXJgvWdur2p7vGXJRPXiOmyiSWuy44wdrEdhd9vMu6xDLO6rdt4h896B1nhjMUEkbaxMPxwqKklk0VNohQ7a+2//EgRNZIj/bnddxH3i+jJ7M=\"}");

        var result = await controller.PostAsync(encryptedMessage);

        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);
        Assert.IsInstanceOfType(result.Value, typeof(LoginResponse));
    }
}
