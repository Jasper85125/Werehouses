using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Services;
using Controllers;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

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
            var newItemGroup = new ItemGroupCS { Id = 3, Name = "Group 3", Description = "Cool items" };
            _mockItemGroupService.Setup(service => service.CreateItemGroup(It.IsAny<ItemGroupCS>())).Returns(newItemGroup);

            // Act
            var result = _itemGroupController.CreateItemGroup(newItemGroup);
            var createdResult = result as CreatedAtActionResult;
            var returnedItem = createdResult.Value as ItemGroupCS;

            // Assert
            Assert.IsNotNull(createdResult);
            Assert.AreEqual(newItemGroup.Name, returnedItem.Name);
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
        }

        [TestMethod]
        public void UpdateItemGroupTest_ValidItem()
        {
            // Arrange
            var existingItemGroup = new ItemGroupCS { Id = 1, Description = "Existing Item" };
            var updatedItemGroup = new ItemGroupCS { Id = 1, Description = "Updated Item" };
            _mockItemGroupService.Setup(service => service.GetItemById(1)).Returns(existingItemGroup);
            _mockItemGroupService.Setup(service => service.UpdateItemGroup(1, updatedItemGroup)).Returns(updatedItemGroup);

            // Act
            var value = _itemGroupController.UpdateItemGroup(1, updatedItemGroup);
            var okResult = value.Result as OkObjectResult;
            var returnedItem = okResult.Value as ItemGroupCS;

            // Assert
            Assert.IsNotNull(okResult);
            Assert.AreEqual(updatedItemGroup.Description, returnedItem.Description);
        }

        [TestMethod]
        public void UpdateItemGroupTest_WrongId()
        {
            // Arrange
            var updatedItemGroup = new ItemGroupCS { Id = 1, Description = "Updated Item" };
            _mockItemGroupService.Setup(service => service.GetItemById(1)).Returns((ItemGroupCS)null);

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
            // Act
            var result = _itemGroupController.DeleteItemGroup(1);
            //assert
            Assert.IsInstanceOfType(result, typeof(OkResult));
            
        }
        [TestMethod]
        public void ItemsFromItemGroupId_Succes(){
            //Arrange
            var testResult = new ItemCS(){ uid= "P000084", code= "xQk78654R",
            description= "Open-architected tertiary contingency",
            short_description= "throughout", upc_code= "6240362357099",
            model_number= "81-buCQA7M", commodity_code= "hV-9935",
            item_line= 67, item_group= 1, item_type= 17,unit_purchase_quantity= 18,
            unit_order_quantity= 17, pack_order_quantity= 13, supplier_id= 27,
            supplier_code= "SUP545", supplier_part_number= "f-768-s2A",
            // created_at= "1995-09-07T07:15:07", updated_at= "1996-09-16T17:31:21"
            };
            _mockItemGroupService.Setup(service => service.ItemsFromItemGroupId(1)).Returns(new List<ItemCS>(){testResult});
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
        }
        [TestMethod]
        public void DeleteItemGroups_Succes(){
            //Arrange
            var idstodel = new List<int>(){1, 2, 3};
            //Act
            var result = _itemGroupController.DeleteItemGroups(idstodel);
            var resultok = result as OkObjectResult;
            //Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            Assert.AreEqual(resultok.StatusCode, 200);
        }

        [TestMethod]
        public void PatchItemGroupTest_Success()
        {
            // Arrange
            var existingItemGroup = new ItemGroupCS { Id = 1, Name = "Group 1", Description = "Existing Description" };
            var patchItemGroup = new ItemGroupCS { Name = "Updated Group", Description = "Updated Description" };

            _mockItemGroupService.Setup(service => service.GetItemById(1)).Returns(existingItemGroup);
            _mockItemGroupService.Setup(service => service.PatchItemGroup(1, patchItemGroup)).Returns(patchItemGroup);

            // Act
            var result = _itemGroupController.PatchItemGroup(1, patchItemGroup);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.IsInstanceOfType(okResult.Value, typeof(ItemGroupCS));
            var returnedItemGroup = okResult.Value as ItemGroupCS;
            Assert.AreEqual(patchItemGroup.Name, returnedItemGroup.Name);
            Assert.AreEqual(patchItemGroup.Description, returnedItemGroup.Description);
        }

        [TestMethod]
        public void PatchItemGroupTest_NotFound()
        {
            // Arrange
            var patchItemGroup = new ItemGroupCS { Name = "Updated Group", Description = "Updated Description" };

            _mockItemGroupService.Setup(service => service.GetItemById(1)).Returns((ItemGroupCS)null);

            // Act
            var result = _itemGroupController.PatchItemGroup(1, patchItemGroup);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));
        }
    }
}