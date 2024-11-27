using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ControllersV1;
using ServicesV1;
using Microsoft.AspNetCore.Mvc;

namespace item.Tests
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

            // Act
            var result = _itemController.GetAllItems();

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.IsInstanceOfType(okResult.Value, typeof(IEnumerable<ItemCS>));
            var returnedItems = okResult.Value as IEnumerable<ItemCS>;
            Assert.AreEqual(2, returnedItems.Count());
        }

        [TestMethod]
        public void GetByUid_ReturnsOkResult_WithItem()
        {
            // Arrange
            var item = new ItemCS { uid = "1", code = "Item1" };
            _mockItemService.Setup(service => service.GetItemById("1")).Returns(item);

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

            // Act
            var result = _itemController.CreateItem(newItem);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(CreatedAtActionResult));
            var createdResult = result.Result as CreatedAtActionResult;
            Assert.IsNotNull(createdResult);
            Assert.IsInstanceOfType(createdResult.Value, typeof(ItemCS));
            var returnedItem = createdResult.Value as ItemCS;
            Assert.AreEqual("P000002", returnedItem.uid);
            Assert.AreEqual("NewItem", returnedItem.code);
        }

        [TestMethod]
        public void UpdateItem_ReturnsOkResult_WithUpdatedItem()
        {
            // Arrange
            var existingItem = new ItemCS { uid = "P000001", code = "ExistingItem" };
            var updatedItem = new ItemCS { uid = "P000001", code = "UpdatedItem" };
            _mockItemService.Setup(service => service.GetItemById("P000001")).Returns(existingItem);
            _mockItemService.Setup(service => service.UpdateItem("P000001", updatedItem)).Returns(updatedItem);

            // Act
            var result = _itemController.UpdateItem("P000001", updatedItem);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.IsInstanceOfType(okResult.Value, typeof(ItemCS));
            var returnedItem = okResult.Value as ItemCS;
            Assert.AreEqual("P000001", returnedItem.uid);
            Assert.AreEqual("UpdatedItem", returnedItem.code);
        }

        [TestMethod]
        public void UpdateItem_ReturnsNotFound_WhenItemDoesNotExist()
        {
            // Arrange
            var updatedItem = new ItemCS { uid = "P000001", code = "UpdatedItem" };
            _mockItemService.Setup(service => service.GetItemById("P000001")).Returns((ItemCS)null);

            // Act
            var result = _itemController.UpdateItem("P000001", updatedItem);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));
        }
        [TestMethod]
        public void DeleteItem_ReturnsOkResult_WhenItemIsDeleted()
        {
            // Arrange
            var existingItem = new ItemCS { uid = "P000001", code = "ExistingItem" };
            _mockItemService.Setup(service => service.GetItemById("P000001")).Returns(existingItem);

            // Act
            var result = _itemController.DeleteItem("P000001");

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkResult));
        }
    }
}