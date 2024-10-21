using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using itemtype.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace itemtype.Tests
{
    [TestClass]
    public class ItemTypeServiceTests
    {
        private Mock<IItemtypeService> _mockItemTypeService;
        private List<ItemTypeCS> _itemTypes;

        [TestInitialize]
        public void Setup()
        {
            _mockItemTypeService = new Mock<IItemtypeService>();
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
    }
}