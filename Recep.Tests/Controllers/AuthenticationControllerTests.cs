using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mini.Common.Models;
using Mini.Common.Requests;
using Mini.Common.Responses;
using Mini.Common.Settings;
using Moq;
using Recep.Controllers;
using System;

namespace Recep.Tests.Controllers;

[TestClass()]
public class AuthenticationControllerTests
{
    private Mock<IOptionsMonitor<JwtSetting>> mockJwtOptionsMonitor = new();
    private Mock<IOptions<ApplicationSetting>> mockApplicationSettingOptions = new();
    private Mock<IOptions<RsaKeySetting>> mockRsaKeySettingOptions = new();

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
        mockRsaKeySettingOptions = new Mock<IOptions<RsaKeySetting>>();
    }

    [TestMethod()]
    public void AuthenticationControllerTest()
    {
        AuthenticationController? controller = new(
            mockJwtOptionsMonitor.Object
            , mockApplicationSettingOptions.Object
            , mockRsaKeySettingOptions.Object);

        Assert.IsNotNull(controller);
    }

    [TestMethod()]
    public void PostSymmetricTest()
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
            , mockRsaKeySettingOptions.Object);

        LoginRequest req = new()
        {
            Username = "someUsername",
            Password = "somePassword"
        };

        var result = controller.Post(req);

        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);
        Assert.IsInstanceOfType(result.Value, typeof(LoginResponse));
    }

    [TestMethod()]
    public void PostAsymmetricTest()
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

        mockRsaKeySettingOptions.Setup(m => m.Value).Returns(new RsaKeySetting
        {
            SourceType = RsaKeySetting.RsaKeyDataSource.EnvironmentVariable,
            Source = "UNIT_TEST_SIGNING_KEY_PRIVATE"

        });


        AuthenticationController? controller = new(
            mockJwtOptionsMonitor.Object
            , mockApplicationSettingOptions.Object
            , mockRsaKeySettingOptions.Object);

        LoginRequest req = new()
        {
            Username = "someUsername",
            Password = "somePassword",
            Encrypting = new SecurityCredential
            {
                SecurityAlgorithm = SecurityAlgorithms.RsaOAEP,
                SecurityDigest = SecurityAlgorithms.Aes256CbcHmacSha512,
                Xml = encryptingKeyPublicXml
            }
        };

        var result = controller.Post(req);

        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);
        Assert.IsInstanceOfType(result.Value, typeof(LoginResponse));
    }
}
