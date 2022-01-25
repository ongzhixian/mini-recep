using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Recep.Controllers;
using Recep.Models;
using System.Collections.Generic;

namespace Recep.Tests;

[TestClass]
public class WeatherForecastControllerTest
{
    Mock<ILogger<WeatherForecastController>> mockLogger = new();


    [TestInitialize]
    public void BeforeEachTest()
    {
        mockLogger = new Mock<ILogger<WeatherForecastController>>();
    }

    [TestMethod]
    public void GetTest()
    {
        WeatherForecastController controller = new WeatherForecastController(mockLogger.Object);

        IEnumerable<WeatherForecast>? result = controller.Get();

        Assert.IsNotNull(result);
    }
}