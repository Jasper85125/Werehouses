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

        [TestInitialize]
        public void Setup()
        {
            _mockItemTypeService = new Mock<IItemtypeService>();
            _mockItemService = new Mock<IItemService>();
            _itemTypeController = new ItemTypeController(_mockItemTypeService.Object, _mockItemService.Object);
            _itemTypes = new List<ItemTypeCS>
            {
                new ItemTypeCS { Id = 1, Name = "Type1", description = "Description1" },
                new ItemTypeCS { Id = 2, Name = "Type2", description = "Description2" }
            };
        }

        [TestMethod]
        public void GetAllItemtypes_ShouldReturnAllItemTypes()
        {
            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";  // Set the UserRole in HttpContext

            // Assign HttpContext to the controller
            _itemTypeController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
            // Arrange
            _mockItemTypeService.Setup(service => service.GetAllItemtypes()).Returns(_itemTypes);

            // Act
            var result = _mockItemTypeService.Object.GetAllItemtypes();

            // Assert
            Assert.AreEqual(2, result.Count);

            httpContext.Items["UserRole"] = "HackerMan";
            _itemTypeController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            //act
            var value = _itemTypeController.GetAllItemtypes();

            //assert
            var unauthorizedResult = value.Result as UnauthorizedResult;
            Assert.IsNotNull(unauthorizedResult);
            Assert.AreEqual(401, unauthorizedResult.StatusCode);
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

            httpContext.Items["UserRole"] = "HackerMan";
            _itemTypeController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            //act
            var value = _itemTypeController.GetItemById(1);

            //assert
            var unauthorizedResult = value.Result as UnauthorizedResult;
            Assert.IsNotNull(unauthorizedResult);
            Assert.AreEqual(401, unauthorizedResult.StatusCode);
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

            httpContext.Items["UserRole"] = "HackerMan";
            _itemTypeController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            //act
            var value = _itemTypeController.CreateItemType(newItemType);

            //assert
            var unauthorizedResult = value.Result as UnauthorizedResult;
            Assert.IsNotNull(unauthorizedResult);
            Assert.AreEqual(401, unauthorizedResult.StatusCode);
        }

        [TestMethod]
        public void UpdateItemType_ValidItem_ShouldReturnUpdatedItemType()
        {
            // Arrange
            var existingItemType = new ItemTypeCS { Id = 1, Name = "Type1", description = "Description1" };
            var updatedItemType = new ItemTypeCS { Id = 1, Name = "UpdatedType", description = "UpdatedDescription" };
            
            // Synchronous return for mocked methods
            _mockItemTypeService.Setup(service => service.GetItemById(1)).Returns(existingItemType);
            _mockItemTypeService.Setup(service => service.UpdateItemType(1, updatedItemType)).Returns(updatedItemType);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin"; // Set the UserRole in HttpContext

            // Assign HttpContext to the controller
            _itemTypeController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act - Synchronous call for update
            var result = _mockItemTypeService.Object.UpdateItemType(1, updatedItemType);

            // Assert - Verify the returned item type is updated
            Assert.IsNotNull(result);
            Assert.AreEqual(updatedItemType.Name, result.Name);
            Assert.AreEqual(updatedItemType.description, result.description);

            // Simulate unauthorized context
            httpContext.Items["UserRole"] = "HackerMan";
            _itemTypeController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act - Unauthorized access
            var value = _itemTypeController.UpdateItemType(1, updatedItemType);

            // Assert - Check for UnauthorizedResult
            var unauthorizedResult = value.Result as UnauthorizedResult;
            Assert.IsNotNull(unauthorizedResult);
            Assert.AreEqual(401, unauthorizedResult.StatusCode);
        }

        [TestMethod]
        public void UpdateItemType_WrongId_ShouldReturnNotFound()
        {
            // Arrange
            var updatedItemType = new ItemTypeCS { Id = 1, Name = "UpdatedType", description = "UpdatedDescription" };
            _mockItemTypeService.Setup(service => service.GetItemById(1)).Returns((ItemTypeCS)null);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";

            // Assign HttpContext to the controller
            _itemTypeController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = _mockItemTypeService.Object.UpdateItemType(1, updatedItemType);

            // Assert
            Assert.IsNull(result);

            httpContext.Items["UserRole"] = "HackerMan";
            _itemTypeController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var value = _itemTypeController.UpdateItemType(1, updatedItemType);

            // Assert
            var unauthorizedResult = value.Result as UnauthorizedResult;
            Assert.IsNotNull(unauthorizedResult);
            Assert.AreEqual(401, unauthorizedResult.StatusCode);
        }

        [TestMethod]
        public void UpdateItemType_IdMismatch_ShouldReturnBadRequest()
        {
            // Arrange
            var updatedItemType = new ItemTypeCS { Id = 2, Name = "UpdatedType", description = "UpdatedDescription" };

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";

            // Assign HttpContext to the controller
            _itemTypeController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = _mockItemTypeService.Object.UpdateItemType(1, updatedItemType);

            // Assert
            Assert.IsNull(result);

            httpContext.Items["UserRole"] = "HackerMan";
            _itemTypeController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var value = _itemTypeController.UpdateItemType(1, updatedItemType);

            // Assert
            var unauthorizedResult = value.Result as UnauthorizedResult;
            Assert.IsNotNull(unauthorizedResult);
            Assert.AreEqual(401, unauthorizedResult.StatusCode);
        }


        [TestMethod]
        public void PatchItemType_Succes(){
            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";

            // Assign HttpContext to the controller
            _itemTypeController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
            //Arrange
            var patcheditemtype = new ItemTypeCS(){ Id=1, Name="HAHA"};
            _mockItemTypeService.Setup(service=>service.PatchItemType(1, "Name", "HAHA")).Returns(patcheditemtype);
            //Act
            var result = _itemTypeController.PatchItemType(1, "Name", "HAHA");
            var resultok = result.Result as OkObjectResult;
            var value = resultok.Value as ItemTypeCS;
            //Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(resultok);
            Assert.IsNotNull(value);
            Assert.AreEqual(resultok.StatusCode, 200);
            Assert.AreEqual(value.Name, "HAHA");

            httpContext.Items["UserRole"] = "HackerMan";
            _itemTypeController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var unauth_attempt = _itemTypeController.PatchItemType(1,"Name","HAHA");

            // Assert
            var unauthorizedResult = unauth_attempt.Result as UnauthorizedResult;
            Assert.IsNotNull(unauthorizedResult);
            Assert.AreEqual(401, unauthorizedResult.StatusCode);
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

            httpContext.Items["UserRole"] = "HackerMan";
            _itemTypeController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var unauth_attempt = _itemTypeController.DeleteItemType(1);

            // Assert
            var unauthorizedResult = unauth_attempt as UnauthorizedResult;
            Assert.IsNotNull(unauthorizedResult);
            Assert.AreEqual(401, unauthorizedResult.StatusCode);
        }

        [TestMethod]
        public void DeleteItemTypesTest_Succes()
        {
            //Arrange
            var itemTypesToDelete = new List<int>() { 1, 2, 3 };

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";  // Set the UserRole in HttpContext

            // Assign HttpContext to the controller
            _itemTypeController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            //Act
            var result = _itemTypeController.DeleteItemTypes(itemTypesToDelete);
            var resultok = result as OkObjectResult;

            //Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            Assert.AreEqual(resultok.StatusCode, 200);

            httpContext.Items["UserRole"] = "HackerMan";
            _itemTypeController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var unauth_attempt = _itemTypeController.DeleteItemTypes(itemTypesToDelete);

            // Assert
            var unauthorizedResult = unauth_attempt as UnauthorizedResult;
            Assert.IsNotNull(unauthorizedResult);
            Assert.AreEqual(401, unauthorizedResult.StatusCode);
        }
    }
}
