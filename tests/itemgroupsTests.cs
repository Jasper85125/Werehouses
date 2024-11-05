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

        [TestMethod]
        public void CreateItemGroupTest_Success()
        {
            // Arrange
            var newItemGroup = new ItemGroupCS { Id = 3, Name = "Group 3" };
            _mockItemGroupService.Setup(service => service.CreateItemGroup(It.IsAny<ItemGroupCS>())).Returns(Task.FromResult(newItemGroup));

            // Act
            var result = _itemGroupController.CreateItemGroup(newItemGroup);

            // Assert
            var createdResult = result.Result as CreatedAtActionResult;
            var returnedItem = createdResult.Value as ItemGroupCS;
            Assert.IsNotNull(createdResult);
            Assert.AreEqual(newItemGroup.Name, returnedItem.Name);
        }

        [TestMethod]
        public async Task UpdateItemGroupTest_ValidItem()
        {
            // Arrange
            var existingItemGroup = new ItemGroupCS { Id = 1, Description = "Existing Item" };
            var updatedItemGroup = new ItemGroupCS { Id = 1, Description = "Updated Item" };
            _mockItemGroupService.Setup(service => service.GetItemById(1)).Returns(existingItemGroup);
            _mockItemGroupService.Setup(service => service.UpdateItemGroup(1, updatedItemGroup)).ReturnsAsync(updatedItemGroup);

            // Act
            var value = await _itemGroupController.UpdateItemGroup(1, updatedItemGroup);

            // Assert
            var okResult = value.Result as OkObjectResult;
            var returnedItem = okResult.Value as ItemGroupCS;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(updatedItemGroup.Description, returnedItem.Description);
        }

        [TestMethod]
        public async Task UpdateItemGroupTest_WrongId()
        {
            // Arrange
            var updatedItemGroup = new ItemGroupCS { Id = 1, Description = "Updated Item" };
            _mockItemGroupService.Setup(service => service.GetItemById(1)).Returns((ItemGroupCS)null);

            // Act
            var value = await _itemGroupController.UpdateItemGroup(1, updatedItemGroup);

            // Assert
            Assert.IsInstanceOfType(value.Result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task UpdateItemGroupTest_IdMismatch()
        {
            // Arrange
            var updatedItemGroup = new ItemGroupCS { Id = 2, Description = "Updated Item" };

            // Act
            var value = await _itemGroupController.UpdateItemGroup(1, updatedItemGroup);

            // Assert
            Assert.IsInstanceOfType(value.Result, typeof(BadRequestResult));
        }

        [TestMethod]
        public void DeleteItemGroupTest_Exists()
        {
            // Arrange
            var itemGroup = new ItemGroupCS { Id = 1, Name = "Group 1" };
            _mockItemGroupService.Setup(service => service.GetItemById(1)).Returns(itemGroup); 
            // Act
            var result = _itemGroupController.DeleteItemGroup(1);
            //assert
            var okResult = result as OkObjectResult;
            
        }
    }
}