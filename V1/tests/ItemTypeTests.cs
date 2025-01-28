using Microsoft.VisualStudio.TestTools.UnitTesting;
using ServicesV1;
using Moq;
using ControllersV1;
using System.Data.Common;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace TestsV1
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
            // Arrange
            _mockItemTypeService.Setup(service => service.GetAllItemtypes()).Returns(_itemTypes);

            // Act
            var result = _itemTypeController.GetAllItemtypes().Result as OkObjectResult;
            var itemTypes = result.Value as List<ItemTypeCS>;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, itemTypes.Count);
        }

        [TestMethod]
        public void GetItemById_ShouldReturnCorrectItemType()
        {
            // Arrange
            var itemType = _itemTypes[0];
            _mockItemTypeService.Setup(service => service.GetItemById(1)).Returns(itemType);

            // Act
            var result = _itemTypeController.GetItemById(1).Result as OkObjectResult;
            var returnedItemType = result.Value as ItemTypeCS;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(itemType, returnedItemType);
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

        // ItemTypeService tests
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
        public async Task CreateItemTypeService_Test()
        {
            var itemTypeService = new ItemTypeService();
            var newItemType = new ItemTypeCS { Id = 2, Name = "Type 2", description = "Cool items", created_at = DateTime.Now, updated_at = DateTime.Now };
            var itemTypes = await itemTypeService.CreateItemType(newItemType);
            Assert.IsNotNull(itemTypes);
            Assert.AreEqual("Type 2", itemTypes.Name);

            var itemTypesUpdated = itemTypeService.GetAllItemtypes();
            Assert.IsNotNull(itemTypesUpdated);
            Assert.AreEqual(2, itemTypesUpdated.Count);
        }

        [TestMethod]
        public async Task CreateItemTypeService_Test_Empty()
        {
            var itemTypeService = new ItemTypeService();

            itemTypeService.DeleteItemType(1);
            var itemTypesGet = itemTypeService.GetAllItemtypes();
            Assert.AreEqual(0, itemTypesGet.Count);

            var newItemType = new ItemTypeCS { Id = 2, Name = "Type 2", description = "Cool items", created_at = DateTime.Now, updated_at = DateTime.Now };
            var itemTypes = await itemTypeService.CreateItemType(newItemType);
            Assert.IsNotNull(itemTypes);
            Assert.AreEqual("Type 2", itemTypes.Name);

            var itemTypesUpdated = itemTypeService.GetAllItemtypes();
            Assert.IsNotNull(itemTypesUpdated);
            Assert.AreEqual(1, itemTypesUpdated.Count);
        }

        [TestMethod]
        public async Task UpdateItemTypeService_Test()
        {
            var itemTypeService = new ItemTypeService();
            var newItemType = new ItemTypeCS { Id = 2, Name = "Updated Type", description = "Updated Description", created_at = DateTime.Now, updated_at = DateTime.Now };

            var updatedItemType = new ItemTypeCS { Id = 1, Name = "Updated Type", description = "Updated Description" };
            var itemTypesUpdated2 = await itemTypeService.UpdateItemType(1, updatedItemType); // Await the task
            Assert.IsNotNull(itemTypesUpdated2);
            Assert.AreEqual("Updated Type", itemTypesUpdated2.Name);
        }

        [TestMethod]
        public async Task UpdateItemTypeService_Test_Failed()
        {
            var itemTypeService = new ItemTypeService();
            var updatedItemType = new ItemTypeCS { Id = 1, Name = "Updated Type", description = "Updated Description" };
            var itemTypesUpdated2 = await itemTypeService.UpdateItemType(5, updatedItemType);
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
        public void DeleteItemTypeService_Test_Empty()
        {
            var itemTypeService = new ItemTypeService();
            itemTypeService.DeleteItemType(1);
            var itemTypesGet = itemTypeService.GetAllItemtypes();
            Assert.AreEqual(0, itemTypesGet.Count);

            itemTypeService.DeleteItemType(1);
            var itemTypesGet2 = itemTypeService.GetAllItemtypes();
            Assert.AreEqual(0, itemTypesGet2.Count);
        }

        [TestMethod]
        public async Task CreateItemType_NullItemType_ShouldReturnBadRequest()
        {
            // Arrange
            ItemTypeCS newItemType = null;

            // Act
            var result = await _itemTypeController.CreateItemType(newItemType);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var badRequestResult = result as BadRequestObjectResult;
            Assert.AreEqual("ItemGroup cannot be null", badRequestResult.Value);
        }

        [TestMethod]
        public async Task CreateItemType_ValidItemType_ShouldReturnCreatedItemType()
        {
            // Arrange
            var newItemType = new ItemTypeCS { Id = 3, Name = "Type3", description = "Description3" };
            _mockItemTypeService.Setup(service => service.CreateItemType(newItemType)).ReturnsAsync(newItemType);

            // Act
            var result = await _itemTypeController.CreateItemType(newItemType);

            // Assert
            Assert.IsInstanceOfType(result, typeof(CreatedAtActionResult));
            var createdResult = result as CreatedAtActionResult;
            Assert.IsNotNull(createdResult);
            Assert.AreEqual(nameof(_itemTypeController.GetItemById), createdResult.ActionName);
            Assert.AreEqual(newItemType.Id, ((ItemTypeCS)createdResult.Value).Id);
        }

        [TestMethod]
        public async Task UpdateItemType_ExistingItem_ShouldReturnUpdatedItemType()
        {
            // Arrange
            var existingItemType = new ItemTypeCS { Id = 1, Name = "Type1", description = "Description1" };
            var updatedItemType = new ItemTypeCS { Id = 1, Name = "UpdatedType", description = "UpdatedDescription" };

            _mockItemTypeService.Setup(service => service.GetItemById(1)).Returns(existingItemType);
            _mockItemTypeService.Setup(service => service.UpdateItemType(1, updatedItemType)).ReturnsAsync(updatedItemType);

            // Act
            var result = await _itemTypeController.UpdateItemType(1, updatedItemType);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ActionResult<ItemTypeCS>));
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(updatedItemType, okResult.Value);
        }

    }
}
