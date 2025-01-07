using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ControllersV2;
using ServicesV2;
// using ModelsV2;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace item.TestsV2
{
    [TestClass]
    public class ItemControllerTests
    {
        private Mock<IItemService> _mockItemService;
        private Mock<IInventoryService> _mockInventoryService;
        private Mock<IItemtypeService> _mockItemTypeService;
        private ItemController _itemController;
        private ItemTypeController _itemTypeController;

        [TestInitialize]
        public void Setup()
        {
            _mockItemService = new Mock<IItemService>();
            _mockItemTypeService = new Mock<IItemtypeService>();
            _mockInventoryService = new Mock<IInventoryService>();
            _itemController = new ItemController(_mockItemService.Object, _mockInventoryService.Object);
            _itemTypeController = new ItemTypeController(_mockItemTypeService.Object, _mockItemService.Object);
        }

        [TestMethod]
        public void GetAllItems_ReturnsOkResult_WithListOfItems()
        {
            // Arrange
            var items = new List<ItemCS>
            {
                new ItemCS { uid = "1", code = "Item1" },
                new ItemCS { uid = "2", code = "Item2" }
            };
            _mockItemService.Setup(service => service.GetAllItems()).Returns(items);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";  // Set the UserRole in HttpContext

            // Assign HttpContext to the controller
            _itemController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = _itemController.GetAllItems(null, 1, 10);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.IsInstanceOfType(okResult.Value, typeof(PaginationCS<ItemCS>));
            var returnedItems = okResult.Value as PaginationCS<ItemCS>;
            Assert.AreEqual(2, returnedItems.Data.Count());
        }

        [TestMethod]
        public void GetByUid_ReturnsOkResult_WithItem()
        {
            // Arrange
            var item = new ItemCS { uid = "1", code = "Item1" };
            _mockItemService.Setup(service => service.GetItemById("1")).Returns(item);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";  // Set the UserRole in HttpContext

            // Assign HttpContext to the controller
            _itemController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = _itemController.GetByUid("1");
            var okResult = result.Result as OkObjectResult;
            var returnedItem = okResult.Value as ItemCS;

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            Assert.IsNotNull(okResult);
            Assert.IsInstanceOfType(okResult.Value, typeof(ItemCS));
            Assert.AreEqual("1", returnedItem.uid);
            Assert.AreEqual("Item1", returnedItem.code);
        }

        [TestMethod]
        public void GetByUid_ReturnsNotFound_WhenItemDoesNotExist()
        {
            // Arrange
            _mockItemService.Setup(service => service.GetItemById("1")).Returns((ItemCS)null);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";  // Set the UserRole in HttpContext

            // Assign HttpContext to the controller
            _itemController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = _itemController.GetByUid("1");

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));
        }

        [TestMethod]
        public void GetInventoryForItem_Success()
        {
            InventoryCS inventory = new InventoryCS { Id = 3, item_id = "P000003", description = "gamers", item_reference = "QVm03739H",
                                                      Locations = new List<int> {5321, 21960}, total_on_hand = 24, total_expected = 0,
                                                      total_ordered = 90, total_allocated = 68, total_available = -134};
            // Arrange
            _mockInventoryService.Setup(service => service.GetInventoriesForItem("P000003")).Returns(inventory);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";  // Set the UserRole in HttpContext

            // Assign HttpContext to the controller
            _itemController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = _itemController.GetInventoriesForItem("P000003");
            var okResult = result.Result as OkObjectResult;
            var returnedItem = okResult.Value as InventoryCS;

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            Assert.IsNotNull(okResult);
            Assert.IsInstanceOfType(okResult.Value, typeof(InventoryCS));
            Assert.AreEqual("P000003", returnedItem.item_id);
            Assert.AreEqual(24, returnedItem.total_on_hand);
            Assert.AreEqual(68, returnedItem.total_allocated);
        }

        [TestMethod]
        public void GetItemsWithItemType_Success()
        {
            // Arrange
            List<ItemCS> items = new List<ItemCS>
            {
                new ItemCS { uid = "P000123", code = "CRD57317J", description = "Organic asymmetric data-warehouse",
                                       short_description = "particularly", upc_code = "9538419150098", item_line = 33,
                                       item_group = 2, item_type= 1, supplier_id = 28, supplier_code = "SUP467"},
                new ItemCS { uid = "P100000", code = "CRD57317J", description = "Organic asymmetric data-warehouse",
                                       short_description = "particularly", upc_code = "9538419150098", item_line = 33,
                                       item_group = 2, item_type= 1, supplier_id = 28, supplier_code = "SUP467"}
            };

            _mockItemService.Setup(service => service.GetAllItemsInItemType(1)).Returns(items);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";  // Set the UserRole in HttpContext

            // Assign HttpContext to the controller
            _itemController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
            _itemTypeController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = _itemTypeController.GetAllItemsInItemType(1);
            var okResult = result.Result as OkObjectResult;
            var returnedItems = okResult.Value as IEnumerable<ItemCS>;

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            Assert.IsNotNull(okResult);
            Assert.IsInstanceOfType(okResult.Value, typeof(IEnumerable<ItemCS>));
            Assert.AreEqual(2, returnedItems.Count());
        }

        [TestMethod]
        public void CreateItem_ReturnsCreatedResult_WithNewItem()
        {
            // Arrange
            var newItem = new ItemCS { uid = "P000001", code = "NewItem" };
            var createdItem = new ItemCS { uid = "P000002", code = "NewItem" };
            _mockItemService.Setup(service => service.CreateItem(newItem)).Returns(createdItem);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";  // Set the UserRole in HttpContext

            // Assign HttpContext to the controller
            _itemController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = _itemController.CreateItem(newItem);
            var createdResult = result.Result as CreatedAtActionResult;
            var returnedItem = createdResult.Value as ItemCS;

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(CreatedAtActionResult));
            Assert.IsNotNull(createdResult);
            Assert.IsInstanceOfType(createdResult.Value, typeof(ItemCS));
            Assert.AreEqual("P000002", returnedItem.uid);
            Assert.AreEqual("NewItem", returnedItem.code);
        }

         [TestMethod]
        public void CreateMultipleItems_ReturnsCreatedResult_WithNewItems()
        {
            // Arrange
            List<ItemCS> items = new List<ItemCS>
            {
                new ItemCS { uid = "P000123", code = "CRD57317J", description = "Organic asymmetric data-warehouse",
                                       short_description = "particularly", upc_code = "9538419150098", item_line = 33,
                                       item_group = 2, item_type= 1, supplier_id = 28, supplier_code = "SUP467"},
                new ItemCS { uid = "P100000", code = "CRD57317J", description = "Organic asymmetric data-warehouse",
                                       short_description = "particularly", upc_code = "9538419150098", item_line = 33,
                                       item_group = 2, item_type= 1, supplier_id = 28, supplier_code = "SUP467"}
            };
            _mockItemService.Setup(service => service.CreateMultipleItems(items)).Returns(items);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";  // Set the UserRole in HttpContext

            // Assign HttpContext to the controller
           _itemController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result =_itemController.CreateMultipleItems(items);
            var createdResult = result.Result as ObjectResult;
            var returnedItems = createdResult.Value as List<ItemCS>;
            var firstItem = returnedItems[0];

            // Assert
            Assert.IsNotNull(createdResult);
            Assert.IsNotNull(returnedItems);
            Assert.AreEqual(items[0].code, firstItem.code);
            Assert.AreEqual(items[0].supplier_code, firstItem.supplier_code);
            Assert.AreEqual(items[0].item_group, firstItem.item_group);
        }

        [TestMethod]
        public void UpdateItem_ReturnsOkResult_WithUpdatedItem()
        {
            // Arrange
            var existingItem = new ItemCS { uid = "P000001", code = "ExistingItem" };
            var updatedItem = new ItemCS { uid = "P000001", code = "UpdatedItem" };
            _mockItemService.Setup(service => service.GetItemById("P000001")).Returns(existingItem);
            _mockItemService.Setup(service => service.UpdateItem("P000001", updatedItem)).Returns(updatedItem);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";  // Set the UserRole in HttpContext

            // Assign HttpContext to the controller
            _itemController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = _itemController.UpdateItem("P000001", updatedItem);
            var okResult = result.Result as OkObjectResult;
            var returnedItem = okResult.Value as ItemCS;

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            Assert.IsNotNull(okResult);
            Assert.IsInstanceOfType(okResult.Value, typeof(ItemCS));
            Assert.AreEqual("P000001", returnedItem.uid);
            Assert.AreEqual("UpdatedItem", returnedItem.code);
        }

        [TestMethod]
        public void UpdateItem_ReturnsNotFound_WhenItemDoesNotExist()
        {
            // Arrange
            var updatedItem = new ItemCS { uid = "P000001", code = "UpdatedItem" };
            _mockItemService.Setup(service => service.GetItemById("P000001")).Returns((ItemCS)null);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";  // Set the UserRole in HttpContext

            // Assign HttpContext to the controller
            _itemController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = _itemController.UpdateItem("P000001", updatedItem);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));
        }

        [TestMethod]
        public void PatchItem_Succes(){
            //Arrange
            var patcheditem = new ItemCS(){ uid="P000001", code="lol no"};
            _mockItemService.Setup(service=>service.PatchItem("P000001", "code", "lol no")).Returns(patcheditem);
            //Act
            var result = _itemController.PatchItem("P000001", "code", "lol no");
            var resultok = result.Result as OkObjectResult;
            var value = resultok.Value as ItemCS;
            //Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(resultok);
            Assert.IsNotNull(value);
            Assert.AreEqual(resultok.StatusCode, 200);
            Assert.AreEqual(value.code, "lol no");
        }
        [TestMethod]
        public void DeleteItem_ReturnsOkResult_WhenItemIsDeleted()
        {
            // Arrange
            var existingItem = new ItemCS { uid = "P000001", code = "ExistingItem" };
            _mockItemService.Setup(service => service.GetItemById("P000001")).Returns(existingItem);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";  // Set the UserRole in HttpContext

            // Assign HttpContext to the controller
            _itemController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = _itemController.DeleteItem("P000001");

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkResult));
        }

        [TestMethod]
        public void DeleteItems_ReturnsOkResult_WhenItemsAreDeleted()
        {
            // Arrange
            var existingItems = new List<ItemCS>
            {
                new ItemCS { uid = "P000001", code = "ExistingItem1" },
                new ItemCS { uid = "P000002", code = "ExistingItem2" }
            };
            _mockItemService.Setup(service => service.GetAllItems()).Returns(existingItems);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";  // Set the UserRole in HttpContext

            // Assign HttpContext to the controller
            _itemController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = _itemController.DeleteMultipleItems(new List<string> { "P000001", "P000002" });
            
            // Assert
            Assert.IsInstanceOfType(result, typeof(OkResult));
        }
    }
}
