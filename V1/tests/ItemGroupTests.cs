using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ServicesV1;
using ControllersV1;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Security.Cryptography.X509Certificates;

namespace TestsV1
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
            var itemgroup = new ItemGroupCS(){
                    Id= 0,
                    Name="Electronics",
                    Description="",
                    created_at=DateTime.Now,
                    updated_at=DateTime.Now
            };
            var itemgrouplist = new List<ItemGroupCS>(){ itemgroup };  
            var json = JsonConvert.SerializeObject(itemgrouplist, Formatting.Indented);
            var directory = Path.GetDirectoryName(filePath);
            if(!Directory.Exists(directory)){
                Directory.CreateDirectory(directory);
            }
            File.WriteAllText(filePath, json);
        }
        [TestMethod]
        public void GetAllItemGroups_Test_Succes(){
            var itemgroupsservice = new ItemGroupService();
            var result = itemgroupsservice.GetAllItemGroups();
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
        }
        [TestMethod]
        public void GetItemGroupByIdTest_Succes(){
            var itemgroupsservice = new ItemGroupService();
            var result = itemgroupsservice.GetItemById(0);
            Assert.IsNotNull(result);
            Assert.AreEqual("Electronics", result.Name);
        }
        [TestMethod]
        public void ItemsFromItemGroupId_Test_Succes(){
            var itemgroupsservice = new ItemGroupService();
            var result = itemgroupsservice.ItemsFromItemGroupId(1);
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }
        [TestMethod]
        public void CreateItemGroup_Test_Succes(){
            var itemgroup = new ItemGroupCS(){
                Id= 1,
                Name= "Furniture",
                Description= "",
                created_at= DateTime.Now,
                updated_at= DateTime.Now
                };
            var itemgroupsservice = new ItemGroupService();
            var result = itemgroupsservice.CreateItemGroup(itemgroup);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Result.Id);
        }
        [TestMethod]
        public void UpdateItemGroup_Test_Succes(){
            var updateditemgroup = new ItemGroupCS(){
                Id=1,
                Name="Stationery",
                Description = "",
                created_at=DateTime.Now,
                updated_at=DateTime.Now
            };
            var itemgroupsservice = new ItemGroupService();
            var result = itemgroupsservice.UpdateItemGroup(0, updateditemgroup);
            Assert.IsNotNull(result);
            Assert.AreEqual("Stationery", result.Result.Name);
        }
        [TestMethod]
        public void DeleteItemGroup_Test_Succes(){
            var itemgroupsservice = new ItemGroupService();
            itemgroupsservice.DeleteItemGroup(0);
            var updateditemgroups = itemgroupsservice.GetAllItemGroups();
            Assert.AreEqual(0, updateditemgroups.Count);
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
            var okResult = result.Result as OkObjectResult;
            var returnedItems = okResult.Value as IEnumerable<ItemGroupCS>;

            // Assert
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
            var okResult = result.Result as OkObjectResult;
            var returnedItem = okResult.Value as ItemGroupCS;

            // Assert
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
            var createdResult = result.Result as CreatedAtActionResult;
            var returnedItem = createdResult.Value as ItemGroupCS;

            // Assert
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
            var okResult = value.Result as OkObjectResult;
            var returnedItem = okResult.Value as ItemGroupCS;

            // Assert
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
            Assert.IsInstanceOfType(result, typeof(OkResult));
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
    }
}
