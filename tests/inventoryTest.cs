using Microsoft.VisualStudio.TestTools.UnitTesting;
using Services;
using Moq;
using Controllers;
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
        [TestMethod] 
        public void CreateInventory_ReturnsCreatedAtActionResult_WithNewInventory()
        {
            // Arrange
            var inventory = new InventoryCS { Id = 1, item_id = "ITEM123", total_on_hand = 50 };
            _mockInventoryService.Setup(service => service.CreateInventory(inventory)).Returns(inventory);

            // Act
            var result = _inventoryController.CreateInventory(inventory);

            // Assert
            var createdAtActionResult = result.Result as CreatedAtActionResult;
            var returnedInventory = createdAtActionResult.Value as InventoryCS;
            Assert.IsNotNull(createdAtActionResult);  // Verify that the result is CreatedAtActionResult
            Assert.IsNotNull(returnedInventory);  // Verify that the returned object is not null
            Assert.AreEqual(1, returnedInventory.Id);  // Verify that the returned object has the expected ID
            Assert.AreEqual("ITEM123", returnedInventory.item_id);  // Verify that the returned object has the expected ItemId
            Assert.AreEqual(50, returnedInventory.total_on_hand);  // Verify that the returned object has the expected Quantity
        }
    }
}

