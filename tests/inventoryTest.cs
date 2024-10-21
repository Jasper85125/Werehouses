using Microsoft.VisualStudio.TestTools.UnitTesting;
using warehouse.Services;
using Moq;
using inventories.Controllers;
using System.Data.Common;
using Microsoft.AspNetCore.Mvc;

namespace Tests
{
    [TestClass]
    public class InventoryTest
    {
        private Mock<IInventoryService> _mockInventoryService;
        private InventoryController _inventoryController;

        [TestInitialize]
        public void Setup()
        {
            _mockInventoryService = new Mock<IInventoryService>();
            _inventoryController = new InventoryController(_mockInventoryService.Object);
        }

        [TestMethod]
        public void GetInventoriesTest_Exists()
        {
            //arrange
            var inventories = new List<InventoryCS>
            {
                new InventoryCS { Id = 1, item_id = "P01", description = "Big blocks", item_reference = "LBJ" },
                new InventoryCS { Id = 2, item_id = "P01", description = "Bricks", item_reference = "LBJ jr" }
            };
            _mockInventoryService.Setup(service => service.GetAllInventories()).Returns(inventories);
            
            //Act
            var value = _inventoryController.GetAllInventories();
            
            //Assert
            var okResult = value.Result as OkObjectResult;
            var returnedItems = okResult.Value as IEnumerable<InventoryCS>;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(2, returnedItems.Count());
        }

        [TestMethod]
        public void GetInventoryByIdTest_Exists()
        {
            //arrange
            var inventories = new List<InventoryCS>
            {
                new InventoryCS { Id = 1, item_id = "P01", description = "Big blocks", item_reference = "LBJ" },
                new InventoryCS { Id = 2, item_id = "P01", description = "Bricks", item_reference = "LBJ jr" }
            };
            _mockInventoryService.Setup(service => service.GetInventoryById(1)).Returns(inventories[0]);
            
            //Act
            var value = _inventoryController.GetInventoryById(1);
            
            //Assert
            var okResult = value.Result as OkObjectResult;
            var returnedItems = okResult.Value as InventoryCS;
            Assert.IsNotNull(okResult);
            Assert.IsNotNull(okResult.Value);
            Assert.AreEqual(inventories[0].item_id, returnedItems.item_id);
        }

        [TestMethod]
        public void GetInventoryByIdTest_WrongId()
        {
            //arrange
            _mockInventoryService.Setup(service => service.GetInventoryById(1)).Returns((InventoryCS)null);
            
            //Act
            var value = _inventoryController.GetInventoryById(1);
            
            //Assert
            Assert.IsInstanceOfType(value.Result, typeof(NotFoundResult));
        }
    }
}

