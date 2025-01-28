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
            var itemgroup = new ItemGroupCS()
            {
                Id = 0,
                Name = "Electronics",
                Description = "",
                created_at = DateTime.Now,
                updated_at = DateTime.Now
            };
            var itemgrouplist = new List<ItemGroupCS>() { itemgroup };
            var json = JsonConvert.SerializeObject(itemgrouplist, Formatting.Indented);
            var directory = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            File.WriteAllText(filePath, json);
        }
        //Service
        [TestMethod]
        public void GetAllItemGroups_Test_Succes()
        {
            var itemgroupsservice = new ItemGroupService();
            var result = itemgroupsservice.GetAllItemGroups();
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
        }

        [TestMethod]
        public void GetItemGroupById_Test_Succes()
        {
            var itemgroupsservice = new ItemGroupService();
            var result = itemgroupsservice.GetItemById(0);
            Assert.IsNotNull(result);
            Assert.AreEqual("Electronics", result.Name);
        }

        [TestMethod]
        public void GetItemsFromItemGroup_Test_Fail()
        {
            var itemgroupsservice = new ItemGroupService();
            var result = itemgroupsservice.ItemsFromItemGroupId(-1);
            Assert.IsNull(result);
        }

        [TestMethod]
        public void CreateItemGroup_Test_Succes()
        {
            var itemgroup = new ItemGroupCS()
            {
                Name = "Furniture",
                Description = "",
                created_at = DateTime.Now,
                updated_at = DateTime.Now
            };
            var itemgroupsservice = new ItemGroupService();
            var result = itemgroupsservice.CreateItemGroup(itemgroup);
            var check = itemgroupsservice.GetAllItemGroups();
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Id);
            Assert.AreEqual(2, check.Count);
        }
        [TestMethod]
        public void CreateItemGroup_Test_EmptyListFirst()
        {
            var itemgroupsservice = new ItemGroupService();
            itemgroupsservice.DeleteItemGroup(0);
            var check1 = itemgroupsservice.GetAllItemGroups();
            Assert.AreEqual(0, check1.Count);
            var result2 = itemgroupsservice.CreateItemGroup(new ItemGroupCS());
            var check2 = itemgroupsservice.GetAllItemGroups();
            Assert.IsNotNull(result2);
            Assert.AreEqual(1, check2.Count);
        }
        [TestMethod]
        public void UpdateItemGroup_Test_Succes()
        {
            var itemgroup = new ItemGroupCS()
            {
                Name = "Furniture",
                Description = "",
                created_at = DateTime.Now,
                updated_at = DateTime.Now
            };
            var updateditemgroup = new ItemGroupCS()
            {
                Id = 1,
                Name = "Stationery",
                Description = "",
                created_at = DateTime.Now,
                updated_at = DateTime.Now
            };
            var itemgroupsservice = new ItemGroupService();
            var result1 = itemgroupsservice.CreateItemGroup(itemgroup);
            var result2 = itemgroupsservice.UpdateItemGroup(1, updateditemgroup);
            Assert.IsNotNull(result1);
            Assert.IsNotNull(result2);
            Assert.AreEqual("Furniture", result1.Name);
            Assert.AreEqual("Stationery", result2.Name);
        }

        [TestMethod]
        public void UpdateItemGroup_Test_Fail()
        {
            var updateditemgroup = new ItemGroupCS()
            {
                Id = 1,
                Name = "Measuring Instruments",
                Description = "",
                created_at = DateTime.Now,
                updated_at = DateTime.Now
            };
            var itemgroupsservice = new ItemGroupService();
            var result = itemgroupsservice.UpdateItemGroup(3, updateditemgroup);
            Assert.IsNull(result);
        }

        [TestMethod]
        public void DeleteItemGroup_Test_Succes()
        {
            var itemgroupsservice = new ItemGroupService();
            itemgroupsservice.DeleteItemGroup(0);
            var updateditemgroups = itemgroupsservice.GetAllItemGroups();
            Assert.AreEqual(0, updateditemgroups.Count);
        }
        [TestMethod]
        public void DeleteItemGroup_Test_Fail()
        {
            var itemgroupsservice = new ItemGroupService();
            itemgroupsservice.DeleteItemGroup(3);
            var result2 = itemgroupsservice.GetAllItemGroups();
            Assert.AreEqual(1, result2.Count);
        }
        //Controller
        [TestMethod]
        public void GetAllItemGroups_Test_Exists()
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
        public void GetItemGroupById_Test_Exists()
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
        public void GetItemGroupById_Test_WrongId()
        {
            // Arrange
            _mockItemGroupService.Setup(service => service.GetItemById(1)).Returns((ItemGroupCS)null);

            // Act
            var result = _itemGroupController.GetItemById(1);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));
        }

        [TestMethod]
        public void CreateItemGroup_Test_Success()
        {
            // Arrange
            var newItemGroup = new ItemGroupCS { Id = 3, Name = "Group 3" };
            _mockItemGroupService.Setup(service => service.CreateItemGroup(newItemGroup)).Returns(newItemGroup);

            // Act
            var result = _itemGroupController.CreateItemGroup(newItemGroup);
            var createdResult = result as CreatedAtActionResult;
            var returnedItem = createdResult.Value as ItemGroupCS;

            // Assert
            Assert.IsNotNull(createdResult);
            Assert.AreEqual(newItemGroup.Name, returnedItem.Name);
        }

        [TestMethod]
        public void UpdateItemGroup_Test_ValidItem()
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
        public void UpdateItemGroup_Test_WrongId()
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
