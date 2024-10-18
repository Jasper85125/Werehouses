using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using item.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace itemtests
{
    [TestClass]
    public class ItemControllerTests
    {
        private ItemController _itemController = null!;
        private Mock<IItemService> _mockItemService = null!;

        // This method runs before each test to set up the necessary objects.
        [TestInitialize]
        public void Setup()
        {
            _mockItemService = new Mock<IItemService>();
            _itemController = new ItemController(_mockItemService.Object);
        }

        // Test to verify that GetAllItems returns an OkResult with a list of items.
        [TestMethod]
        public void GetAllItems_ReturnsOkResult_WithListOfItems()
        {
            // Arrange: Set up the mock service to return a list of items.
            var items = new List<ItemCS>
            {
                new ItemCS { Uid = "1", Code = "Item1" },
                new ItemCS { Uid = "2", Code = "Item2" }
            };
            _mockItemService.Setup(service => service.GetAllItems()).Returns(items);

            // Act: Call the GetAllItems method on the controller.
            var result = _itemController.GetAllItems();

            // Assert: Verify that the result is an OkObjectResult with the expected items.
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            var returnedItems = okResult.Value as List<ItemCS>;
            Assert.IsNotNull(returnedItems);
            Assert.AreEqual(2, returnedItems.Count);
        }

        // Test to verify that GetAllItems returns an OkResult with an empty list.
        [TestMethod]
        public void GetAllItems_ReturnsOkResult_WithEmptyList()
        {
            // Arrange: Set up the mock service to return an empty list.
            var items = new List<ItemCS>();
            _mockItemService.Setup(service => service.GetAllItems()).Returns(items);

            // Act: Call the GetAllItems method on the controller.
            var result = _itemController.GetAllItems();

            // Assert: Verify that the result is an OkObjectResult with an empty list.
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            var returnedItems = okResult.Value as List<ItemCS>;
            Assert.IsNotNull(returnedItems);
            Assert.AreEqual(0, returnedItems.Count);
        }
    }
}