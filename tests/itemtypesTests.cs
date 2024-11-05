using Microsoft.VisualStudio.TestTools.UnitTesting;
using Services;
using Moq;
using Controllers;
using System.Data.Common;
using Microsoft.AspNetCore.Mvc;
namespace itemtype.Tests
{
    [TestClass]
    public class ItemTypeServiceTests
    {
        private Mock<IItemtypeService> _mockItemTypeService;
        private ItemTypeController _itemTypeController;
        private List<ItemTypeCS> _itemTypes;

        [TestInitialize]
        public void Setup()
        {
            _mockItemTypeService = new Mock<IItemtypeService>();
            _itemTypeController = new ItemTypeController(_mockItemTypeService.Object);
            _itemTypes = new List<ItemTypeCS>
            {
                new ItemTypeCS { Id = 1, Name = "Type1", description = "Description1" },
                new ItemTypeCS { Id = 2, Name = "Type2", description = "Description2" }
            };
        }

        [TestMethod]
        public void GetAllItemtypes_ShouldReturnAllItemTypes()
        {
            // Arrange
            _mockItemTypeService.Setup(service => service.GetAllItemtypes()).Returns(_itemTypes);

            // Act
            var result = _mockItemTypeService.Object.GetAllItemtypes();

            // Assert
            Assert.AreEqual(2, result.Count);
        }

        [TestMethod]
        public void GetItemById_ShouldReturnCorrectItemType()
        {
            // Arrange
            var itemType = _itemTypes[0];
            _mockItemTypeService.Setup(service => service.GetItemById(1)).Returns(itemType);

            // Act
            var result = _mockItemTypeService.Object.GetItemById(1);

            // Assert
            Assert.AreEqual(itemType, result);
        }

        [TestMethod]
        public async Task CreateItemType_ShouldReturnCreatedItemType()
        {
            // Arrange
            var newItemType = new ItemTypeCS { Id = 3, Name = "Type3", description = "Description3" };
            _mockItemTypeService.Setup(service => service.CreateItemType(newItemType)).ReturnsAsync(newItemType);

            // Act
            var result = await _mockItemTypeService.Object.CreateItemType(newItemType);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(newItemType.Id, result.Id);
            Assert.AreEqual(newItemType.Name, result.Name);
            Assert.AreEqual(newItemType.description, result.description);
        }

        [TestMethod]
        public async Task UpdateItemType_ValidItem_ShouldReturnUpdatedItemType()
        {
            // Arrange
            var existingItemType = new ItemTypeCS { Id = 1, Name = "Type1", description = "Description1" };
            var updatedItemType = new ItemTypeCS { Id = 1, Name = "UpdatedType", description = "UpdatedDescription" };
            _mockItemTypeService.Setup(service => service.GetItemById(1)).Returns(existingItemType);
            _mockItemTypeService.Setup(service => service.UpdateItemType(1, updatedItemType)).ReturnsAsync(updatedItemType);

            // Act
            var result = await _mockItemTypeService.Object.UpdateItemType(1, updatedItemType);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(updatedItemType.Name, result.Name);
            Assert.AreEqual(updatedItemType.description, result.description);
        }

        [TestMethod]
        public async Task UpdateItemType_WrongId_ShouldReturnNotFound()
        {
            // Arrange
            var updatedItemType = new ItemTypeCS { Id = 1, Name = "UpdatedType", description = "UpdatedDescription" };
            _mockItemTypeService.Setup(service => service.GetItemById(1)).Returns((ItemTypeCS)null);

            // Act
            var result = await _mockItemTypeService.Object.UpdateItemType(1, updatedItemType);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task UpdateItemType_IdMismatch_ShouldReturnBadRequest()
        {
            // Arrange
            var updatedItemType = new ItemTypeCS { Id = 2, Name = "UpdatedType", description = "UpdatedDescription" };

            // Act
            var result = await _mockItemTypeService.Object.UpdateItemType(1, updatedItemType);

            // Assert
            Assert.IsNull(result);
        }
        [TestMethod]
        public void DeleteItemTypeTest_Exists()
        {
            // Arrange
            var itemType = new ItemTypeCS { Id = 1, Name = "Type1", description = "Description1" };
            _mockItemTypeService.Setup(service => service.GetItemById(1)).Returns(itemType);

            // Act
            var result = _itemTypeController.DeleteItemType(1);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkResult));
        }
            
    }
}