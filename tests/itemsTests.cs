using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using item.Controllers;
using item.Services;
using System.Collections.Generic;
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
    }
}