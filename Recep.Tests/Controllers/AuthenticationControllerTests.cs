using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
    private Mock<IOptionsMonitor<JwtSetting>> mockJwtOptionsMonitor = new Mock<IOptionsMonitor<JwtSetting>>();
    private Mock<IOptions<ApplicationSetting>> mockApplicationSettingOptions = new Mock<IOptions<ApplicationSetting>>();

    [TestInitialize()]
    public void BeforeEachTest()
    {
        mockJwtOptionsMonitor = new Mock<IOptionsMonitor<JwtSetting>>();
        mockApplicationSettingOptions = new Mock<IOptions<ApplicationSetting>>();
    }

    [TestMethod()]
    public void AuthenticationControllerTest()
    {
        var controller = new AuthenticationController(mockJwtOptionsMonitor.Object, mockApplicationSettingOptions.Object);

        Assert.IsNotNull(controller);
    }

    [TestMethod()]
    public void PostTest()
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

        var controller = new AuthenticationController(mockJwtOptionsMonitor.Object, mockApplicationSettingOptions.Object);

        LoginRequest req = new LoginRequest()
        {
            Username = "someUsername",
            Password = "somePassword"
        };

        var result = controller.Post(req);

        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);
        Assert.IsInstanceOfType(result.Value, typeof(LoginResponse));
    }
}
