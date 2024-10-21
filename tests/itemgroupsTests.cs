using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Services;
using Controllers;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace itemgroup.Tests
{
    [TestClass]
    public class ItemGroupTests
    {
        private Mock<IitemGroupService> _mockItemGroupService;
        private ItemGroupController _itemGroupController;

        [TestInitialize]
        public void Setup()
        {
            _mockItemGroupService = new Mock<IitemGroupService>();
            _itemGroupController = new ItemGroupController(_mockItemGroupService.Object);
        }

        [TestMethod]
        public void GetAllItemGroupsTest_Exists()
        {
            // Arrange
            var itemGroups = new List<ItemGroupCS>
            {
                new ItemGroupCS { Id = 1, Name = "Group 1" },
                new ItemGroupCS { Id = 2, Name = "Group 2" }
            };
            _mockItemGroupService.Setup(service => service.GetAllItemGroups()).Returns(itemGroups);

            // Act
            var result = _itemGroupController.GetAllItemGroups();

            // Assert
            var okResult = result.Result as OkObjectResult;
            var returnedItems = okResult.Value as IEnumerable<ItemGroupCS>;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(2, returnedItems.Count());
        }

        [TestMethod]
        public void GetItemGroupByIdTest_Exists()
        {
            // Arrange
            var itemGroup = new ItemGroupCS { Id = 1, Name = "Group 1" };
            _mockItemGroupService.Setup(service => service.GetItemById(1)).Returns(itemGroup);

            // Act
            var result = _itemGroupController.GetItemById(1);

            // Assert
            var okResult = result.Result as OkObjectResult;
            var returnedItem = okResult.Value as ItemGroupCS;
            Assert.IsNotNull(okResult);
            Assert.IsNotNull(okResult.Value);
            Assert.AreEqual(itemGroup.Name, returnedItem.Name);
        }

        [TestMethod]
        public void GetItemGroupByIdTest_WrongId()
        {
            // Arrange
            _mockItemGroupService.Setup(service => service.GetItemById(1)).Returns((ItemGroupCS)null);

            // Act
            var result = _itemGroupController.GetItemById(1);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));
        }
    }
}