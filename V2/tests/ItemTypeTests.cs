using Microsoft.VisualStudio.TestTools.UnitTesting;
using ServicesV2;
using Moq;
using ControllersV2;
using System.Data.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

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

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "../../data/item_types.json");
            var itemType = new ItemTypeCS { Id = 1, Name = "Type 1", description = "Cool items", created_at = DateTime.Now, updated_at = DateTime.Now };

            var itemTypeList = new List<ItemTypeCS> { itemType };
            var json = JsonConvert.SerializeObject(itemTypeList, Formatting.Indented);

            var directory = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            File.WriteAllText(filePath, json);
        }

        [TestMethod]
        public void GetAllItemtypes_ShouldReturnAllItemTypes()
        {
            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";

            _itemTypeController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
            // Arrange
            _mockItemTypeService.Setup(service => service.GetAllItemtypes()).Returns(_itemTypes);

            // Act
            var result = _itemTypeController.GetAllItemtypes();
            var okResult = result.Result as OkObjectResult;

            // Assert
            var itemTypes = okResult.Value as IEnumerable<ItemTypeCS>;
            Assert.AreEqual(2, itemTypes.Count());

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
            httpContext.Items["UserRole"] = "Admin";

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
            httpContext.Items["UserRole"] = "Admin";

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

            _mockItemTypeService.Setup(service => service.GetItemById(1)).Returns(existingItemType);
            _mockItemTypeService.Setup(service => service.UpdateItemType(1, updatedItemType)).Returns(updatedItemType);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";

            _itemTypeController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = _mockItemTypeService.Object.UpdateItemType(1, updatedItemType);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(updatedItemType.Name, result.Name);
            Assert.AreEqual(updatedItemType.description, result.description);

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
        public void UpdateItemType_WrongId_ShouldReturnNotFound()
        {
            // Arrange
            var updatedItemType = new ItemTypeCS { Id = 1, Name = "UpdatedType", description = "UpdatedDescription" };
            _mockItemTypeService.Setup(service => service.GetItemById(1)).Returns((ItemTypeCS)null);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";

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
        public void PatchItemType_Succes()
        {
            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";

            _itemTypeController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            //Arrange
            var patcheditemtype = new ItemTypeCS() { Id = 1, Name = "HAHA" };
            _mockItemTypeService.Setup(service => service.PatchItemType(1, "Name", "HAHA")).Returns(patcheditemtype);

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
            var unauth_attempt = _itemTypeController.PatchItemType(1, "Name", "HAHA");

            // Assert
            var unauthorizedResult = unauth_attempt.Result as UnauthorizedResult;
            Assert.IsNotNull(unauthorizedResult);
            Assert.AreEqual(401, unauthorizedResult.StatusCode);
        }

        [TestMethod]
        public void PatchItemType_Failed()
        {
            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";

            _itemTypeController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            //Arrange
            _mockItemTypeService.Setup(service => service.PatchItemType(-1, "Name", "HAHA")).Returns((ItemTypeCS)null);

            //Act
            var result = _itemTypeController.PatchItemType(-1, "Name", "HAHA");
            var resultok = result.Result as BadRequestResult;

            //Assert
            Assert.IsNull(resultok);
        }

        [TestMethod]
        public void DeleteItemTypeTest_Exists()
        {
            // Arrange
            var itemType = new ItemTypeCS { Id = 1, Name = "Type1", description = "Description1" };
            _mockItemTypeService.Setup(service => service.GetItemById(1)).Returns(itemType);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";

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
        public void DeleteItemTypeTest_Failed()
        {
            // Arrange
            _mockItemTypeService.Setup(service => service.GetItemById(-1)).Returns((ItemTypeCS)null);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";

            _itemTypeController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = _itemTypeController.DeleteItemType(-1) as ActionResult;

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public void DeleteItemTypesTest_Succes()
        {
            //Arrange
            var itemTypesToDelete = new List<int>() { 1, 2, 3 };

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";

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

        [TestMethod]
        public void DeleteItemTypesTest_Failed()
        {

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";

            _itemTypeController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            //Act
            var result = _itemTypeController.DeleteItemTypes(null);
            var resultok = result as ActionResult;

            //Assert
            Assert.IsInstanceOfType(resultok, typeof(NotFoundResult));

        }

        [TestMethod]
        public void GetAllItemTypesService_Test()
        {
            var itemTypeService = new ItemTypeService();
            var itemTypes = itemTypeService.GetAllItemtypes();
            Assert.IsNotNull(itemTypes);
            Assert.AreEqual(1, itemTypes.Count);
        }

        [TestMethod]
        public void GetItemTypeByIdService_Test()
        {
            var itemTypeService = new ItemTypeService();
            var itemTypes = itemTypeService.GetItemById(1);
            Assert.IsNotNull(itemTypes);
            Assert.AreEqual("Type 1", itemTypes.Name);
        }

        [TestMethod]
        public void CreateItemTypeService_Test()
        {
            var itemTypeService = new ItemTypeService();
            var newItemType = new ItemTypeCS { Id = 2, Name = "Type 2", description = "Cool items", created_at = DateTime.Now, updated_at = DateTime.Now };
            var itemTypes = itemTypeService.CreateItemType(newItemType);
            Assert.IsNotNull(itemTypes);
            Assert.AreEqual("Type 2", itemTypes.Name);

            var itemTypesUpdated = itemTypeService.GetAllItemtypes();
            Assert.IsNotNull(itemTypesUpdated);
            Assert.AreEqual(2, itemTypesUpdated.Count);
        }

        [TestMethod]
        public void CreateItemTypeService_Test_Empty()
        {
            var itemTypeService = new ItemTypeService();

            itemTypeService.DeleteItemType(1);
            var itemTypesGet = itemTypeService.GetAllItemtypes();
            Assert.AreEqual(0, itemTypesGet.Count);

            var newItemType = new ItemTypeCS { Id = 2, Name = "Type 2", description = "Cool items", created_at = DateTime.Now, updated_at = DateTime.Now };
            var itemTypes = itemTypeService.CreateItemType(newItemType);
            Assert.IsNotNull(itemTypes);
            Assert.AreEqual("Type 2", itemTypes.Name);

            var itemTypesUpdated = itemTypeService.GetAllItemtypes();
            Assert.IsNotNull(itemTypesUpdated);
            Assert.AreEqual(1, itemTypesUpdated.Count);
        }

        [TestMethod]
        public void CreateMultipleItemTypes_Test()
        {
            var itemTypeService = new ItemTypeService();
            var newItemType = new List<ItemTypeCS> {new ItemTypeCS { Id = 2, Name = "Type 2", description = "Cool items", created_at = DateTime.Now, updated_at = DateTime.Now },
                new ItemTypeCS { Id = 3, Name = "Type 3", description = "Cool items", created_at = DateTime.Now, updated_at = DateTime.Now }};
            var itemTypes = itemTypeService.CreateMultipleItemTypes(newItemType);
            Assert.IsNotNull(itemTypes);

            var itemTypesUpdated = itemTypeService.GetAllItemtypes();
            Assert.IsNotNull(itemTypesUpdated);
            Assert.AreEqual(3, itemTypesUpdated.Count);
        }

        [TestMethod]
        public void UpdateItemTypeService_Test()
        {
            var itemTypeService = new ItemTypeService();
            var newItemType = new ItemTypeCS { Id = 2, Name = "Updated Type", description = "Updated Description", created_at = DateTime.Now, updated_at = DateTime.Now };

            var updatedItemType = new ItemTypeCS { Id = 1, Name = "Updated Type", description = "Updated Description" };
            var itemTypesUpdated2 = itemTypeService.UpdateItemType(1, updatedItemType);
            Assert.IsNotNull(itemTypesUpdated2);
            Assert.AreEqual("Updated Type", itemTypesUpdated2.Name);
        }

        [TestMethod]
        public void UpdateItemTypeService_Test_Failed()
        {
            var itemTypeService = new ItemTypeService();
            var newItemType = new ItemTypeCS { Id = 2, Name = "Updated Type", description = "Updated Description", created_at = DateTime.Now, updated_at = DateTime.Now };

            var updatedItemType = new ItemTypeCS { Id = 1, Name = "Updated Type", description = "Updated Description" };
            var itemTypesUpdated2 = itemTypeService.UpdateItemType(5, updatedItemType);
            Assert.IsNull(itemTypesUpdated2);
        }

        [TestMethod]
        public void DeleteItemTypeService_Test()
        {
            var itemTypeService = new ItemTypeService();
            itemTypeService.DeleteItemType(1);
            var itemTypesGet = itemTypeService.GetAllItemtypes();
            Assert.AreEqual(0, itemTypesGet.Count);
        }

        [TestMethod]
        public void DeleteItemTypeService_Test_Failed()
        {
            var itemTypeService = new ItemTypeService();
            itemTypeService.DeleteItemType(5);
            var itemTypesGet = itemTypeService.GetAllItemtypes();
            Assert.AreEqual(1, itemTypesGet.Count);
        }

        [TestMethod]
        public void DeleteMultipleItemTypeService_Test()
        {
            var itemTypeService = new ItemTypeService();

            var newItemType = new ItemTypeCS { Id = 2, Name = "Type 2", description = "Cool items", created_at = DateTime.Now, updated_at = DateTime.Now };
            var itemTypes = itemTypeService.CreateItemType(newItemType);
            Assert.IsNotNull(itemTypes);
            Assert.AreEqual("Type 2", itemTypes.Name);

            var itemTypesUpdated = itemTypeService.GetAllItemtypes();
            Assert.IsNotNull(itemTypesUpdated);
            Assert.AreEqual(2, itemTypesUpdated.Count);

            List<int> itemTypesToDelete = new List<int>() { 1, 2 };
            itemTypeService.DeleteItemTypes(itemTypesToDelete);
            var itemTypesGet = itemTypeService.GetAllItemtypes();
            Assert.AreEqual(0, itemTypesGet.Count);
        }

        [TestMethod]
        public void PatchItemTypeService_Test()
        {
            var itemTypeService = new ItemTypeService();
            var patcheditemtype = itemTypeService.PatchItemType(1, "Name", "New Type");
            patcheditemtype = itemTypeService.PatchItemType(1, "description", "New Description");
            var PatchedItemGoneWrong = itemTypeService.PatchItemType(3, "Name", "New Description");
            Assert.IsNotNull(patcheditemtype);
            Assert.IsNull(PatchedItemGoneWrong);
            Assert.AreEqual("New Type", patcheditemtype.Name);
            Assert.AreEqual("New Description", patcheditemtype.description);
        }

        [TestMethod]
        public void GetAllItemtypes_ShouldReturnUnauthorized_WhenUserRoleIsInvalid()
        {
            // Arrange
            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "HackerMan";
            _itemTypeController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = _itemTypeController.GetAllItemtypes().Result as UnauthorizedResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(401, result.StatusCode);
        }

        [TestMethod]
        public void CreateMultipleItemTypes_ShouldReturnCreatedItemTypes()
        {
            // Arrange
            var newItemTypes = new List<ItemTypeCS>
            {
                new ItemTypeCS { Id = 1, Name = "Type1", description = "Description1" },
                new ItemTypeCS { Id = 2, Name = "Type2", description = "Description2" }
            };

            _mockItemTypeService.Setup(service => service.CreateMultipleItemTypes(newItemTypes)).Returns(newItemTypes);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";

            _itemTypeController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = _itemTypeController.CreateMultipleItemTypes(newItemTypes).Result;

            // Assert
            var createdResult = result as ObjectResult;
            Assert.IsNotNull(createdResult);
            Assert.AreEqual(StatusCodes.Status201Created, createdResult.StatusCode);
            var returnedItemTypes = createdResult.Value as IEnumerable<ItemTypeCS>;
            Assert.IsNotNull(returnedItemTypes);
            Assert.AreEqual(2, returnedItemTypes.Count());
        }

        [TestMethod]
        public void CreateMultipleItemTypes_ShouldReturnUnauthorized_WhenUserRoleIsInvalid()
        {
            // Arrange
            var newItemTypes = new List<ItemTypeCS>
            {
                new ItemTypeCS { Id = 1, Name = "Type1", description = "Description1" },
                new ItemTypeCS { Id = 2, Name = "Type2", description = "Description2" }
            };

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "HackerMan";

            _itemTypeController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = _itemTypeController.CreateMultipleItemTypes(newItemTypes);

            // Assert
            var unauthorizedResult = result.Result as UnauthorizedResult;
            Assert.IsNotNull(unauthorizedResult);
            Assert.AreEqual(StatusCodes.Status401Unauthorized, unauthorizedResult.StatusCode);
        }

        [TestMethod]
        public void CreateMultipleItemTypes_ShouldReturnBadRequest_WhenItemTypesIsNull()
        {
            // Arrange
            List<ItemTypeCS> newItemTypes = null;

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";

            _itemTypeController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = _itemTypeController.CreateMultipleItemTypes(newItemTypes);

            // Assert
            var badRequestResult = result.Result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);
            Assert.AreEqual("ItemType data is null", badRequestResult.Value);
        }

        [TestMethod]
        public void UpdateItemType_IdMismatch_ShouldReturnBadRequest_2()
        {
            // Arrange
            var updatedItemType = new ItemTypeCS { Id = 2, Name = "UpdatedType", description = "UpdatedDescription" };

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";

            _itemTypeController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = _itemTypeController.UpdateItemType(1, updatedItemType);

            // Assert
            var badRequestResult = result.Result as BadRequestResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);
        }

        [TestMethod]
        public void UpdateItemType_ItemNotFound_ShouldReturnNotFound()
        {
            // Arrange
            var updatedItemType = new ItemTypeCS { Id = 1, Name = "UpdatedType", description = "UpdatedDescription" };
            _mockItemTypeService.Setup(service => service.GetItemById(1)).Returns((ItemTypeCS)null);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";

            _itemTypeController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = _itemTypeController.UpdateItemType(1, updatedItemType);

            // Assert
            var notFoundResult = result.Result as NotFoundResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(StatusCodes.Status404NotFound, notFoundResult.StatusCode);
        }

        [TestMethod]
        public void UpdateItemType_ValidItem_ShouldReturnUpdatedItemType_2()
        {
            // Arrange
            var existingItemType = new ItemTypeCS { Id = 1, Name = "Type1", description = "Description1" };
            var updatedItemType = new ItemTypeCS { Id = 1, Name = "UpdatedType", description = "UpdatedDescription" };

            _mockItemTypeService.Setup(service => service.GetItemById(1)).Returns(existingItemType);
            _mockItemTypeService.Setup(service => service.UpdateItemType(1, updatedItemType)).Returns(updatedItemType);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";

            _itemTypeController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = _itemTypeController.UpdateItemType(1, updatedItemType);

            // Assert
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(StatusCodes.Status200OK, okResult.StatusCode);
            var returnedItemType = okResult.Value as ItemTypeCS;
            Assert.IsNotNull(returnedItemType);
            Assert.AreEqual(updatedItemType.Name, returnedItemType.Name);
            Assert.AreEqual(updatedItemType.description, returnedItemType.description);
        }

        [TestMethod]
        public void GetItemById_ShouldReturnCorrectItemType_2()
        {
            // Arrange
            var itemType = _itemTypes[0];
            _mockItemTypeService.Setup(service => service.GetItemById(1)).Returns(itemType);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";

            _itemTypeController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = _itemTypeController.GetItemById(1);

            // Assert
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(StatusCodes.Status200OK, okResult.StatusCode);
            var returnedItemType = okResult.Value as ItemTypeCS;
            Assert.IsNotNull(returnedItemType);
            Assert.AreEqual(itemType.Name, returnedItemType.Name);
            Assert.AreEqual(itemType.description, returnedItemType.description);
        }

        [TestMethod]
        public void GetItemById_ShouldReturnNotFound_WhenItemTypeIsNull()
        {
            // Arrange
            _mockItemTypeService.Setup(service => service.GetItemById(1)).Returns((ItemTypeCS)null);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";

            _itemTypeController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = _itemTypeController.GetItemById(1);

            // Assert
            var notFoundResult = result.Result as NotFoundResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(StatusCodes.Status404NotFound, notFoundResult.StatusCode);
        }

        [TestMethod]
        public void CreateItemType_ShouldReturnBadRequest_WhenItemTypeIsNull()
        {
            // Arrange
            ItemTypeCS newItemType = null;

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";

            _itemTypeController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = _itemTypeController.CreateItemType(newItemType).Result;

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);
            Assert.AreEqual("ItemGroup cannot be null", badRequestResult.Value);
        }

        [TestMethod]
        public void CreateItemType_ShouldReturnCreatedItemType_2()
        {
            // Arrange
            var newItemType = new ItemTypeCS { Id = 3, Name = "Type3", description = "Description3" };
            _mockItemTypeService.Setup(service => service.CreateItemType(newItemType)).Returns(newItemType);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";

            _itemTypeController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = _itemTypeController.CreateItemType(newItemType).Result;

            // Assert
            var createdResult = result as CreatedAtActionResult;
            Assert.IsNotNull(createdResult);
            Assert.AreEqual(StatusCodes.Status201Created, createdResult.StatusCode);
            var returnedItemType = createdResult.Value as ItemTypeCS;
            Assert.IsNotNull(returnedItemType);
            Assert.AreEqual(newItemType.Id, returnedItemType.Id);
            Assert.AreEqual(newItemType.Name, returnedItemType.Name);
            Assert.AreEqual(newItemType.description, returnedItemType.description);
        }

        [TestMethod]
        public void GetAllItemtypes_ShouldReturnAllItemTypes_2()
        {
            // Arrange
            _mockItemTypeService.Setup(service => service.GetAllItemtypes()).Returns(_itemTypes);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";
            _itemTypeController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = _itemTypeController.GetAllItemtypes().Result as OkObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(StatusCodes.Status200OK, result.StatusCode);
            var itemTypes = result.Value as IEnumerable<ItemTypeCS>;
            Assert.IsNotNull(itemTypes);
            Assert.AreEqual(2, itemTypes.Count());
        }

        [TestMethod]
        public void GetAllItemtypes_ShouldReturnUnauthorized_WhenUserRoleIsInvalid_2()
        {
            // Arrange
            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "HackerMan";
            _itemTypeController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = _itemTypeController.GetAllItemtypes().Result as UnauthorizedResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(StatusCodes.Status401Unauthorized, result.StatusCode);
        }

        [TestMethod]
        public void GetItemById_ShouldReturnCorrectItemType_3()
        {
            // Arrange
            var itemType = _itemTypes[0];
            _mockItemTypeService.Setup(service => service.GetItemById(1)).Returns(itemType);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";

            _itemTypeController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = _itemTypeController.GetItemById(1);

            // Assert
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(StatusCodes.Status200OK, okResult.StatusCode);
            var returnedItemType = okResult.Value as ItemTypeCS;
            Assert.IsNotNull(returnedItemType);
            Assert.AreEqual(itemType.Name, returnedItemType.Name);
            Assert.AreEqual(itemType.description, returnedItemType.description);
        }

        [TestMethod]
        public void GetItemById_ShouldReturnNotFound_WhenItemTypeIsNull_2()
        {
            // Arrange
            _mockItemTypeService.Setup(service => service.GetItemById(1)).Returns((ItemTypeCS)null);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";

            _itemTypeController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = _itemTypeController.GetItemById(1);

            // Assert
            var notFoundResult = result.Result as NotFoundResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(StatusCodes.Status404NotFound, notFoundResult.StatusCode);
        }

    }
}