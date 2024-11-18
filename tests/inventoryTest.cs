using Microsoft.VisualStudio.TestTools.UnitTesting;
using Services;
using Moq;
using Controllers;
using System.Data.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.HttpResults;

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
        public void GetInventoryTotalByItemIdTest_Exists()
        {
            // Arrange: Mock returns a single InventoryCS item (assuming the service does not return a list)
            var inventoryItem = new InventoryCS { Id = 1, item_id = "P01", total_on_hand = 50, total_allocated = 10 };
            _mockInventoryService.Setup(service => service.GetInventoriesForItem("P01")).Returns(inventoryItem);

            // Act: Call the controller method
            var result = _inventoryController.GetInventoryByItemId("P01");

            // Assert: Check if the result is as expected
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult, "Expected OkObjectResult, but got null.");

            Assert.IsNotNull(okResult.Value, "Expected a single InventoryCS item to be returned.");
            
            // Verify the total_on_hand + total_allocated calculation
            Assert.AreEqual(60, okResult.Value, "The total of total_on_hand and total_allocated did not match the expected value.");
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
        [TestMethod]
        public void UpdateInventoryByIdTest_Succes(){
            //arrange
            var inventory = new InventoryCS(){ Id = 1, item_id="ITEM321", total_on_hand= 100};
            _mockInventoryService.Setup(service => service.UpdateInventoryById(1, inventory)).Returns(inventory);

            //Act
            var result = _inventoryController.UpdateInventoryById(1, inventory);
            var resultOk = result.Result as OkObjectResult;
            var patchedinventory = resultOk.Value as InventoryCS;

            //Assert
            Assert.AreEqual(resultOk.StatusCode, 200);
            Assert.IsNotNull(resultOk);
            Assert.IsNotNull(patchedinventory);
            Assert.AreEqual(patchedinventory.Id, inventory.Id);
            Assert.AreEqual(patchedinventory.item_id, inventory.item_id);
            Assert.AreEqual(patchedinventory.total_on_hand, inventory.total_on_hand);
        }
        
        [TestMethod]
        public void DeleteInventoryTest_Exists()
        {
            //arrange
            var inventory = new InventoryCS { Id = 1, item_id = "ITEM123", total_on_hand = 50 };
            _mockInventoryService.Setup(service => service.GetInventoryById(1)).Returns(inventory);
            
            //Act
            var result = _inventoryController.DeleteInventory(1);
            
            //Assert
            Assert.IsInstanceOfType(result, typeof(OkResult));
        }
        [TestMethod]
        public void DeleteInventoriesTest_Succes(){
            //Act
            var idstodel = new List<int>(){1, 2, 3};
            //Arrange
            var result = _inventoryController.DeleteInventories(idstodel);
            var reslutok = result as OkObjectResult;
            // var reslutok = result as OkResult;
            //Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            Assert.AreEqual(reslutok.StatusCode, 200);
        }
    }
}

