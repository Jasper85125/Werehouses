using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ServicesV2;
using ControllersV2;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace itemgroup.TestsV2
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

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "../../data/item_groups.json");
            var itemGroup = new ItemGroupCS { Id = 1, Name = "Group 1", Description = "Cool items", created_at = DateTime.Now, updated_at = DateTime.Now };

            var itemGroupList = new List<ItemGroupCS> { itemGroup };
            var json = JsonConvert.SerializeObject(itemGroupList, Formatting.Indented);

            // Create directory if it does not exist
            var directory = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            // Write the JSON data to the file
            File.WriteAllText(filePath, json);
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

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin"; 

            _itemGroupController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = _itemGroupController.GetAllItemGroups();
            var okResult = result.Result as OkObjectResult;
            var returnedItems = okResult.Value as IEnumerable<ItemGroupCS>;

            // Assert
            Assert.IsNotNull(okResult);
            Assert.AreEqual(2, returnedItems.Count());

            httpContext.Items["UserRole"] = "NoRole";
            _itemGroupController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            //act
            result = _itemGroupController.GetAllItemGroups();

            //assert
            var unauthorizedResult = result.Result as UnauthorizedResult;
            Assert.IsNotNull(unauthorizedResult);
            Assert.AreEqual(401, unauthorizedResult.StatusCode);
        }

        [TestMethod]
        public void GetItemGroupByIdTest_Exists()
        {
            // Arrange
            var itemGroup = new ItemGroupCS { Id = 1, Name = "Group 1" };
            _mockItemGroupService.Setup(service => service.GetItemById(1)).Returns(itemGroup);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";

            _itemGroupController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = _itemGroupController.GetItemById(1);
            var okResult = result.Result as OkObjectResult;
            var returnedItem = okResult.Value as ItemGroupCS;

            // Assert
            Assert.IsNotNull(okResult);
            Assert.IsNotNull(okResult.Value);
            Assert.AreEqual(itemGroup.Name, returnedItem.Name);

            httpContext.Items["UserRole"] = "NoRole";
            _itemGroupController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            //act
            result = _itemGroupController.GetItemById(1);

            //assert
            var unauthorizedResult = result.Result as UnauthorizedResult;
            Assert.IsNotNull(unauthorizedResult);
            Assert.AreEqual(401, unauthorizedResult.StatusCode);
        }

        [TestMethod]
        public void GetItemGroupByIdTest_WrongId()
        {
            // Arrange
            _mockItemGroupService.Setup(service => service.GetItemById(1)).Returns((ItemGroupCS)null);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";

            _itemGroupController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = _itemGroupController.GetItemById(1);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));
        }

        [TestMethod]
        public void CreateItemGroupTest_Success()
        {
            // Arrange
            var newItemGroup = new ItemGroupCS { Id = 3, Name = "Group 3", Description = "Cool items" };
            _mockItemGroupService.Setup(service => service.CreateItemGroup(It.IsAny<ItemGroupCS>())).Returns(newItemGroup);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";

            _itemGroupController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = _itemGroupController.CreateItemGroup(newItemGroup);
            var createdResult = result as CreatedAtActionResult;
            var returnedItem = createdResult.Value as ItemGroupCS;

            // Assert
            Assert.IsNotNull(createdResult);
            Assert.AreEqual(newItemGroup.Name, returnedItem.Name);

            httpContext.Items["UserRole"] = "NoRole";
            _itemGroupController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            //act
            var value = _itemGroupController.GetItemById(1);

            //assert
            var unauthorizedResult = value.Result as UnauthorizedResult;
            Assert.IsNotNull(unauthorizedResult);
            Assert.AreEqual(401, unauthorizedResult.StatusCode);
        }

        [TestMethod]
        public void CreateMultipleItemGroups_ReturnsCreatedResult_WithNewGroups()
        {
            // Arrange
            var itemGroups = new List<ItemGroupCS>
            {
                new ItemGroupCS { Id = 3, Name = "Group 3", Description = "Cool items" },
                new ItemGroupCS { Id = 4, Name = "Group 4", Description = "Cool items" }
            };
            _mockItemGroupService.Setup(service => service.CreateMultipleItemGroups(itemGroups)).Returns(itemGroups);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin"; 

            _itemGroupController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = _itemGroupController.CreateMultipleItemGroups(itemGroups);
            var createdResult = result.Result as ObjectResult;
            var returnedItems = createdResult.Value as List<ItemGroupCS>;
            var firstItemGroup = returnedItems[0];

            // Assert
            Assert.IsNotNull(createdResult);
            Assert.IsNotNull(returnedItems);
            Assert.AreEqual(itemGroups[0].Name, firstItemGroup.Name);
            Assert.AreEqual(itemGroups[0].Description, firstItemGroup.Description);

            httpContext.Items["UserRole"] = "NoRole";
            _itemGroupController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            //act
            result = _itemGroupController.CreateMultipleItemGroups(itemGroups);

            //assert
            var unauthorizedResult = result.Result as UnauthorizedResult;
            Assert.IsNotNull(unauthorizedResult);
            Assert.AreEqual(401, unauthorizedResult.StatusCode);
        }

        [TestMethod]
        public void UpdateItemGroupTest_ValidItem()
        {
            // Arrange
            var existingItemGroup = new ItemGroupCS { Id = 1, Description = "Existing Item" };
            var updatedItemGroup = new ItemGroupCS { Id = 1, Description = "Updated Item" };
            _mockItemGroupService.Setup(service => service.GetItemById(1)).Returns(existingItemGroup);
            _mockItemGroupService.Setup(service => service.UpdateItemGroup(1, updatedItemGroup)).Returns(updatedItemGroup);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";

            _itemGroupController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var value = _itemGroupController.UpdateItemGroup(1, updatedItemGroup);
            var okResult = value.Result as OkObjectResult;
            var returnedItem = okResult.Value as ItemGroupCS;

            // Assert
            Assert.IsNotNull(okResult);
            Assert.AreEqual(updatedItemGroup.Description, returnedItem.Description);

            httpContext.Items["UserRole"] = "NoRole";
            _itemGroupController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            //act
            var result = _itemGroupController.UpdateItemGroup(1, updatedItemGroup);

            //assert
            var unauthorizedResult = result.Result as UnauthorizedResult;
            Assert.IsNotNull(unauthorizedResult);
            Assert.AreEqual(401, unauthorizedResult.StatusCode);
        }

        [TestMethod]
        public void UpdateItemGroupTest_WrongId()
        {
            // Arrange
            var updatedItemGroup = new ItemGroupCS { Id = 1, Description = "Updated Item" };
            _mockItemGroupService.Setup(service => service.GetItemById(1)).Returns((ItemGroupCS)null);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";

            _itemGroupController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var value = _itemGroupController.UpdateItemGroup(1, updatedItemGroup);

            // Assert
            Assert.IsInstanceOfType(value.Result, typeof(NotFoundResult));
        }

        [TestMethod]
        public void UpdateItemGroupTest_IdMismatch()
        {
            // Arrange
            var updatedItemGroup = new ItemGroupCS { Id = 2, Description = "Updated Item" };

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";

            _itemGroupController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var value = _itemGroupController.UpdateItemGroup(1, updatedItemGroup);

            // Assert
            Assert.IsInstanceOfType(value.Result, typeof(BadRequestResult));
        }

        [TestMethod]
        public void DeleteItemGroupTest_Exists()
        {
            // Arrange
            var itemGroup = new ItemGroupCS { Id = 1, Name = "Group 1" };
            _mockItemGroupService.Setup(service => service.GetItemById(1)).Returns(itemGroup);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";

            _itemGroupController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = _itemGroupController.DeleteItemGroup(1);

            //assert
            Assert.IsInstanceOfType(result, typeof(OkResult));

            httpContext.Items["UserRole"] = "NoRole";
            _itemGroupController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            //act
            result = _itemGroupController.DeleteItemGroup(1);

            //assert
            var unauthorizedResult = result as UnauthorizedResult;
            Assert.IsNotNull(unauthorizedResult);
            Assert.AreEqual(401, unauthorizedResult.StatusCode);

        }

        [TestMethod]
        public void ItemsFromItemGroupId_Succes()
        {
            //Arrange
            var testResult = new ItemCS()
            {
                uid = "P000084",
                code = "xQk78654R",
                description = "Open-architected tertiary contingency",
                short_description = "throughout",
                upc_code = "6240362357099",
                model_number = "81-buCQA7M",
                commodity_code = "hV-9935",
                item_line = 67,
                item_group = 1,
                item_type = 17,
                unit_purchase_quantity = 18,
                unit_order_quantity = 17,
                pack_order_quantity = 13,
                supplier_id = 27,
                supplier_code = "SUP545",
                supplier_part_number = "f-768-s2A",
            };
            _mockItemGroupService.Setup(service => service.ItemsFromItemGroupId(1)).Returns(new List<ItemCS>() { testResult });

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";

            _itemGroupController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            //Act
            var result = _itemGroupController.GetAllItemsFromItemGroupId(1);
            var resultOK = result.Result as OkObjectResult;
            var value = resultOK.Value as List<ItemCS>;

            //Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(resultOK);
            Assert.IsNotNull(value);
            Assert.IsInstanceOfType(value, typeof(List<ItemCS>));
            Assert.AreEqual(value[0].item_group, 1);

            httpContext.Items["UserRole"] = "NoRole";
            _itemGroupController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            //act
            var unAuth = _itemGroupController.GetAllItemsFromItemGroupId(1);

            //assert
            var unauthorizedResult = unAuth.Result as UnauthorizedResult;
            Assert.IsNotNull(unauthorizedResult);
            Assert.AreEqual(401, unauthorizedResult.StatusCode);
        }

        [TestMethod]
        public void DeleteItemGroups_Succes()
        {
            //Arrange
            var itemgroupsToDelete = new List<int>() { 1, 2, 3 };

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";

            _itemGroupController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            //Act
            var result = _itemGroupController.DeleteItemGroups(itemgroupsToDelete);
            var resultok = result as OkObjectResult;

            //Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            Assert.AreEqual(resultok.StatusCode, 200);

            httpContext.Items["UserRole"] = "NoRole";
            _itemGroupController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            //act
            var value = _itemGroupController.DeleteItemGroups(itemgroupsToDelete);

            //assert
            var unauthorizedResult = value as UnauthorizedResult;
            Assert.IsNotNull(unauthorizedResult);
            Assert.AreEqual(401, unauthorizedResult.StatusCode);
        }

        [TestMethod]
        public void PatchItemGroupTest_Success()
        {
            // Arrange
            var existingItemGroup = new ItemGroupCS { Id = 1, Name = "Group 1"  };
            var patchItemGroup = new ItemGroupCS { Name = "Updated Group" };

            _mockItemGroupService.Setup(service => service.GetItemById(1)).Returns(existingItemGroup);
            _mockItemGroupService.Setup(service => service.PatchItemGroup(1, "Name", "Updated Group")).Returns(patchItemGroup);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";

            _itemGroupController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = _itemGroupController.PatchItemGroup(1, "Name", "Updated Group");
            var okResult = result.Result as OkObjectResult;
            var returnedItemGroup = okResult.Value as ItemGroupCS;

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            Assert.IsNotNull(okResult);
            Assert.IsInstanceOfType(okResult.Value, typeof(ItemGroupCS));
            Assert.AreEqual(patchItemGroup.Name, returnedItemGroup.Name);

            httpContext.Items["UserRole"] = "NoRole";
            _itemGroupController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            //act
            var value = _itemGroupController.PatchItemGroup(1, "Name", "Updated Group");

            //assert
            var unauthorizedResult = value.Result as UnauthorizedResult;
            Assert.IsNotNull(unauthorizedResult);
            Assert.AreEqual(401, unauthorizedResult.StatusCode);
        }

        [TestMethod]
        public void PatchItemGroupTest_NotFound()
        {
            // Arrange
            var patchItemGroup = new ItemGroupCS { Name = "Updated Group", Description = "Updated Description" };

            _mockItemGroupService.Setup(service => service.GetItemById(1)).Returns((ItemGroupCS)null);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";

            _itemGroupController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = _itemGroupController.PatchItemGroup(1, "Name", "Updated Group");

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));
        }

        [TestMethod]
        public void GetAllItemGroupsService_Test()
        {
            var itemGroupService = new ItemGroupService();
            var itemGroups = itemGroupService.GetAllItemGroups();
            Assert.IsNotNull(itemGroups);
            Assert.AreEqual(1, itemGroups.Count);
        }

        [TestMethod]
        public void GetItemGroupByIdService_Test()
        {
            var itemGroupService = new ItemGroupService();
            var itemGroups = itemGroupService.GetItemById(1);
            Assert.IsNotNull(itemGroups);
            Assert.AreEqual("Group 1", itemGroups.Name);
        }

        [TestMethod]
        public void CreateItemGroupService_Test()
        {
            var itemGroup = new ItemGroupCS { Id = 2, Name = "Group 2", Description = "Cool items 2", created_at = DateTime.Now, updated_at = DateTime.Now };
            var itemGroupService = new ItemGroupService();
            var itemGroups = itemGroupService.CreateItemGroup(itemGroup);
            Assert.IsNotNull(itemGroups);
            Assert.AreEqual("Group 2", itemGroups.Name);

            var itemGroupsUpdated = itemGroupService.GetAllItemGroups();
            Assert.AreEqual(2, itemGroupsUpdated.Count);
        }

        [TestMethod]
        public void CreateItemGroupService_Test_Empty()
        {
            var itemGroupService = new ItemGroupService();
            itemGroupService.DeleteItemGroup(1);
            var itemGroupsUpdated = itemGroupService.GetAllItemGroups();
            Assert.AreEqual(0, itemGroupsUpdated.Count);
            
            var itemGroup = new ItemGroupCS { Id = 1, Name = "Group 2", Description = "Cool items 2", created_at = DateTime.Now, updated_at = DateTime.Now };
            var itemGroupService = new ItemGroupService();
            var itemGroups = itemGroupService.CreateItemGroup(itemGroup);
            Assert.IsNotNull(itemGroups);
            Assert.AreEqual("Group 2", itemGroups.Name);

            var itemGroupsUpdated = itemGroupService.GetAllItemGroups();
            Assert.AreEqual(2, itemGroupsUpdated.Count);
        }

        [TestMethod]
        public void CreateMultipleItemGroupService_Test()
        {
            var itemGroup = new List<ItemGroupCS> {
                new ItemGroupCS { Id = 2, Name = "Group 2", Description = "Cool items 2", created_at = DateTime.Now, updated_at = DateTime.Now },
                new ItemGroupCS { Id = 3, Name = "Group 3", Description = "Cool items 3", created_at = DateTime.Now, updated_at = DateTime.Now }
            };
            var itemGroupService = new ItemGroupService();
            var itemGroups = itemGroupService.CreateMultipleItemGroups(itemGroup);
            Assert.IsNotNull(itemGroups);

            var itemGroupsUpdated = itemGroupService.GetAllItemGroups();
            Assert.AreEqual(3, itemGroupsUpdated.Count);
        }

        [TestMethod]
        public void UpdateItemGroupService_Test()
        {
            var itemGroup = new ItemGroupCS { Id = 1, Name = "Group 2", Description = "Cool items 2", created_at = DateTime.Now, updated_at = DateTime.Now }; 
            var itemGroupService = new ItemGroupService();
            var itemGroups = itemGroupService.UpdateItemGroup(1, itemGroup);
            Assert.IsNotNull(itemGroups);
            Assert.AreEqual("Group 2", itemGroups.Name);
        }

        [TestMethod]
        public void UpdateItemGroupService_Test_Failed()
        {
            var itemGroup = new ItemGroupCS { Id = 2, Name = "Group 2", Description = "Cool items 2", created_at = DateTime.Now, updated_at = DateTime.Now }; 
            var itemGroupService = new ItemGroupService();
            var itemGroups = itemGroupService.UpdateItemGroup(3, itemGroup);
            Assert.IsNull(itemGroups);
        }

        [TestMethod]
        public void DeleteItemGroupService_Test()
        {
            var itemGroupService = new ItemGroupService();
            itemGroupService.DeleteItemGroup(1);
            var itemGroupsUpdated = itemGroupService.GetAllItemGroups();
            Assert.AreEqual(0, itemGroupsUpdated.Count);
        }

        [TestMethod]
        public void DeleteItemGroupService_Test_Failed()
        {
            var itemGroupService = new ItemGroupService();
            itemGroupService.DeleteItemGroup(4);
            var itemGroupsUpdated = itemGroupService.GetAllItemGroups();
            Assert.AreEqual(1, itemGroupsUpdated.Count);
        }

        [TestMethod]
        public void DeleteMultipleItemGroupService_Test()
        {
            var itemGroup = new ItemGroupCS { Id = 2, Name = "Group 2", Description = "Cool items 2", created_at = DateTime.Now, updated_at = DateTime.Now };
            var itemGroupService = new ItemGroupService();
            var itemGroups = itemGroupService.CreateItemGroup(itemGroup);
            Assert.IsNotNull(itemGroups);
            Assert.AreEqual("Group 2", itemGroups.Name);

            var itemGroupsUpdated = itemGroupService.GetAllItemGroups();
            Assert.AreEqual(2, itemGroupsUpdated.Count);
            List<int> itemGroupsToDelete = new List<int> { 1, 2 };
            itemGroupService.DeleteItemGroups(itemGroupsToDelete);
            var itemGroupsUpdatedAgain = itemGroupService.GetAllItemGroups();
            Assert.AreEqual(0, itemGroupsUpdatedAgain.Count);
        }

        [TestMethod]
        public void PatchItemGroupService_Test()
        {
            var itemGroupService = new ItemGroupService();
            var itemGroups = itemGroupService.PatchItemGroup(1, "Name", "Updated Group");
            itemGroups = itemGroupService.PatchItemGroup(1, "Description", "Updated Description");
            var itemGroupGoneWrong = itemGroupService.PatchItemGroup(2, "Name", "Updated Group");
            var itemGroupGoneWrongAgain = itemGroupService.PatchItemGroup(1, "Items", "Updated Items");
            Assert.IsNotNull(itemGroups);
            Assert.AreEqual("Updated Group", itemGroups.Name);
            Assert.AreEqual("Updated Description", itemGroups.Description);
            Assert.IsNull(itemGroupGoneWrong);
            Assert.IsNull(itemGroupGoneWrongAgain);
        }
    }
}