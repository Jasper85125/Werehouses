using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Microsoft.AspNetCore.Mvc;
using ServicesV2;
using ControllersV2;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

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
            _adminController = new AdminController(_mockAdminService.Object);
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
                .Returns(newApiKey);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";

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

            _mockAdminService
                .Setup(service => service.UpdateAPIKeys(apiKey, newApiKey))
                .Returns((ApiKeyModel)null);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";

            _adminController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = _adminController.UpdateAPIKeys(apiKey, newApiKey);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public void TestAddAPIKeys_Success()
        {
            // Arrange
            var newApiKey = new ApiKeyModel
            {
                Key = "AnalystKey",
                Role = "Analyst",
                WarehouseID = "1,2,3"
            };

            _mockAdminService
                .Setup(service => service.AddAPIKeys(newApiKey))
                .Returns(newApiKey);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";

            _adminController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = _adminController.AddAPIKeys(newApiKey);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            _mockAdminService.Verify(service => service.AddAPIKeys(newApiKey), Times.Once);
        }

        [TestMethod]
        public void TestAddAPIKeys_Failed()
        {
            // Arrange
            var newApiKey = new ApiKeyModel
            {
                Key = "AnalystKey",
                Role = "Analyst",
                WarehouseID = "1,2,3"
            };

            _mockAdminService
                .Setup(service => service.AddAPIKeys(newApiKey))
                .Returns((ApiKeyModel)null);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";

            _adminController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = _adminController.AddAPIKeys(newApiKey);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public void TestDeleteAPIKeys_Success()
        {
            // Arrange
            var apiKey = "AnalystKey";

            _mockAdminService
                .Setup(service => service.DeleteAPIKeys(apiKey))
                .Returns(new ApiKeyModel
                {
                    Key = "AnalystKey",
                    Role = "Analyst",
                    WarehouseID = "1,2,3"
                });

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";

            _adminController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = _adminController.DeleteAPIKeys(apiKey);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            _mockAdminService.Verify(service => service.DeleteAPIKeys(apiKey), Times.Once);
        }

        [TestMethod]
        public void TestDeleteAPIKeys_Failed()
        {
            // Arrange
            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";

            _adminController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
            //arrange
            var apiKey = "FAIL";

            _mockAdminService
                .Setup(service => service.DeleteAPIKeys(apiKey))
                .Returns((ApiKeyModel)null);

            // Act
            var result = _adminController.DeleteAPIKeys(apiKey);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            _mockAdminService.Verify(service => service.DeleteAPIKeys(apiKey), Times.Once);
        }

        [TestMethod]
        public void TestFileProcessing_JsonFile()
        {
            // Arrange
            var fileMock = new Mock<IFormFile>();
            var content = "{\"key\":\"value\"}";
            var fileName = "test.json";
            var ms = new MemoryStream();
            var writer = new StreamWriter(ms);
            writer.Write(content);
            writer.Flush();
            ms.Position = 0;
            fileMock.Setup(_ => _.OpenReadStream()).Returns(ms);
            fileMock.Setup(_ => _.FileName).Returns(fileName);
            fileMock.Setup(_ => _.Length).Returns(ms.Length);

            var path = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "data", fileName);

            // Act
            var result = ProcessFile(fileMock.Object);

            // Assert
            Assert.AreEqual(fileName, result);
            Assert.IsTrue(File.Exists(path));
        }

        [TestMethod]
        public void TestFileProcessing_CsvFile()
        {
            // Arrange
            var fileMock = new Mock<IFormFile>();
            var content = "header1,header2\nvalue1,value2";
            var fileName = "test.csv";
            var ms = new MemoryStream();
            var writer = new StreamWriter(ms);
            writer.Write(content);
            writer.Flush();
            ms.Position = 0;
            fileMock.Setup(_ => _.OpenReadStream()).Returns(ms);
            fileMock.Setup(_ => _.FileName).Returns(fileName);
            fileMock.Setup(_ => _.Length).Returns(ms.Length);

            var saveFileName = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "Data", Path.ChangeExtension(fileName, ".json"));

            // Act
            var result = ProcessFile(fileMock.Object);

            // Assert
            Assert.AreEqual(saveFileName, result);
            Assert.IsTrue(File.Exists(saveFileName));
        }

        private string ProcessFile(IFormFile file)
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "data", file.FileName);
            if (Path.GetExtension(file.FileName) == ".json")
            {
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    file.CopyTo(stream);
                }
                return file.FileName;
            }
            using var reader = new StreamReader(file.OpenReadStream());
            var csvContent = reader.ReadToEnd();
            var lines = csvContent.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            var headers = lines[0].Split(',');

            var clients = new List<Dictionary<string, string>>();

            for (int i = 1; i < lines.Length; i++)
            {
                var values = lines[i].Split(',');
                var client = new Dictionary<string, string>();

                for (int j = 0; j < headers.Length; j++)
                {
                    client[headers[j].Trim()] = values[j].Trim();
                }

                clients.Add(client);
            }

            var jsonContent = JsonSerializer.Serialize(clients, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            var saveFileName = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "Data", Path.ChangeExtension(file.FileName, ".json"));
            File.WriteAllText(saveFileName, jsonContent);
            return saveFileName;
        }
    }
}
