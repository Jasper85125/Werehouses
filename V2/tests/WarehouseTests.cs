using Microsoft.VisualStudio.TestTools.UnitTesting;
using ServicesV2;
using Moq;
using ControllersV2;
using System.Data.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace TestsV2
{
    [TestClass]
    public class WarehouseTest
    {
        private Mock<IWarehouseService> _mockWarehouseService;
        private WarehouseController _warehouseController;

        [TestInitialize]
        public void Setup()
        {
            _mockWarehouseService = new Mock<IWarehouseService>();
            _warehouseController = new WarehouseController(_mockWarehouseService.Object);
        }

        [TestMethod]
        public void GetWarehousesTest_Exists()
        {
            // Arrange
            var warehouses = new List<WarehouseCS>
            {
                new WarehouseCS { Id = 1, Address = "Straat 1" },
                new WarehouseCS { Id = 2, Address = "Warenhuislaan 280" }
            };
            _mockWarehouseService.Setup(service => service.GetAllWarehouses()).Returns(warehouses);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";  // Set the UserRole in HttpContext

            // Assign HttpContext to the controller
            _warehouseController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = _warehouseController.GetAllWarehouses(null, 1, 10);

            // Assert
            var okResult = result.Result as OkObjectResult;
            var returnedItems = okResult.Value as PaginationCS<WarehouseCS>;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(2, returnedItems.Data.Count());

            httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Client";  // Set the wrong UserRole in HttpContext

            // Assign HttpContext to the controller
            _warehouseController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
            //act
            result = _warehouseController.GetAllWarehouses(null, 1, 10);
            //assert
            var unauthorizedResult = result.Result as UnauthorizedResult;
            Assert.IsNotNull(unauthorizedResult);
            Assert.AreEqual(401, unauthorizedResult.StatusCode);
        }

        [TestMethod]
        public void GetWarehouseByIdTest_Exists()
        {
            // Arrange
            var warehouses = new List<WarehouseCS>
            {
                new WarehouseCS { Id = 1, Address = "Straat 1" },
                new WarehouseCS { Id = 2, Address = "Warenhuislaan 280" }
            };
            _mockWarehouseService.Setup(service => service.GetWarehouseById(1)).Returns(warehouses[0]);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";  // Set the UserRole in HttpContext

            // Assign HttpContext to the controller
            _warehouseController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = _warehouseController.GetWarehouseById(1);

            // Assert
            var okResult = result.Result as OkObjectResult;
            var returnedItems = okResult.Value as WarehouseCS;
            Assert.IsNotNull(okResult);
            Assert.IsNotNull(okResult.Value);
            Assert.AreEqual(warehouses[0].Address, returnedItems.Address);
                        httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Client";  // Set the wrong UserRole in HttpContext

            // Assign HttpContext to the controller
            _warehouseController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
            //act
            result = _warehouseController.GetWarehouseById(1);
            //assert
            var unauthorizedResult = result.Result as UnauthorizedResult;
            Assert.IsNotNull(unauthorizedResult);
            Assert.AreEqual(401, unauthorizedResult.StatusCode);
        }

        [TestMethod]
        public void GetWarehouseByIdTest_WrongId()
        {
            // Arrange
            _mockWarehouseService.Setup(service => service.GetWarehouseById(1)).Returns((WarehouseCS)null);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";  // Set the UserRole in HttpContext

            // Assign HttpContext to the controller
            _warehouseController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = _warehouseController.GetWarehouseById(1);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));

                        httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Client";  // Set the wrong UserRole in HttpContext

            // Assign HttpContext to the controller
            _warehouseController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
            //act
            result = _warehouseController.GetWarehouseById(1);
            //assert
            var unauthorizedResult = result.Result as UnauthorizedResult;
            Assert.IsNotNull(unauthorizedResult);
            Assert.AreEqual(401, unauthorizedResult.StatusCode);
        }

        [TestMethod]
        public void CreateWarehouse_ReturnsCreatedResult_WithNewWarehouse()
        {
            // Arrange
            var warehouse = new WarehouseCS { Id = 1, Address = "Straat 1" };

            _mockWarehouseService.Setup(service => service.CreateWarehouse(warehouse)).Returns(warehouse);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";  // Set the UserRole in HttpContext

            // Assign HttpContext to the controller
            _warehouseController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = _warehouseController.CreateWarehouse(warehouse);

            // Assert
            var createdResult = result.Result as CreatedAtActionResult;  // Use CreatedAtActionResult here
            Assert.IsNotNull(createdResult);

            var returnedItems = createdResult.Value as WarehouseCS;
            Assert.IsNotNull(returnedItems);
            Assert.AreEqual(warehouse.Address, returnedItems.Address);

            httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Client";  // Set the wrong UserRole in HttpContext

            // Assign HttpContext to the controller
            _warehouseController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
            //act
            result = _warehouseController.CreateWarehouse(warehouse);
            //assert
            var unauthorizedResult = result.Result as UnauthorizedResult;
            Assert.IsNotNull(unauthorizedResult);
            Assert.AreEqual(401, unauthorizedResult.StatusCode);
        }

        [TestMethod]
        public void CreateMultipleWarehouse_ReturnsCreatedResult_WithNewWarehouse()
        {
            // Arrange
            var warehouses = new List<WarehouseCS>
            {
                new WarehouseCS { Code= "X", Name= "cargo hub", Address= "bruv", Zip= "4002 AZ", City= "hub", Province= "Utrecht",
                                                Country= "GER", Contact= new Dictionary<string, string>{
                                                {"name", "Fem Keijzer"}, {"phone", "(078) 0013363"}, {"email", "blamore@example.net"}}},
                new WarehouseCS { Code= "X", Name= "cargo hub", Address= "bruv", Zip= "4002 AZ", City= "hub", Province= "Utrecht",
                                                Country= "GER", Contact= new Dictionary<string, string>{
                                                {"name", "Fem Keijzer"}, {"phone", "(078) 0013363"}, {"email", "blamore@example.net"}}}
            };
            _mockWarehouseService.Setup(service => service.CreateMultipleWarehouse(warehouses)).Returns(warehouses);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";  // Set the UserRole in HttpContext

            // Assign HttpContext to the controller
            _warehouseController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = _warehouseController.CreateMultipleWarehouse(warehouses);
            var createdResult = result.Result as ObjectResult;
            var returnedItems = createdResult.Value as List<WarehouseCS>;
            var firstWarehouse = returnedItems[0];

            // Assert
            Assert.IsNotNull(createdResult);
            Assert.IsNotNull(returnedItems);
            Assert.AreEqual(warehouses[0].Address, firstWarehouse.Address);
            Assert.AreEqual(warehouses[0].Contact, firstWarehouse.Contact);

            httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Client";  // Set the wrong UserRole in HttpContext

            // Assign HttpContext to the controller
            _warehouseController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
            //act
            result = _warehouseController.CreateMultipleWarehouse(warehouses);
            //assert
            var unauthorizedResult = result.Result as UnauthorizedResult;
            Assert.IsNotNull(unauthorizedResult);
            Assert.AreEqual(401, unauthorizedResult.StatusCode);
        }

        [TestMethod]
        public void UpdatedWarehouseTest_Success()
        {
            // Arrange
            var updatedWarehouse = new WarehouseCS
            {
                Id = 1,
                Code = "X",
                Name = "cargo hub",
                Address = "bruv",
                Zip = "4002 AZ",
                City = "hub",
                Province = "Utrecht",
                Country = "GER",
                Contact = new Dictionary<string, string> { { "name", "Fem Keijzer" }, { "phone", "(078) 0013363" }, { "email", "blamore@example.net" } }
            };

            _mockWarehouseService.Setup(service => service.UpdateWarehouse(1, updatedWarehouse)).Returns(updatedWarehouse);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";  // Set the UserRole in HttpContext

            // Assign HttpContext to the controller
            _warehouseController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = _warehouseController.UpdateWarehouse(1, updatedWarehouse);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var createdResult = result.Result as OkObjectResult;
            Assert.IsNotNull(createdResult);
            Assert.IsInstanceOfType(createdResult.Value, typeof(WarehouseCS));
            var returnedWarehouse = createdResult.Value as WarehouseCS;
            Assert.AreEqual(updatedWarehouse.Code, returnedWarehouse.Code);
            Assert.AreEqual(updatedWarehouse.Address, returnedWarehouse.Address);

            httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Client";  // Set the wrong UserRole in HttpContext

            // Assign HttpContext to the controller
            _warehouseController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
            //act
            result = _warehouseController.UpdateWarehouse(1, updatedWarehouse);
            //assert
            var unauthorizedResult = result.Result as UnauthorizedResult;
            Assert.IsNotNull(unauthorizedResult);
            Assert.AreEqual(401, unauthorizedResult.StatusCode);
        }

        [TestMethod]
        public void UpdatedWarehouseTest_Failed()
        {
            // Arrange
            var updatedWarehouse = new WarehouseCS
            {
                Id = 1,
                Code = "X",
                Name = "cargo hub",
                Address = "bruv",
                Zip = "4002 AZ",
                City = "hub",
                Province = "Utrecht",
                Country = "GER",
                Contact = new Dictionary<string, string> { { "name", "Fem Keijzer" }, { "phone", "(078) 0013363" }, { "email", "blamore@example.net" } }
            };

            _mockWarehouseService.Setup(service => service.UpdateWarehouse(0, updatedWarehouse)).Returns((WarehouseCS)null);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";  // Set the UserRole in HttpContext

            // Assign HttpContext to the controller
            _warehouseController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = _warehouseController.UpdateWarehouse(0, updatedWarehouse);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(NotFoundObjectResult));
            var createdResult = result.Result as NotFoundObjectResult;
            var returnedWarehouse = createdResult.Value as WarehouseCS;
            Assert.IsNull(returnedWarehouse);

            httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Client";  // Set the wrong UserRole in HttpContext

            // Assign HttpContext to the controller
            _warehouseController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
            //act
            result = _warehouseController.UpdateWarehouse(0, updatedWarehouse);
            //assert
            var unauthorizedResult = result.Result as UnauthorizedResult;
            Assert.IsNotNull(unauthorizedResult);
            Assert.AreEqual(401, unauthorizedResult.StatusCode);
        }
        
        [TestMethod]
        public void PatchWarehouse_Succes(){
            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";  // Set the UserRole in HttpContext

            // Assign HttpContext to the controller
            _warehouseController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
            //Arrange
            var warehouse = new WarehouseCS(){ Id = 1, Code= "LOLJK", Name="KOPLER"};
            _mockWarehouseService.Setup(service=>service.PatchWarehouse(1, "Code", "LOLJK")).Returns(warehouse);
            _mockWarehouseService.Setup(service=>service.PatchWarehouse(1, "Name", "KOPLER")).Returns(warehouse);
            //Act
            var result1 = _warehouseController.PatchWarehouse(1, "Code", "LOLJK");
            //ik wilde ook Name testen, result2*
            var result2 = _warehouseController.PatchWarehouse(1, "Name", "KOPLER");
            var result1ok = result1.Result as OkObjectResult;
            var result2ok = result2.Result as OkObjectResult;
            var result1value = result1ok.Value as WarehouseCS;
            var result2value = result2ok.Value as WarehouseCS;
            //Assert
            Assert.IsNotNull(result1);
            Assert.IsNotNull(result1ok);
            Assert.IsNotNull(result2);
            Assert.IsNotNull(result2ok);
            Assert.IsNotNull(result1value);
            Assert.IsNotNull(result2value);
            Assert.AreEqual(result1ok.StatusCode, 200);
            Assert.AreEqual(result2ok.StatusCode, 200);
            Assert.AreEqual(result1value.Code, warehouse.Code);
            Assert.AreEqual(result2value.Name, warehouse.Name);

            httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Client";  // Set the wrong UserRole in HttpContext

            // Assign HttpContext to the controller
            _warehouseController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
            //act
            var result = _warehouseController.PatchWarehouse(1, "Code", "LOLJK");
            //assert
            var unauthorizedResult = result.Result as UnauthorizedResult;
            Assert.IsNotNull(unauthorizedResult);
            Assert.AreEqual(401, unauthorizedResult.StatusCode);
        }
        [TestMethod]
        public void DeleteWarehouseTest_Success()
        {
            // Arrange
            var warehouse = new WarehouseCS { Id = 1, Address = "Straat 1" };
            _mockWarehouseService.Setup(service => service.GetWarehouseById(1)).Returns(warehouse);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";  // Set the UserRole in HttpContext

            // Assign HttpContext to the controller
            _warehouseController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = _warehouseController.DeleteWarehouse(1);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkResult));

            httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Client";  // Set the wrong UserRole in HttpContext

            // Assign HttpContext to the controller
            _warehouseController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
            //act
            result = _warehouseController.DeleteWarehouse(1);
            //assert
            var unauthorizedResult = result as UnauthorizedResult;
            Assert.IsNotNull(unauthorizedResult);
            Assert.AreEqual(401, unauthorizedResult.StatusCode);
        }
        [TestMethod]
        public void DeleteWarehousesTest_Succes()
        {
            //Arrange
            var idsToDelete = new List<int>() { 1, 2, 3 };

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";  // Set the UserRole in HttpContext

            // Assign HttpContext to the controller
            _warehouseController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            //Act
            var result = _warehouseController.DeleteWarehouses(idsToDelete);
            var resultok = result as OkObjectResult;

            //Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            Assert.AreEqual(resultok.StatusCode, 200);

            httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Client";  // Set the wrong UserRole in HttpContext

            // Assign HttpContext to the controller
            _warehouseController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
            //act
            result = _warehouseController.DeleteWarehouses(idsToDelete);
            //assert
            var unauthorizedResult = result as UnauthorizedResult;
            Assert.IsNotNull(unauthorizedResult);
            Assert.AreEqual(401, unauthorizedResult.StatusCode);
        }

        [TestMethod]
        public void GetLatestUpdatedWarehouseTest_Success()
        {
            // Arrange
            var warehouse = new WarehouseCS { Id = 1, Address = "Straat 1" };
            _mockWarehouseService.Setup(service => service.GetLatestUpdatedWarehouse(It.IsAny<int>())).Returns(new List<WarehouseCS> { warehouse });

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";  // Set the UserRole in HttpContext

            // Assign HttpContext to the controller
            _warehouseController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = _warehouseController.GetLatestUpdatedWarehouse();

            // Assert
            var okResult = result.Result as OkObjectResult;
            var returnedItems = (okResult.Value as IEnumerable<WarehouseCS>)?.FirstOrDefault();
            Assert.IsNotNull(okResult);
            Assert.IsNotNull(returnedItems);
            Assert.AreEqual(warehouse.Address, returnedItems.Address);

            httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Client";  // Set the wrong UserRole in HttpContext

            // Assign HttpContext to the controller
            _warehouseController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
            //act
            result = _warehouseController.GetLatestUpdatedWarehouse();
            //assert
            var unauthorizedResult = result.Result as UnauthorizedResult;
            Assert.IsNotNull(unauthorizedResult);
            Assert.AreEqual(401, unauthorizedResult.StatusCode);
        }

        [TestMethod]
        public void GetLatestUpdatedWarehouseTest_Failed()
        {
            // Arrange
            _mockWarehouseService.Setup(service => service.GetLatestUpdatedWarehouse(It.IsAny<int>())).Returns((List<WarehouseCS>)null);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";  // Set the UserRole in HttpContext

            // Assign HttpContext to the controller
            _warehouseController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = _warehouseController.GetLatestUpdatedWarehouse();

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));

            httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Client";  // Set the wrong UserRole in HttpContext

            // Assign HttpContext to the controller
            _warehouseController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
            //act
            result = _warehouseController.GetLatestUpdatedWarehouse();
            //assert
            var unauthorizedResult = result.Result as UnauthorizedResult;
            Assert.IsNotNull(unauthorizedResult);
            Assert.AreEqual(401, unauthorizedResult.StatusCode);
        }
    }
}

