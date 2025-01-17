using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Microsoft.AspNetCore.Mvc;
using ServicesV2;
using ControllersV2;
using Microsoft.AspNetCore.Http;

namespace clients.TestsV2
{
    [TestClass]
    public class AdminTest
    {
        private Mock<IAdminService> _mockAdminService;
        private AdminController _adminController;

        [TestInitialize]
        public void Setup()
        {
            _mockAdminService = new Mock<IAdminService>();
            _adminController = new AdminController(_mockAdminService.Object); // Corrected capitalization
        }

        [TestMethod]
        public void TestUpdateAPIKeys_Success()
        {
            // Arrange
            var apiKey = "AnalystKey";
            var newApiKey = new ApiKeyModel
            {
                Key = "AnalystKey",
                Role = "Analyst",
                WarehouseID = "1,2,3"
            };

            _mockAdminService
                .Setup(service => service.UpdateAPIKeys(apiKey, newApiKey))
                .Verifiable(); // Set up the mock service behavior

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";  // Set the UserRole in HttpContext

            // Assign HttpContext to the controller
            _adminController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = _adminController.UpdateAPIKeys(apiKey, newApiKey);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            _mockAdminService.Verify(service => service.UpdateAPIKeys(apiKey, newApiKey), Times.Once);
        }
        [TestMethod]
public void TestUpdateAPIKeys_Failed()
{
    // Arrange
    var apiKey = "FAIL";
    var newApiKey = new ApiKeyModel
    {
        Key = "testtt",
        Role = "Analyst",
        WarehouseID = "1,2,3"
    };

    // Mock the UpdateAPIKeys method to simulate failure
    _mockAdminService
        .Setup(service => service.UpdateAPIKeys(apiKey, newApiKey))
        .Returns((ApiKeyModel)null); // Simulating failure

    var httpContext = new DefaultHttpContext();
    httpContext.Items["UserRole"] = "Admin"; // Set the UserRole in HttpContext

    // Assign HttpContext to the controller
    _adminController.ControllerContext = new ControllerContext
    {
        HttpContext = httpContext
    };

    // Act
    var result = _adminController.UpdateAPIKeys(apiKey, newApiKey);

    // Assert
    Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
    var badRequestResult = result as BadRequestObjectResult;
    Assert.AreEqual("API Key update failed", badRequestResult.Value);
}


    }
}
