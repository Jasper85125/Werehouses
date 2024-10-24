using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Controllers;
using Services;
using Microsoft.AspNetCore.Mvc;

namespace item.Tests
{
    [TestClass]
    public class ItemControllerTests
    {
        private Mock<IItemService> _mockItemService;
        private ItemController _itemController;

        [TestInitialize]
        public void Setup()
        {
            _mockItemService = new Mock<IItemService>();
            _itemController = new ItemController(_mockItemService.Object);
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

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.IsInstanceOfType(okResult.Value, typeof(ItemCS));
            var returnedItem = okResult.Value as ItemCS;
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
    }
}