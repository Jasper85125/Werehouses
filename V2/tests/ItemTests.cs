using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ControllersV2;
using ServicesV2;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace item.TestsV2
{
    [TestClass]
    public class ItemControllerTests
    {
        private Mock<IItemService> _mockItemService;
        private Mock<IInventoryService> _mockInventoryService;
        private Mock<IItemtypeService> _mockItemTypeService;
        private Mock<ILocationService> _mockLocationService;
        private ItemController _itemController;
        private ItemTypeController _itemTypeController;

        [TestInitialize]
        public void Setup()
        {
            _mockItemService = new Mock<IItemService>();
            _mockItemTypeService = new Mock<IItemtypeService>();
            _mockInventoryService = new Mock<IInventoryService>();
            _mockLocationService = new Mock<ILocationService>();
            _itemController = new ItemController(_mockItemService.Object, _mockInventoryService.Object, _mockLocationService.Object);
            _itemTypeController = new ItemTypeController(_mockItemTypeService.Object, _mockItemService.Object);

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "../../data/items.json");
            var item = new ItemCS { uid = "P01", code = "CRD57317J", description = "Organic asymmetric data-warehouse",
                                       short_description = "particularly", upc_code = "9538419150098", item_line = 33,
                                       item_group = 1, item_type= 1, supplier_id = 28, supplier_code = "SUP467", supplier_part_number = "SUP467", created_at = DateTime.Now, updated_at = DateTime.Now};

            var itemList = new List<ItemCS> { item };
            var json = JsonConvert.SerializeObject(itemList, Formatting.Indented);

            var directory = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            File.WriteAllText(filePath, json);
        }

        [TestMethod]
        public void GetAllItems_ReturnsOkResult_WithListOfItems()
        {
            // Arrange
            var items = new List<ItemCS>
            {
                new ItemCS { uid = "1", code = "Item1" },
                new ItemCS { uid = "2", code = "Item2" }
            };
            _mockItemService.Setup(service => service.GetAllItems()).Returns(items);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";  
            httpContext.Items["WarehouseID"] = "1,2,3,4";

            _itemController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = _itemController.GetAllItems(null, 1, 10);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.IsInstanceOfType(okResult.Value, typeof(PaginationCS<ItemCS>));
            var returnedItems = okResult.Value as PaginationCS<ItemCS>;
            Assert.AreEqual(2, returnedItems.Data.Count());

            httpContext.Items["UserRole"] = "NoRole";  

            _itemController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
            result = _itemController.GetAllItems(null, 1, 10);

            var unauthorizedResult = result.Result as UnauthorizedResult;
            Assert.IsNotNull(unauthorizedResult);
            Assert.AreEqual(401, unauthorizedResult.StatusCode);
        }

        [TestMethod]
        public void GetAllItems_ReturnsOkResult_Filtered()
        {
            // Arrange
            var filtered = new itemFilter { code = "JAMADY", upc_code = "5", item_line = 33, item_group = 1, 
                                            item_type= 1, unit_purchase_quantity = 5, unit_order_quantity = 10, pack_order_quantity = 6, supplier_id = 28, supplier_code = "SUP467"};
            var items = new List<ItemCS>
            {
                new ItemCS { uid = "P02", code = "JAMADY", description = "COOL", short_description = "Jamper", upc_code = "5", item_line = 33,
                                       item_group = 1, item_type= 1, unit_purchase_quantity = 5, unit_order_quantity = 10, pack_order_quantity = 6 ,supplier_id = 28, supplier_code = "SUP467", supplier_part_number = "SUP467", 
                                       created_at = DateTime.Now, updated_at = DateTime.Now},
                new ItemCS { uid = "P03", code = "JOJO", description = "Organic asymmetric data-warehouse",
                                       short_description = "particularly", upc_code = "9538419150098", item_line = 33,
                                       item_group = 1, item_type= 1, supplier_id = 28, supplier_code = "SUP467", supplier_part_number = "SUP467", created_at = DateTime.Now, updated_at = DateTime.Now}

            };
            _mockItemService.Setup(service => service.GetAllItems()).Returns([items[0]]);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";  
            httpContext.Items["WarehouseID"] = "1,2,3,4";

            _itemController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = _itemController.GetAllItems(filtered, 0, 10);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.IsInstanceOfType(okResult.Value, typeof(PaginationCS<ItemCS>));
            var returnedItems = okResult.Value as PaginationCS<ItemCS>;
            Assert.AreEqual(1, returnedItems.Data.Count());
        }


        [TestMethod]
        public void GetByUid_ReturnsOkResult_WithItem()
        {
            // Arrange
            var item = new ItemCS { uid = "1", code = "Item1" };
            _mockItemService.Setup(service => service.GetItemById("1")).Returns(item);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";

            _itemController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = _itemController.GetByUid("1");
            var okResult = result.Result as OkObjectResult;
            var returnedItem = okResult.Value as ItemCS;

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            Assert.IsNotNull(okResult);
            Assert.IsInstanceOfType(okResult.Value, typeof(ItemCS));
            Assert.AreEqual("1", returnedItem.uid);
            Assert.AreEqual("Item1", returnedItem.code);

            httpContext.Items["UserRole"] = "NoRole";

            _itemController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
            result = _itemController.GetByUid("1");

            var unauthorizedResult = result.Result as UnauthorizedResult;
            Assert.IsNotNull(unauthorizedResult);
            Assert.AreEqual(401, unauthorizedResult.StatusCode);
        }

        [TestMethod]
        public void GetByUid_ReturnsNotFound_WhenItemDoesNotExist()
        {
            // Arrange
            _mockItemService.Setup(service => service.GetItemById("1")).Returns((ItemCS)null);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";

            _itemController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = _itemController.GetByUid("1");

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));
        }

        [TestMethod]
        public void GetInventoryForItem_Success()
        {
            InventoryCS inventory = new InventoryCS
            {
                Id = 3,
                item_id = "P000003",
                description = "gamers",
                item_reference = "QVm03739H",
                Locations = new List<int> { 5321, 21960 },
                total_on_hand = 24,
                total_expected = 0,
                total_ordered = 90,
                total_allocated = 68,
                total_available = -134
            };
            // Arrange
            _mockInventoryService.Setup(service => service.GetInventoriesForItem("P000003")).Returns(inventory);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";  

            _itemController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = _itemController.GetInventoriesForItem("P000003");
            var okResult = result.Result as OkObjectResult;
            var returnedItem = okResult.Value as InventoryCS;

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            Assert.IsNotNull(okResult);
            Assert.IsInstanceOfType(okResult.Value, typeof(InventoryCS));
            Assert.AreEqual("P000003", returnedItem.item_id);
            Assert.AreEqual(24, returnedItem.total_on_hand);
            Assert.AreEqual(68, returnedItem.total_allocated);

            httpContext.Items["UserRole"] = "NoRole";

            _itemController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
            result = _itemController.GetInventoriesForItem("P000003");

            var unauthorizedResult = result.Result as UnauthorizedResult;
            Assert.IsNotNull(unauthorizedResult);
            Assert.AreEqual(401, unauthorizedResult.StatusCode);
        }

        [TestMethod]
        public void GetItemsWithItemType_Success()
        {
            // Arrange
            List<ItemCS> items = new List<ItemCS>
            {
                new ItemCS { uid = "P000123", code = "CRD57317J", description = "Organic asymmetric data-warehouse",
                                       short_description = "particularly", upc_code = "9538419150098", item_line = 33,
                                       item_group = 2, item_type= 1, supplier_id = 28, supplier_code = "SUP467"},
                new ItemCS { uid = "P100000", code = "CRD57317J", description = "Organic asymmetric data-warehouse",
                                       short_description = "particularly", upc_code = "9538419150098", item_line = 33,
                                       item_group = 2, item_type= 1, supplier_id = 28, supplier_code = "SUP467"}
            };

            _mockItemService.Setup(service => service.GetAllItemsInItemType(1)).Returns(items);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin"; 

            _itemController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
            _itemTypeController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = _itemTypeController.GetAllItemsInItemType(1);
            var okResult = result.Result as OkObjectResult;
            var returnedItems = okResult.Value as IEnumerable<ItemCS>;

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            Assert.IsNotNull(okResult);
            Assert.IsInstanceOfType(okResult.Value, typeof(IEnumerable<ItemCS>));
            Assert.AreEqual(2, returnedItems.Count());

            httpContext.Items["UserRole"] = "NoRole";

            _itemTypeController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
            result = _itemTypeController.GetAllItemsInItemType(1);

            var unauthorizedResult = result.Result as UnauthorizedResult;
            Assert.IsNotNull(unauthorizedResult);
            Assert.AreEqual(401, unauthorizedResult.StatusCode);
        }

        [TestMethod]
        public void CreateItem_ReturnsCreatedResult_WithNewItem()
        {
            // Arrange
            var newItem = new ItemCS { uid = "P000001", code = "NewItem" };
            var createdItem = new ItemCS { uid = "P000002", code = "NewItem" };
            _mockItemService.Setup(service => service.CreateItem(newItem)).Returns(createdItem);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";

            _itemController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = _itemController.CreateItem(newItem);
            var createdResult = result.Result as CreatedAtActionResult;
            var returnedItem = createdResult.Value as ItemCS;

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(CreatedAtActionResult));
            Assert.IsNotNull(createdResult);
            Assert.IsInstanceOfType(createdResult.Value, typeof(ItemCS));
            Assert.AreEqual("P000002", returnedItem.uid);
            Assert.AreEqual("NewItem", returnedItem.code);

            httpContext.Items["UserRole"] = "NoRole";

            _itemController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
            result = _itemController.CreateItem(newItem);

            var unauthorizedResult = result.Result as UnauthorizedResult;
            Assert.IsNotNull(unauthorizedResult);
            Assert.AreEqual(401, unauthorizedResult.StatusCode);
        }

        [TestMethod]
        public void CreateMultipleItems_ReturnsCreatedResult_WithNewItems()
        {
            // Arrange
            List<ItemCS> items = new List<ItemCS>
            {
                new ItemCS { uid = "P000123", code = "CRD57317J", description = "Organic asymmetric data-warehouse",
                                       short_description = "particularly", upc_code = "9538419150098", item_line = 33,
                                       item_group = 2, item_type= 1, supplier_id = 28, supplier_code = "SUP467"},
                new ItemCS { uid = "P100000", code = "CRD57317J", description = "Organic asymmetric data-warehouse",
                                       short_description = "particularly", upc_code = "9538419150098", item_line = 33,
                                       item_group = 2, item_type= 1, supplier_id = 28, supplier_code = "SUP467"}
            };
            _mockItemService.Setup(service => service.CreateMultipleItems(items)).Returns(items);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";

            _itemController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = _itemController.CreateMultipleItems(items);
            var createdResult = result.Result as ObjectResult;
            var returnedItems = createdResult.Value as List<ItemCS>;
            var firstItem = returnedItems[0];

            // Assert
            Assert.IsNotNull(createdResult);
            Assert.IsNotNull(returnedItems);
            Assert.AreEqual(items[0].code, firstItem.code);
            Assert.AreEqual(items[0].supplier_code, firstItem.supplier_code);
            Assert.AreEqual(items[0].item_group, firstItem.item_group);

            httpContext.Items["UserRole"] = "NoRole";

            _itemController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
            result = _itemController.CreateMultipleItems(items);

            var unauthorizedResult = result.Result as UnauthorizedResult;
            Assert.IsNotNull(unauthorizedResult);
            Assert.AreEqual(401, unauthorizedResult.StatusCode);
        }

        [TestMethod]
        public void UpdateItem_ReturnsOkResult_WithUpdatedItem()
        {
            // Arrange
            var existingItem = new ItemCS { uid = "P000001", code = "ExistingItem" };
            var updatedItem = new ItemCS { uid = "P000001", code = "UpdatedItem" };
            _mockItemService.Setup(service => service.GetItemById("P000001")).Returns(existingItem);
            _mockItemService.Setup(service => service.UpdateItem("P000001", updatedItem)).Returns(updatedItem);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";

            _itemController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = _itemController.UpdateItem("P000001", updatedItem);
            var okResult = result.Result as OkObjectResult;
            var returnedItem = okResult.Value as ItemCS;

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            Assert.IsNotNull(okResult);
            Assert.IsInstanceOfType(okResult.Value, typeof(ItemCS));
            Assert.AreEqual("P000001", returnedItem.uid);
            Assert.AreEqual("UpdatedItem", returnedItem.code);

            httpContext.Items["UserRole"] = "NoRole";

            _itemController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
            result = _itemController.UpdateItem("P000001", updatedItem);

            var unauthorizedResult = result.Result as UnauthorizedResult;
            Assert.IsNotNull(unauthorizedResult);
            Assert.AreEqual(401, unauthorizedResult.StatusCode);
        }

        [TestMethod]
        public void UpdateItem_ReturnsNotFound_WhenItemDoesNotExist()
        {
            // Arrange
            var updatedItem = new ItemCS { uid = "P000001", code = "UpdatedItem" };
            _mockItemService.Setup(service => service.GetItemById("P000001")).Returns((ItemCS)null);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin"; 

            _itemController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = _itemController.UpdateItem("P000001", updatedItem);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));
        }

        [TestMethod]
        public void PatchItem_Succes()
        {
            //Arrange
            var patcheditem = new ItemCS() { uid = "P000001", code = "lol no" };
            _mockItemService.Setup(service => service.PatchItem("P000001", "code", "lol no")).Returns(patcheditem);
            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";  

            _itemController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            //Act
            var result = _itemController.PatchItem("P000001", "code", "lol no");
            var resultok = result.Result as OkObjectResult;
            var value = resultok.Value as ItemCS;
            
            //Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(resultok);
            Assert.IsNotNull(value);
            Assert.AreEqual(resultok.StatusCode, 200);
            Assert.AreEqual(value.code, "lol no");

            httpContext.Items["UserRole"] = "NoRole";

            _itemController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
            result = _itemController.PatchItem("P000001", "code", "lol no");

            var unauthorizedResult = result.Result as UnauthorizedResult;
            Assert.IsNotNull(unauthorizedResult);
            Assert.AreEqual(401, unauthorizedResult.StatusCode);
        }

        [TestMethod]
        public void DeleteItem_ReturnsOkResult_WhenItemIsDeleted()
        {
            // Arrange
            var existingItem = new ItemCS { uid = "P000001", code = "ExistingItem" };
            _mockItemService.Setup(service => service.GetItemById("P000001")).Returns(existingItem);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin"; 

            _itemController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = _itemController.DeleteItem("P000001");

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkResult));

            httpContext.Items["UserRole"] = "NoRole";

            _itemController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
            result = _itemController.DeleteItem("P000001");

            var unauthorizedResult = result as UnauthorizedResult;
            Assert.IsNotNull(unauthorizedResult);
            Assert.AreEqual(401, unauthorizedResult.StatusCode);
        }

        [TestMethod]
        public void DeleteItems_ReturnsOkResult_WhenItemsAreDeleted()
        {
            // Arrange
            var existingItems = new List<ItemCS>
            {
                new ItemCS { uid = "P000001", code = "ExistingItem1" },
                new ItemCS { uid = "P000002", code = "ExistingItem2" }
            };
            _mockItemService.Setup(service => service.GetAllItems()).Returns(existingItems);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";

            _itemController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = _itemController.DeleteMultipleItems(new List<string> { "P000001", "P000002" });

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkResult));

            httpContext.Items["UserRole"] = "NoRole";

            _itemController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
            result = _itemController.DeleteMultipleItems(new List<string> { "P000001", "P000002" });

            var unauthorizedResult = result as UnauthorizedResult;
            Assert.IsNotNull(unauthorizedResult);
            Assert.AreEqual(401, unauthorizedResult.StatusCode);
        }
        
        [TestMethod]
        public void GetAllItemsService_Test()
        {
            var itemService = new ItemService();
            var items = itemService.GetAllItems();
            Assert.IsNotNull(items);
            Assert.AreEqual(1, items.Count);
        }

        [TestMethod]
        public void GetItemByIdService_Test()
        {
            var itemService = new ItemService();
            var items = itemService.GetItemById("P01");
            Assert.IsNotNull(items);
            Assert.AreEqual("CRD57317J", items.code);
        }

        [TestMethod]
        public void GetItemsInItemType_Test()
        {
            var itemService = new ItemService();
            var items = itemService.GetAllItemsInItemType(1);
            Assert.IsNotNull(items);
            Assert.AreEqual(1, items.Count());
        }

        [TestMethod]
        public void GenerateReportService_Test()
        {
            var itemService = new ItemService();
            itemService.GenerateReport(new List<string> { "P01", "P02" });
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "reports/report.txt");
            Assert.IsTrue(File.Exists(filePath));
        }

        [TestMethod]
        public void CreateItemService_Test()
        {
            var itemService = new ItemService();
            var item = new ItemCS { uid = "P02", code = "CRD57317J", description = "Organic asymmetric data-warehouse",
                                       short_description = "particularly", upc_code = "9538419150098", item_line = 33,
                                       item_group = 2, item_type= 1, supplier_id = 28, supplier_code = "SUP467", supplier_part_number = "SUP467", created_at = DateTime.Now, updated_at = DateTime.Now};
            var createdItem = itemService.CreateItem(item);
            Assert.IsNotNull(createdItem);
            Assert.AreEqual("CRD57317J", createdItem.code);

            var items = itemService.GetAllItems();
            Assert.AreEqual(2, items.Count);
        }

        [TestMethod]
        public void CreateItemService_Test_Empty()
        {
            var itemService = new ItemService();
            itemService.DeleteItem("P01");
            var itemEmpty = itemService.GetAllItems();
            Assert.AreEqual(0, itemEmpty.Count());

            var item = new ItemCS { uid = "P02", code = "Cool", description = "Organic asymmetric data-warehouse",
                                       short_description = "particularly", upc_code = "9538419150098", item_line = 33,
                                       item_group = 2, item_type= 1, supplier_id = 28, supplier_code = "SUP467", supplier_part_number = "SUP467", created_at = DateTime.Now, updated_at = DateTime.Now};
            var createdItem = itemService.CreateItem(item);
            Assert.IsNotNull(createdItem);
            Assert.AreEqual("Cool", createdItem.code);

            var items = itemService.GetAllItems();
            Assert.AreEqual(1, items.Count);
        }

        [TestMethod]
        public void CreateMultipleItemGroupsService_Test()
        {
            var itemService = new ItemService();
            var items = new List<ItemCS>
            {
                new ItemCS { uid = "P02", code = "CRD57317J", description = "Organic asymmetric data-warehouse",
                                       short_description = "particularly", upc_code = "9538419150098", item_line = 33,
                                       item_group = 2, item_type= 1, supplier_id = 28, supplier_code = "SUP467", supplier_part_number = "SUP467", created_at = DateTime.Now, updated_at = DateTime.Now},
                new ItemCS { uid = "P03", code = "CRD57317J", description = "Organic asymmetric data-warehouse",
                                       short_description = "particularly", upc_code = "9538419150098", item_line = 33,
                                       item_group = 2, item_type= 1, supplier_id = 28, supplier_code = "SUP467", supplier_part_number = "SUP467", created_at = DateTime.Now, updated_at = DateTime.Now}
            };
            var createdItems = itemService.CreateMultipleItems(items);
            Assert.IsNotNull(createdItems);

            var allItems = itemService.GetAllItems();
            Assert.AreEqual(3, allItems.Count);
        }

        [TestMethod]
        public void UpdateItemService_Test()
        {
            var itemService = new ItemService();
            var item = new ItemCS { uid = "P02", code = "New Code", description = "Organic asymmetric data-warehouse",
                                       short_description = "particularly", upc_code = "9538419150098", item_line = 33,
                                       item_group = 2, item_type= 1, supplier_id = 28, supplier_code = "SUP467", supplier_part_number = "SUP467", created_at = DateTime.Now, updated_at = DateTime.Now};
            var updatedItem = itemService.UpdateItem("P01", item);
            Assert.IsNotNull(updatedItem);
            Assert.AreEqual("New Code", updatedItem.code);
        }

        [TestMethod]
        public void UpdateItemService_Test_Failed()
        {
            var itemService = new ItemService();
            var item = new ItemCS { uid = "P02", code = "New Code", description = "Organic asymmetric data-warehouse",
                                       short_description = "particularly", upc_code = "9538419150098", item_line = 33,
                                       item_group = 2, item_type= 1, supplier_id = 28, supplier_code = "SUP467", supplier_part_number = "SUP467", created_at = DateTime.Now, updated_at = DateTime.Now};
            var updatedItem = itemService.UpdateItem("P1", item);
            Assert.IsNull(updatedItem);
        }

        [TestMethod]
        public void DeleteItemGroupService_Test()
        {
            var itemService = new ItemService();
            itemService.DeleteItem("P01");
            var itemEmpty = itemService.GetAllItems();
            Assert.AreEqual(0, itemEmpty.Count());
        }

        [TestMethod]
        public void DeleteItemGroupService_Test_Failed()
        {
            var itemService = new ItemService();
            itemService.DeleteItem("P001");
            var itemEmpty = itemService.GetAllItems();
            Assert.AreEqual(1, itemEmpty.Count());
        }
        
        [TestMethod]
        public void DeleteMultipleItemGroupService_Test()
        {
            var itemService = new ItemService();
            var item = new ItemCS { uid = "P02", code = "CRD57317J", description = "Organic asymmetric data-warehouse",
                                       short_description = "particularly", upc_code = "9538419150098", item_line = 33,
                                       item_group = 2, item_type= 1, supplier_id = 28, supplier_code = "SUP467", supplier_part_number = "SUP467", created_at = DateTime.Now, updated_at = DateTime.Now};
            var createdItem = itemService.CreateItem(item);
            var itemsToDelete = new List<string> { "P01", "P000002" };
            itemService.DeleteItems(itemsToDelete);
            var itemEmpty = itemService.GetAllItems();
            Assert.AreEqual(0, itemEmpty.Count());
        }

        [TestMethod]
        public void PatchItemService_Test()
        {
            var itemService = new ItemService();
            var patchedItem = itemService.PatchItem("P01", "code", "new code");
            patchedItem = itemService.PatchItem("P01", "description", "new description");
            patchedItem = itemService.PatchItem("P01", "short_description", "new short description");
            patchedItem = itemService.PatchItem("P01", "upc_code", "new upc code");
            patchedItem = itemService.PatchItem("P01", "model_number", "new model number");
            patchedItem = itemService.PatchItem("P01", "commodity_code", "new commodity code");
            patchedItem = itemService.PatchItem("P01", "item_line", 2);
            patchedItem = itemService.PatchItem("P01", "item_group", 2);
            patchedItem = itemService.PatchItem("P01", "item_type", 2);
            patchedItem = itemService.PatchItem("P01", "unit_purchase_quantity", 4);
            patchedItem = itemService.PatchItem("P01", "unit_order_quantity", 3);
            patchedItem = itemService.PatchItem("P01", "pack_order_quantity", 2);
            patchedItem = itemService.PatchItem("P01", "supplier_id", 2);
            patchedItem = itemService.PatchItem("P01", "supplier_code", "new supplier code");
            patchedItem = itemService.PatchItem("P01", "supplier_part_number", "new supplier part number");
            var patchedItemGoneWrong = itemService.PatchItem("-1", "supplier_part_number", "new supplier part number");
            Assert.IsNotNull(patchedItem);
            Assert.IsNull(patchedItemGoneWrong);
            Assert.AreEqual("new code", patchedItem.code);
            Assert.AreEqual("new description", patchedItem.description);
            Assert.AreEqual("new short description", patchedItem.short_description);
            Assert.AreEqual("new upc code", patchedItem.upc_code);
            Assert.AreEqual("new model number", patchedItem.model_number);
            Assert.AreEqual("new commodity code", patchedItem.commodity_code);
            Assert.AreEqual(2, patchedItem.item_line);
            Assert.AreEqual(2, patchedItem.item_group);
            Assert.AreEqual(2, patchedItem.item_type);
            Assert.AreEqual(4, patchedItem.unit_purchase_quantity);
            Assert.AreEqual(3, patchedItem.unit_order_quantity);
            Assert.AreEqual(2, patchedItem.pack_order_quantity);
            Assert.AreEqual(2, patchedItem.supplier_id);
            Assert.AreEqual("new supplier code", patchedItem.supplier_code);
            Assert.AreEqual("new supplier part number", patchedItem.supplier_part_number);
        }
    }
}
