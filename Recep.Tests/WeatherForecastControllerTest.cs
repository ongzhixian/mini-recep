using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Recep.Controllers;

namespace Recep.Tests;

[TestClass]
public class WeatherForecastControllerTest
{
    private Mock<ILogger<WeatherForecastController>> mockLogger = new();


    [TestInitialize]
    public void BeforeEachTest()
    {
        mockLogger = new Mock<ILogger<WeatherForecastController>>();
    }

    [TestMethod]
    public void GetTest()
    {
        WeatherForecastController controller = new(mockLogger.Object);

        var result = controller.Get();

        Assert.IsNotNull(result);
    }
}