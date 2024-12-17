using Microsoft.VisualStudio.TestTools.UnitTesting;
using ServicesV2;
using Moq;
using ControllersV2;
using System.Data.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace itemtype.TestsV2
{
    [TestClass]
    public class ItemTypeServiceTests
    {
        private Mock<IItemtypeService> _mockItemTypeService;
        private Mock<IItemService> _mockItemService;
        private ItemTypeController _itemTypeController;
        private List<ItemTypeCS> _itemTypes;
        private Mock<Iactionlogservice> _actionlogservice; 
        [TestInitialize]
        public void Setup()
        {
            _mockItemTypeService = new Mock<IItemtypeService>();
            _mockItemService = new Mock<IItemService>();
            _itemTypeController = new ItemTypeController(_mockItemTypeService.Object, _mockItemService.Object);
            _actionlogservice = new Mock<Iactionlogservice>();
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

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";  // Set the UserRole in HttpContext

            // Assign HttpContext to the controller
            _itemTypeController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = _mockItemTypeService.Object.GetItemById(1);

            // Assert
            Assert.AreEqual(itemType, result);
        }

        [TestMethod]
        public void CreateItemType_ShouldReturnCreatedItemType()
        {
            // Arrange
            var newItemType = new ItemTypeCS { Id = 3, Name = "Type3", description = "Description3" };
            _mockItemTypeService.Setup(service => service.CreateItemType(newItemType)).Returns(newItemType);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";  // Set the UserRole in HttpContext

            // Assign HttpContext to the controller
            _itemTypeController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = _mockItemTypeService.Object.CreateItemType(newItemType);

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

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";  // Set the UserRole in HttpContext

            // Assign HttpContext to the controller
            _itemTypeController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

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

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";  // Set the UserRole in HttpContext

            // Assign HttpContext to the controller
            _itemTypeController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

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

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";  // Set the UserRole in HttpContext

            // Assign HttpContext to the controller
            _itemTypeController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = await _mockItemTypeService.Object.UpdateItemType(1, updatedItemType);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void PatchItemType_Succes(){
            //Arrange
            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";  // Set the UserRole in HttpContext
            var item_type = new ItemTypeCS(){Id=1 , Name="old name", description="old description"};
            var patcheditemtype = new ItemTypeCS(){Id=1 , Name="new name", description="new description"};
            _mockItemTypeService.Setup(service => service.GetItemById(1)).Returns(item_type);
            _mockItemTypeService.Setup(service=>service.PatchItemType(1, "description", "new description")).Returns(patcheditemtype);
            //Act
            var result = _itemTypeController.PatchItemType(1, "description", "new description");
            var resultok = result.Result as OkObjectResult;
            var value = resultok.Value as ItemTypeCS;
            //Assert
            Assert.IsNotNull(item_type);
            Assert.IsNotNull(patcheditemtype);
            Assert.IsNotNull(result);
            Assert.IsNotNull(resultok);
            Assert.IsNotNull(value);
            Assert.AreEqual(resultok.StatusCode, 200);
        }
        [TestMethod]
        public void DeleteItemTypeTest_Exists()
        {
            // Arrange
            var itemType = new ItemTypeCS { Id = 1, Name = "Type1", description = "Description1" };
            _mockItemTypeService.Setup(service => service.GetItemById(1)).Returns(itemType);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";  // Set the UserRole in HttpContext

            // Assign HttpContext to the controller
            _itemTypeController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = _itemTypeController.DeleteItemType(1);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkResult));
        }

        [TestMethod]
        public void DeleteItemTypesTest_Succes()
        {
            //Arrange
            var locationsToDelete = new List<int>() { 1, 2, 3 };

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";  // Set the UserRole in HttpContext

            // Assign HttpContext to the controller
            _itemTypeController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            //Act
            var result = _itemTypeController.DeleteItemTypes(locationsToDelete);
            var resultok = result as OkObjectResult;

            //Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            Assert.AreEqual(resultok.StatusCode, 200);
        }
    }
}