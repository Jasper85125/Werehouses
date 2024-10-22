using Microsoft.VisualStudio.TestTools.UnitTesting;
using Services;
using Moq;
using Controllers;
using System.Data.Common;
using Microsoft.AspNetCore.Mvc;

namespace Tests
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
            //arrange
            var warehouses = new List<WarehouseCS>
            {
                new WarehouseCS { Id = 1, Address = "Straat 1" },
                new WarehouseCS { Id = 2, Address = "Warenhuislaan 280" }
            };
            _mockWarehouseService.Setup(service => service.GetAllWarehouses()).Returns(warehouses);
            
            //Act
            var value = _warehouseController.GetAllWarehouses();
            
            //Assert
            var okResult = value.Result as OkObjectResult;
            var returnedItems = okResult.Value as IEnumerable<WarehouseCS>;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(2, returnedItems.Count());
        }

        [TestMethod]
        public void GetWarehouseByIdTest_Exists()
        {
            //arrange
            var warehouses = new List<WarehouseCS>
            {
                new WarehouseCS { Id = 1, Address = "Straat 1" },
                new WarehouseCS { Id = 2, Address = "Warenhuislaan 280" }
            };
            _mockWarehouseService.Setup(service => service.GetWarehouseById(1)).Returns(warehouses[0]);
            
            //Act
            var value = _warehouseController.GetWarehouseById(1);
            
            //Assert
            var okResult = value.Result as OkObjectResult;
            var returnedItems = okResult.Value as WarehouseCS;
            Assert.IsNotNull(okResult);
            Assert.IsNotNull(okResult.Value);
            Assert.AreEqual(warehouses[0].Address, returnedItems.Address);
        }

        [TestMethod]
        public void GetWarehouseByIdTest_WrongId()
        {
            //arrange
            _mockWarehouseService.Setup(service => service.GetWarehouseById(1)).Returns((WarehouseCS)null);
            
            //Act
            var value = _warehouseController.GetWarehouseById(1);
            
            //Assert
            Assert.IsInstanceOfType(value.Result, typeof(NotFoundResult));
        }

        [TestMethod]
        public void CreateWarehouse_ReturnsCreatedResult_WithNewWarehouse()
        {
            // Arrange
            var warehouse = new WarehouseCS { Id = 1, Address = "Straat 1" };
            _mockWarehouseService.Setup(service => service.CreateWarehouse(warehouse)).Returns(warehouse);
            
            // Act
            var value = _warehouseController.CreateWarehouse(warehouse);
            
            // Assert
            var createdResult = value.Result as CreatedAtActionResult;  // Use CreatedAtActionResult here
            Assert.IsNotNull(createdResult);
            
            var returnedItems = createdResult.Value as WarehouseCS;
            Assert.IsNotNull(returnedItems);
            Assert.AreEqual(warehouse.Address, returnedItems.Address);
        }
    }
}

