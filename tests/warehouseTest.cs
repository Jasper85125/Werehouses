using Microsoft.VisualStudio.TestTools.UnitTesting;
using warehouse.Services;
using Moq;
using warehouses.Controllers;
using System.Data.Common;
using Microsoft.AspNetCore.Mvc;

namespace Tests
{
    [TestClass]
    public class WarehouseTest
    {
<<<<<<< HEAD
        
        [TestMethod]
        public void GetWarehousesTest()
=======
        private Mock<IWarehouseService> _mockWarehouseService;
        private WarehouseController _warehouseController;

        [TestInitialize]
        public void Setup()
>>>>>>> c83730f4c475e48a05089c788e5df93f8c9ea5b9
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
    }
}

