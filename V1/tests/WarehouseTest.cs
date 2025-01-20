using Microsoft.VisualStudio.TestTools.UnitTesting;
using ServicesV1;
using Moq;
using ControllersV1;
using System.Data.Common;
using Microsoft.AspNetCore.Mvc;

namespace TestsV1
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
            
            // Act
            var result = _warehouseController.GetAllWarehouses();
            
            // Assert
            var okResult = result.Result as OkObjectResult;
            var returnedItems = okResult.Value as IEnumerable<WarehouseCS>;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(2, returnedItems.Count());
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
            
            // Act
            var result = _warehouseController.GetWarehouseById(1);
            
            // Assert
            var okResult = result.Result as OkObjectResult;
            var returnedItems = okResult.Value as WarehouseCS;
            Assert.IsNotNull(okResult);
            Assert.IsNotNull(okResult.Value);
            Assert.AreEqual(warehouses[0].Address, returnedItems.Address);
        }

        [TestMethod]
        public void GetWarehouseByIdTest_WrongId()
        {
            // Arrange
            _mockWarehouseService.Setup(service => service.GetWarehouseById(1)).Returns((WarehouseCS)null);
            
            // Act
            var result = _warehouseController.GetWarehouseById(1);
            
            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));
        }

        [TestMethod]
        public void CreateWarehouse_ReturnsCreatedResult_WithNewWarehouse()
        {
            // Arrange
            var warehouse = new WarehouseCS { Id = 1, Address = "Straat 1" };
    
            _mockWarehouseService.Setup(service => service.CreateWarehouse(warehouse)).Returns(warehouse);
            
            // Act
            var result = _warehouseController.CreateWarehouse(warehouse);
            
            // Assert
            var createdResult = result.Result as CreatedAtActionResult;
            Assert.IsNotNull(createdResult);
            
            var returnedItems = createdResult.Value as WarehouseCS;
            Assert.IsNotNull(returnedItems);
            Assert.AreEqual(warehouse.Address, returnedItems.Address);
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
        }

        [TestMethod]
        public void UpdatedWarehouseTest_Success()
        {
            // Arrange
            var updatedWarehouse = new WarehouseCS { Id= 1, Code= "X", Name= "cargo hub", Address= "bruv", Zip= "4002 AZ", City= "hub", Province= "Utrecht",
                                                    Country= "GER", Contact= new Dictionary<string, string>{ {"name", "Fem Keijzer"}, {"phone", "(078) 0013363"}, {"email", "blamore@example.net"}}
                                                   };

             _mockWarehouseService.Setup(service => service.UpdateWarehouse(1, updatedWarehouse)).Returns(updatedWarehouse);

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
        }

        [TestMethod]
        public void UpdatedWarehouseTest_Failed()
        {
            // Arrange
            var updatedWarehouse = new WarehouseCS { Id= 1, Code= "X", Name= "cargo hub", Address= "bruv", Zip= "4002 AZ", City= "hub", Province= "Utrecht",
                                                    Country= "GER", Contact= new Dictionary<string, string>{ {"name", "Fem Keijzer"}, {"phone", "(078) 0013363"}, {"email", "blamore@example.net"}}
                                                   };

             _mockWarehouseService.Setup(service => service.UpdateWarehouse(0, updatedWarehouse)).Returns((WarehouseCS)null);

            // Act
            var result = _warehouseController.UpdateWarehouse(0, updatedWarehouse);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(NotFoundObjectResult));
            var createdResult = result.Result as NotFoundObjectResult;
            var returnedWarehouse = createdResult.Value as WarehouseCS;
            Assert.IsNull(returnedWarehouse);
        }
        
        [TestMethod]
        public void DeleteWarehouseTest_Success()
        {
            // Arrange
            var warehouse = new WarehouseCS { Id = 1, Address = "Straat 1" };
            _mockWarehouseService.Setup(service => service.GetWarehouseById(1)).Returns(warehouse);
            
            // Act
            var result = _warehouseController.DeleteWarehouse(1);
            
            // Assert
            Assert.IsInstanceOfType(result, typeof(OkResult));
        }
    }
}

