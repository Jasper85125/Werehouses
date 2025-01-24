using Microsoft.VisualStudio.TestTools.UnitTesting;
using ServicesV1;
using Moq;
using ControllersV1;
using System.Data.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.HttpResults;
using Newtonsoft.Json;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;

namespace TestsV1
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
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "../../data/inventories.json");
            var inventory = new InventoryCS(){
                Id= 1,
                item_id= "P000001",
                description="Face-to-face clear-thinking complexity",
                item_reference="sjQ23408K",
                Locations= new List<int>(){
                    3211,
                    24700,
                    14123,
                    19538,
                    31071,
                    24701,
                    11606,
                    11817
                    },
                total_on_hand=262,
                total_expected=0,
                total_ordered=80,
                total_allocated=41,
                total_available=141,
                created_at=DateTime.Now,
                updated_at=DateTime.Now,
            };
            var inventorieslist = new List<InventoryCS>(){ inventory };
            var json = JsonConvert.SerializeObject(inventorieslist);
            var directory = Path.GetDirectoryName(filePath);
            if(!Directory.Exists(directory)){
                Directory.CreateDirectory(directory);
            }
            File.WriteAllText(filePath, json);
        }
        [TestMethod]
        public void GetAllInventoriesService_Test_Succes(){
            var inventoryservice = new InventoryService();
            var result = inventoryservice.GetAllInventories();
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
        }
        [TestMethod]
        public void GetInventoryById_Test_Succes(){
            var inventoryservice = new InventoryService();
            var result = inventoryservice.GetInventoryById(1);
            Assert.IsNotNull(result);
            Assert.AreEqual("P000001", result.item_id);
        }
        [TestMethod]
        public void GetInventoriesForItem_Test_Succes(){
            var inventoryservice = new InventoryService();
            var result = inventoryservice.GetInventoriesForItem("P000001");
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Id);
        }
        [TestMethod]
        public void CreateInventory_Test_Succes(){
            var inventory = new InventoryCS(){
                Id= 2,
                item_id= "P000002",
                description="Focused transitional alliance",
                item_reference="nyg48736S",
                Locations=new List<int>{
                    19800,
                    23653,
                    3068,
                    3334,
                    20477,
                    20524,
                    17579,
                    2271,
                    2293,
                    22717
                    },
                total_on_hand=194,
                total_expected=0,
                total_ordered=139,
                total_allocated=0,
                total_available=55,
                created_at=DateTime.Now,
                updated_at=DateTime.Now
                };
                var inventoryservice = new InventoryService();
                var result = inventoryservice.CreateInventory(inventory);
                var updatedinventories = inventoryservice.GetAllInventories();
                Assert.IsNotNull(result);
                Assert.AreEqual(2, updatedinventories.Count);
                Assert.AreEqual("P000002", updatedinventories[1].item_id);
        }
        [TestMethod]
        public void UpdateInventoryById_Test_Succes(){
            var updatedinventory = new InventoryCS(){
                Id= 1,
                item_id= "P000001",
                description="updated test",
                item_reference="sjQ23408K",
                Locations= new List<int>(){
                    3211,
                    24700,
                    14123,
                    19538,
                    31071,
                    24701,
                    11606,
                    11817
                    },
                total_on_hand=262,
                total_expected=0,
                total_ordered=80,
                total_allocated=41,
                total_available=141,
                created_at=DateTime.Now,
                updated_at=DateTime.Now,
            };
            var inventoryservice = new InventoryService();
            var result = inventoryservice.UpdateInventoryById(1, updatedinventory);
            Assert.IsNotNull(result);
            Assert.AreEqual("updated test", result.description);
        }
        [TestMethod]
        public void DeleteInventoryService_Test_Succes(){
            var inventoryservice = new InventoryService();
            inventoryservice.DeleteInventory(1);
            var updatedinventories = inventoryservice.GetAllInventories();
            Assert.AreEqual(0, updatedinventories.Count);
        }
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
            var okResult = value.Result as OkObjectResult;
            var returnedItems = okResult.Value as IEnumerable<InventoryCS>;

            //Assert
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
            var okResult = value.Result as OkObjectResult;
            var returnedItems = okResult.Value as InventoryCS;

            //Assert
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
            // Arrange
            var inventoryItem = new InventoryCS { Id = 1, item_id = "P01", total_on_hand = 50, total_allocated = 10 };
            _mockInventoryService.Setup(service => service.GetInventoriesForItem("P01")).Returns(inventoryItem);

            // Act
            var result = _inventoryController.GetInventoryByItemId("P01");
            var okResult = result.Result as OkObjectResult;

            // Assert
            Assert.IsNotNull(okResult, "Expected OkObjectResult, but got null.");
            Assert.IsNotNull(okResult.Value, "Expected a single InventoryCS item to be returned.");
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
            var createdAtActionResult = result.Result as CreatedAtActionResult;
            var returnedInventory = createdAtActionResult.Value as InventoryCS;

            // Assert
            Assert.IsNotNull(createdAtActionResult);
            Assert.IsNotNull(returnedInventory);
            Assert.AreEqual(1, returnedInventory.Id);
            Assert.AreEqual("ITEM123", returnedInventory.item_id);
            Assert.AreEqual(50, returnedInventory.total_on_hand);
        }

        [TestMethod]
        public void UpdateInventoryByIdTest_Succes()
        {
            //arrange
            var inventory = new InventoryCS() { Id = 1, item_id = "ITEM321", total_on_hand = 100 };
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

    }
}

