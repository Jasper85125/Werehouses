using Microsoft.VisualStudio.TestTools.UnitTesting;
using ServicesV2;
using Moq;
using ControllersV2;
using System.Data.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace inventory.TestsV2
{
    [TestClass]
    public class InventoryTest
    {
        private Mock<IInventoryService> _mockInventoryService;
        private InventoryController _inventoryController;
        private Mock<ILocationService> _mockLocationService;

        [TestInitialize]
        public void Setup()
        {
            _mockInventoryService = new Mock<IInventoryService>();
            _mockLocationService = new Mock<ILocationService>();
            _inventoryController = new InventoryController(_mockInventoryService.Object, _mockLocationService.Object);

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "../../data/inventories.json");
            var inventory = new InventoryCS
            {
                Id = 1,
                item_id = "P01",
                description = "Cool items",
                item_reference = "REF-123",
                Locations = new List<int> { 1, 202, 303 },
                total_on_hand = 50,
                total_expected = 20,
                total_ordered = 15,
                total_allocated = 10,
                total_available = 45,
                created_at = DateTime.Now,
                updated_at = DateTime.Now
            };

            var inventoryList = new List<InventoryCS> { inventory };
            var json = JsonConvert.SerializeObject(inventoryList, Formatting.Indented);

            var directory = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            File.WriteAllText(filePath, json);
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

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin"; 
            httpContext.Items["WarehouseID"] = "1,2,3,4";

            _inventoryController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            //Act
            var value = _inventoryController.GetAllInventories(null, 1, 10);

            //Assert
            var okResult = value.Result as OkObjectResult;
            var returnedItems = okResult.Value as PaginationCS<InventoryCS>;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(2, returnedItems.Data.Count());

            httpContext.Items["UserRole"] = "NoRole";
            _inventoryController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            //act
            var result = _inventoryController.GetAllInventories(null, 1, 10);

            //assert
            var unauthorizedResult = result.Result as UnauthorizedResult;
            Assert.IsNotNull(unauthorizedResult);
            Assert.AreEqual(401, unauthorizedResult.StatusCode);
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

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";

            _inventoryController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            //Act
            var value = _inventoryController.GetInventoryById(1);

            //Assert
            var okResult = value.Result as OkObjectResult;
            var returnedItems = okResult.Value as InventoryCS;
            Assert.IsNotNull(okResult);
            Assert.IsNotNull(okResult.Value);
            Assert.AreEqual(inventories[0].item_id, returnedItems.item_id);

            httpContext.Items["UserRole"] = "NoRole";
            _inventoryController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            //act
            var result = _inventoryController.GetInventoryById(1);

            //assert
            var unauthorizedResult = result.Result as UnauthorizedResult;
            Assert.IsNotNull(unauthorizedResult);
            Assert.AreEqual(401, unauthorizedResult.StatusCode);
        }

        [TestMethod]
        public void GetInventoryByIdTest_WrongId()
        {
            //arrange
            _mockInventoryService.Setup(service => service.GetInventoryById(1)).Returns((InventoryCS)null);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";

            _inventoryController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

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

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";

            _inventoryController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = _inventoryController.GetInventoryByItemId("P01");

            // Assert
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult, "Expected OkObjectResult, but got null.");
            Assert.IsNotNull(okResult.Value, "Expected a single InventoryCS item to be returned.");
            Assert.AreEqual(60, okResult.Value, "The total of total_on_hand and total_allocated did not match the expected value.");

            httpContext.Items["UserRole"] = "NoRole";
            _inventoryController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            //act
            result = _inventoryController.GetInventoryByItemId("P01");

            //assert
            var unauthorizedResult = result.Result as UnauthorizedResult;
            Assert.IsNotNull(unauthorizedResult);
            Assert.AreEqual(401, unauthorizedResult.StatusCode);
        }


        [TestMethod]
        public void CreateInventory_ReturnsCreatedAtActionResult_WithNewInventory()
        {
            // Arrange
            var inventory = new InventoryCS { Id = 1, item_id = "ITEM123", total_on_hand = 50 };
            _mockInventoryService.Setup(service => service.CreateInventory(inventory)).Returns(inventory);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin"; 

            _inventoryController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

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

            httpContext.Items["UserRole"] = "NoRole";
            _inventoryController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            //act
            result = _inventoryController.CreateInventory(inventory);

            //assert
            var unauthorizedResult = result.Result as UnauthorizedResult;
            Assert.IsNotNull(unauthorizedResult);
            Assert.AreEqual(401, unauthorizedResult.StatusCode);
        }

        [TestMethod]
        public void CreateMultipleInventories_ReturnsCreatedResult_WithNewInventories()
        {
            // Arrange
            var inventories = new List<InventoryCS>
            {
                new InventoryCS { item_id="P000001", Locations= new List<int>{1,2,3}, total_on_hand=59, total_expected=10,
                               total_ordered=50, total_allocated=25, total_available=75},
                new InventoryCS { item_id="P000001", Locations= new List<int>{1,2,3}, total_on_hand=59, total_expected=10,
                               total_ordered=50, total_allocated=25, total_available=75},
            };
            _mockInventoryService.Setup(service => service.CreateMultipleInventories(inventories)).Returns(inventories);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";

            _inventoryController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = _inventoryController.CreateMultipleInventories(inventories);
            var createdResult = result.Result as ObjectResult;
            var returnedItems = createdResult.Value as List<InventoryCS>;
            var firstInventory = returnedItems[0];

            // Assert
            Assert.IsNotNull(createdResult);
            Assert.IsNotNull(returnedItems);
            Assert.AreEqual(inventories[0].total_on_hand, firstInventory.total_on_hand);
            Assert.AreEqual(inventories[0].total_allocated, firstInventory.total_allocated);
            Assert.AreEqual(inventories[0].total_available, firstInventory.total_available);

            httpContext.Items["UserRole"] = "NoRole";
            _inventoryController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            //act
            result = _inventoryController.CreateMultipleInventories(inventories);

            //assert
            var unauthorizedResult = result.Result as UnauthorizedResult;
            Assert.IsNotNull(unauthorizedResult);
            Assert.AreEqual(401, unauthorizedResult.StatusCode);
        }

        [TestMethod]
        public void UpdateInventoryByIdTest_Succes()
        {
            //arrange
            var inventory = new InventoryCS() { Id = 1, item_id = "ITEM321", total_on_hand = 100 };
            _mockInventoryService.Setup(service => service.UpdateInventoryById(1, inventory)).Returns(inventory);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";

            _inventoryController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

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

            httpContext.Items["UserRole"] = "NoRole";
            _inventoryController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            //act
            result = _inventoryController.UpdateInventoryById(1, inventory);

            //assert
            var unauthorizedResult = result.Result as UnauthorizedResult;
            Assert.IsNotNull(unauthorizedResult);
            Assert.AreEqual(401, unauthorizedResult.StatusCode);
        }

        [TestMethod]
        public void DeleteInventoryTest_Exists()
        {
            //arrange
            var inventory = new InventoryCS { Id = 1, item_id = "ITEM123", total_on_hand = 50 };
            _mockInventoryService.Setup(service => service.GetInventoryById(1)).Returns(inventory);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";

            _inventoryController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            //Act
            var result = _inventoryController.DeleteInventory(1);

            //Assert
            Assert.IsInstanceOfType(result, typeof(OkResult));

            httpContext.Items["UserRole"] = "NoRole";
            _inventoryController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            //act
            result = _inventoryController.DeleteInventory(1);

            //assert
            var unauthorizedResult = result as UnauthorizedResult;
            Assert.IsNotNull(unauthorizedResult);
            Assert.AreEqual(401, unauthorizedResult.StatusCode);
        }

        [TestMethod]
        public void DeleteInventoriesTest_Succes()
        {
            //Act
            var inventoriesToDelete = new List<int>() { 1, 2, 3 };

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";

            _inventoryController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            //Arrange
            var result = _inventoryController.DeleteInventories(inventoriesToDelete);
            var reslutok = result as OkObjectResult;

            //Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            Assert.AreEqual(reslutok.StatusCode, 200);

            httpContext.Items["UserRole"] = "NoRole";
            _inventoryController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            //act
            result = _inventoryController.DeleteInventories(inventoriesToDelete);

            //assert
            var unauthorizedResult = result as UnauthorizedResult;
            Assert.IsNotNull(unauthorizedResult);
            Assert.AreEqual(401, unauthorizedResult.StatusCode);
        }

        [TestMethod]
        public void PatchInventoryTest_Succes(){
            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";

            _inventoryController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
            
            //arrange
            var inventory = new InventoryCS(){ Id = 1, item_id="ITEM321", total_on_hand= 100};
            _mockInventoryService.Setup(service => service.PatchInventory(1, "total_on_hand", 100)).Returns(inventory);

            //Act
            var result = _inventoryController.PatchInventory(1, "total_on_hand", 100);
            var resultOk = result.Result as OkObjectResult;
            var patchedinventory = resultOk.Value as InventoryCS;

            //Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(resultOk);
            Assert.IsNotNull(patchedinventory);
            Assert.AreEqual(resultOk.StatusCode, 200);
            Assert.AreEqual(typeof(InventoryCS), patchedinventory.GetType());
            Assert.AreEqual(patchedinventory.total_on_hand, inventory.total_on_hand);

            httpContext.Items["UserRole"] = "NoRole";
            _inventoryController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            //act
            result = _inventoryController.PatchInventory(1, "total_on_hand", 100);

            //assert
            var unauthorizedResult = result.Result as UnauthorizedResult;
            Assert.IsNotNull(unauthorizedResult);
            Assert.AreEqual(401, unauthorizedResult.StatusCode);
        }

        [TestMethod]
        public void PatchInventoryTest_Fail(){
            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";

            _inventoryController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            //arrange
            var inventory = new InventoryCS(){ Id = 1, item_id="ITEM321", total_on_hand= 100};
            _mockInventoryService.Setup(service => service.PatchInventory(1, "total_on_hand", 100)).Returns((InventoryCS)null);

            //Act
            var result = _inventoryController.PatchInventory(1, "total_on_hand", 100);
            var resultOk = result.Result as OkObjectResult;
            var patchedinventory = resultOk.Value as InventoryCS;

            //Assert
            Assert.AreEqual(resultOk.StatusCode, 200);
            Assert.IsNull(patchedinventory);
        }
        
    }
}

