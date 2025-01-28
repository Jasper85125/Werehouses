using Microsoft.VisualStudio.TestTools.UnitTesting;
using ServicesV2;
using Moq;
using ControllersV2;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace TestsV2;

[TestClass]
public class ItemLineTests
{
    private Mock<IItemLineService> _mockItemLineService;
    private ItemLineController _itemLineController;

    [TestInitialize]
    public void Setup()
    {
        _mockItemLineService = new Mock<IItemLineService>();
        _itemLineController = new ItemLineController(_mockItemLineService.Object);
    
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "../../data/item_lines.json");
        var itemLine = new ItemLineCS { Id = 1, Name = "Line 1", Description = "Cool items", created_at = DateTime.Now, updated_at = DateTime.Now };

        var itemLineList = new List<ItemLineCS> { itemLine };
        var json = JsonConvert.SerializeObject(itemLineList, Formatting.Indented);

        var directory = Path.GetDirectoryName(filePath);
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        File.WriteAllText(filePath, json);
    }

    [TestMethod]
    public void GetItemLinesTest_Exists()
    {
        // Arrange
        var itemLines = new List<ItemLineCS>
            {
                new ItemLineCS { Id = 1, Description = "Item 1" },
                new ItemLineCS { Id = 2, Description = "Item 2" }
            };
        _mockItemLineService.Setup(service => service.GetAllItemlines()).Returns(itemLines);

        var httpContext = new DefaultHttpContext();
        httpContext.Items["UserRole"] = "Admin";

        _itemLineController.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };

        // Act
        var value = _itemLineController.GetAllItemLines();
        var okResult = value.Result as OkObjectResult;
        var returnedItems = okResult.Value as IEnumerable<ItemLineCS>;

        // Assert
        Assert.IsNotNull(okResult);
        Assert.AreEqual(2, returnedItems.Count());

        httpContext.Items["UserRole"] = "NoRole";
        _itemLineController.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };

        //act
        var result = _itemLineController.GetAllItemLines();

        //assert
        var unauthorizedResult = result.Result as UnauthorizedResult;
        Assert.IsNotNull(unauthorizedResult);
        Assert.AreEqual(401, unauthorizedResult.StatusCode);
    }

    [TestMethod]
    public void GetItemLineByIdTest_Exists()
    {
        // Arrange
        var itemLine = new ItemLineCS { Id = 1, Description = "Item 1" };
        _mockItemLineService.Setup(service => service.GetItemLineById(1)).Returns(itemLine);

        var httpContext = new DefaultHttpContext();
        httpContext.Items["UserRole"] = "Admin"; 

        _itemLineController.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };

        // Act
        var value = _itemLineController.GetItemLineById(1);
        var okResult = value.Result as OkObjectResult;
        var returnedItem = okResult.Value as ItemLineCS;

        // Assert
        Assert.IsNotNull(okResult);
        Assert.IsNotNull(okResult.Value);
        Assert.AreEqual(itemLine.Description, returnedItem.Description);

        httpContext.Items["UserRole"] = "NoRole";
        _itemLineController.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };

        //act
        var secondResult = _itemLineController.GetItemLineById(1);

        //assert
        var unauthorizedResult = secondResult.Result as UnauthorizedResult;
        Assert.IsNotNull(unauthorizedResult);
        Assert.AreEqual(401, unauthorizedResult.StatusCode);
    }

    [TestMethod]
    public void GetItemLineByIdTest_WrongId()
    {
        // Arrange
        _mockItemLineService.Setup(service => service.GetItemLineById(1)).Returns((ItemLineCS)null);

        var httpContext = new DefaultHttpContext();
        httpContext.Items["UserRole"] = "Admin";

        _itemLineController.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };

        // Act
        var value = _itemLineController.GetItemLineById(1);

        // Assert
        Assert.IsInstanceOfType(value.Result, typeof(NotFoundResult));

        httpContext.Items["UserRole"] = "NoRole";
        _itemLineController.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };

        //act
        var result = _itemLineController.GetItemLineById(1);

        //assert
        var unauthorizedResult = result.Result as UnauthorizedResult;
        Assert.IsNotNull(unauthorizedResult);
        Assert.AreEqual(401, unauthorizedResult.StatusCode);
    }

    [TestMethod]
    public void AddItemLineTest_ValidItem()
    {
        // Arrange
        var newItemLine = new ItemLineCS { Id = 1, Description = "New Item" };
        _mockItemLineService.Setup(service => service.AddItemLine(newItemLine)).Returns(newItemLine);

        var httpContext = new DefaultHttpContext();
        httpContext.Items["UserRole"] = "Admin";

        _itemLineController.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };

        // Act
        var value = _itemLineController.AddItemLine(newItemLine);
        var createdResult = value as CreatedAtActionResult;
        var returnedItem = createdResult.Value as ItemLineCS;

        // Assert
        Assert.IsNotNull(createdResult);
        Assert.AreEqual(newItemLine.Description, returnedItem.Description);

        httpContext.Items["UserRole"] = "NoRole";
        _itemLineController.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };

        //act
        var result = _itemLineController.AddItemLine(newItemLine);

        //assert
        var unauthorizedResult = result as UnauthorizedResult;
        Assert.IsNotNull(unauthorizedResult);
        Assert.AreEqual(401, unauthorizedResult.StatusCode);
    }

    [TestMethod]
    public void AddItemLineTest_NullItem()
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Items["UserRole"] = "Admin";

        _itemLineController.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };

        // Act
        var value = _itemLineController.AddItemLine(null);
        var createdResult = value as BadRequestObjectResult;

        // Assert
        Assert.IsInstanceOfType(createdResult, typeof(BadRequestObjectResult));

        httpContext.Items["UserRole"] = "NoRole";
        _itemLineController.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };

        //act
        var result = _itemLineController.AddItemLine(null);

        //assert
        var unauthorizedResult = result as UnauthorizedResult;
        Assert.IsNotNull(unauthorizedResult);
        Assert.AreEqual(401, unauthorizedResult.StatusCode);
    }

    [TestMethod]
    public void CreateMultipleWarehouse_ReturnsCreatedResult_WithNewWarehouse()
    {
        // Arrange
        var itemLines = new List<ItemLineCS>
            {
                new ItemLineCS { Id = 3, Name = "Group 3", Description = "Cool items" },
                new ItemLineCS { Id = 4, Name = "Group 4", Description = "Cool items" }
            };
        _mockItemLineService.Setup(service => service.CreateMultipleItemLines(itemLines)).Returns(itemLines);

        var httpContext = new DefaultHttpContext();
        httpContext.Items["UserRole"] = "Admin";

        _itemLineController.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };

        // Act
        var result = _itemLineController.CreateMultipleItemLines(itemLines);
        var createdResult = result.Result as ObjectResult;
        var returnedItems = createdResult.Value as List<ItemLineCS>;
        var firstItemLine = returnedItems[0];

        // Assert
        Assert.IsNotNull(createdResult);
        Assert.IsNotNull(returnedItems);
        Assert.AreEqual(itemLines[0].Name, firstItemLine.Name);
        Assert.AreEqual(itemLines[0].Description, firstItemLine.Description);

        httpContext.Items["UserRole"] = "NoRole";
        _itemLineController.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };

        //act
        result = _itemLineController.CreateMultipleItemLines(itemLines);

        //assert
        var unauthorizedResult = result.Result as UnauthorizedResult;
        Assert.IsNotNull(unauthorizedResult);
        Assert.AreEqual(401, unauthorizedResult.StatusCode);
    }

    [TestMethod]
    public void UpdateItemLineTest_ValidItem()
    {
        // Arrange
        var existingItemLine = new ItemLineCS { Id = 1, Description = "Existing Item" };
        var updatedItemLine = new ItemLineCS { Id = 1, Description = "Updated Item" };
        _mockItemLineService.Setup(service => service.GetItemLineById(1)).Returns(existingItemLine);
        _mockItemLineService.Setup(service => service.UpdateItemLine(1, updatedItemLine)).Returns(updatedItemLine);

        var httpContext = new DefaultHttpContext();
        httpContext.Items["UserRole"] = "Admin"; 

        _itemLineController.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };

        // Act
        var value = _itemLineController.UpdateItemLine(1, updatedItemLine);
        var okResult = value.Result as OkObjectResult;
        var returnedItem = okResult.Value as ItemLineCS;

        // Assert
        Assert.IsNotNull(okResult);
        Assert.AreEqual(updatedItemLine.Description, returnedItem.Description);

        httpContext.Items["UserRole"] = "NoRole";
        _itemLineController.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };

        //act
        var result = _itemLineController.UpdateItemLine(1, updatedItemLine);

        //assert
        var unauthorizedResult = result.Result as UnauthorizedResult;
        Assert.IsNotNull(unauthorizedResult);
        Assert.AreEqual(401, unauthorizedResult.StatusCode);
    }

    [TestMethod]
    public void UpdateItemLineTest_WrongId()
    {
        // Arrange
        var updatedItemLine = new ItemLineCS { Id = 1, Description = "Updated Item" };
        _mockItemLineService.Setup(service => service.GetItemLineById(1)).Returns((ItemLineCS)null);

        var httpContext = new DefaultHttpContext();
        httpContext.Items["UserRole"] = "Admin";

        _itemLineController.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };

        // Act
        var value = _itemLineController.UpdateItemLine(1, updatedItemLine);

        // Assert
        Assert.IsInstanceOfType(value.Result, typeof(NotFoundResult));
    }

    [TestMethod]
    public void UpdateItemLineTest_IdMismatch()
    {
        // Arrange
        var updatedItemLine = new ItemLineCS { Id = 2, Description = "Updated Item" };

        var httpContext = new DefaultHttpContext();
        httpContext.Items["UserRole"] = "Admin";  

        _itemLineController.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };

        // Act
        var value = _itemLineController.UpdateItemLine(1, updatedItemLine);

        // Assert
        Assert.IsInstanceOfType(value.Result, typeof(BadRequestResult));
    }

    [TestMethod]
    public void DeleteItemLineTest_Exists()
    {
        // Arrange
        var itemLine = new ItemLineCS { Id = 1, Description = "Item 1" };
        _mockItemLineService.Setup(service => service.GetItemLineById(1)).Returns(itemLine);

        var httpContext = new DefaultHttpContext();
        httpContext.Items["UserRole"] = "Admin";  

        _itemLineController.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };

        // Act
        var value = _itemLineController.DeleteItemLine(1);

        // Assert
        Assert.IsInstanceOfType(value, typeof(OkResult));

        httpContext.Items["UserRole"] = "NoRole";
        _itemLineController.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };

        //act
        var result = _itemLineController.DeleteItemLine(1);

        //assert
        var unauthorizedResult = result as UnauthorizedResult;
        Assert.IsNotNull(unauthorizedResult);
        Assert.AreEqual(401, unauthorizedResult.StatusCode);
    }


    [TestMethod]
    public void GetItemsByItemLineId_ExistingId()
    {
        // Arrange
        var itemLine = new ItemLineCS { Id = 1 };
        _mockItemLineService.Setup(service => service.GetItemLineById(1)).Returns(itemLine);

        var items = new List<ItemCS>
        {
            new ItemCS { uid = "U001", code = "Item1", item_line = 1 },
            new ItemCS { uid = "U002", code = "Item2", item_line = 1 }
        };
        _mockItemLineService.Setup(service => service.GetItemsByItemLineId(1)).Returns(items);

        var httpContext = new DefaultHttpContext();
        httpContext.Items["UserRole"] = "Admin";

        _itemLineController.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };

        // Act
        var result = _itemLineController.GetItemsByItemLineId(1);

        // Assert
        Assert.IsNotNull(result);
        var okResult = result.Result as OkObjectResult;
        Assert.IsNotNull(okResult, "Expected OkObjectResult, but got null.");

        var returnedItems = okResult.Value as IEnumerable<ItemCS>;
        Assert.IsNotNull(returnedItems, "Expected returnedItems to be non-null.");
        Assert.AreEqual(2, returnedItems.Count(), "Expected returnedItems to contain 2 items.");

        httpContext.Items["UserRole"] = "NoRole";
        _itemLineController.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };

        //act
        result = _itemLineController.GetItemsByItemLineId(1);

        //assert
        var unauthorizedResult = result.Result as UnauthorizedResult;
        Assert.IsNotNull(unauthorizedResult);
        Assert.AreEqual(401, unauthorizedResult.StatusCode);
    }


    [TestMethod]
    public void GetItemsByItemLineIdTest_WrongId()
    {
        // Arrange
        _mockItemLineService.Setup(service => service.GetItemsByItemLineId(1)).Returns((List<ItemCS>)null);

        var httpContext = new DefaultHttpContext();
        httpContext.Items["UserRole"] = "Admin";

        _itemLineController.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };

        // Act
        var value = _itemLineController.GetItemsByItemLineId(1);

        // Assert
        Assert.IsInstanceOfType(value.Result, typeof(NotFoundResult));
    }

    [TestMethod]
    public void DeleteItemLinesTest_Succes()
    {
        //Arrange
        var itemlinesToDelete = new List<int>() { 1, 2, 3 };

        var httpContext = new DefaultHttpContext();
        httpContext.Items["UserRole"] = "Admin"; 

        _itemLineController.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };

        //Act
        var result = _itemLineController.DeleteItemLines(itemlinesToDelete);
        var resultok = result as OkObjectResult;

        //Assert
        Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        Assert.AreEqual(resultok.StatusCode, 200);

        httpContext.Items["UserRole"] = "NoRole";
        _itemLineController.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };

        //act
        var secondResult = _itemLineController.DeleteItemLine(1);

        //assert
        var unauthorizedResult = secondResult as UnauthorizedResult;
        Assert.IsNotNull(unauthorizedResult);
        Assert.AreEqual(401, unauthorizedResult.StatusCode);
    }

    [TestMethod]
    public void PatchItemLineTest_Success()
    {
        // Arrange
        var existingItemLine = new ItemLineCS { Id = 1, Description = "Existing Description" };
        var patchItemLine = new ItemLineCS { Description = "Updated Description" };

        _mockItemLineService.Setup(service => service.GetItemLineById(1)).Returns(existingItemLine);
        _mockItemLineService.Setup(service => service.PatchItemLine(1, "Description", "Updated Description")).Returns(patchItemLine);

        var httpContext = new DefaultHttpContext();
        httpContext.Items["UserRole"] = "Admin";

        _itemLineController.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };

        // Act
        var result = _itemLineController.PatchItemLine(1, "Description", "Updated Description");
        var okResult = result.Result as OkObjectResult;
        var returnedItemLine = okResult.Value as ItemLineCS;

        // Assert
        Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
        Assert.IsNotNull(okResult);
        Assert.IsInstanceOfType(okResult.Value, typeof(ItemLineCS));
        Assert.AreEqual(patchItemLine.Description, returnedItemLine.Description);

        httpContext.Items["UserRole"] = "NoRole";
        _itemLineController.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };

        //act
        var secondResult = _itemLineController.PatchItemLine(1, "Description", "Updated Description");

        //assert
        var unauthorizedResult = secondResult.Result as UnauthorizedResult;
        Assert.IsNotNull(unauthorizedResult);
        Assert.AreEqual(401, unauthorizedResult.StatusCode);
    }

    [TestMethod]
    public void PatchItemLineTest_NotFound()
    {
        // Arrange
        var patchItemLine = new ItemLineCS { Description = "Updated Description" };

        _mockItemLineService.Setup(service => service.GetItemLineById(1)).Returns((ItemLineCS)null);

        var httpContext = new DefaultHttpContext();
        httpContext.Items["UserRole"] = "Admin";

        _itemLineController.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };

        // Act
        var result = _itemLineController.PatchItemLine(1, "Description", "Updated Description");

        // Assert
        Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));

        httpContext.Items["UserRole"] = "NoRole";
        _itemLineController.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };

        //act
        var secondResult = _itemLineController.PatchItemLine(1, "Description", "Updated Description");

        //assert
        var unauthorizedResult = secondResult.Result as UnauthorizedResult;
        Assert.IsNotNull(unauthorizedResult);
        Assert.AreEqual(401, unauthorizedResult.StatusCode);
    }

    [TestMethod]
    public void GetAllItemLinesService_Test()
    {
        var itemLinesService = new ItemLineService();
        var itemLines = itemLinesService.GetAllItemlines();
        Assert.IsNotNull(itemLines);
        Assert.AreEqual(1, itemLines.Count);
    }

    [TestMethod]
    public void GetItemLineByIdService_Test()
    {
        var itemLinesService = new ItemLineService();
        var itemLines = itemLinesService.GetItemLineById(1);
        Assert.IsNotNull(itemLines);
        Assert.AreEqual("Line 1", itemLines.Name);
    }

    [TestMethod]
    public void GetItemsByItemLineIdService_Test()
    {
        var itemLinesService = new ItemLineService();
        var items = itemLinesService.GetItemsByItemLineId(2);
        Assert.IsNotNull(items);
        Assert.AreEqual(1, items.Count);
    }

    [TestMethod]
    public void CreateItemLineService_Test()
    {
        var itemLine = new ItemLineCS { Id = 2, Name = "Line 2", Description = "Cool items 2", created_at = DateTime.Now, updated_at = DateTime.Now };
        var itemLinesService = new ItemLineService();
        var itemLines = itemLinesService.AddItemLine(itemLine);
        Assert.IsNotNull(itemLines);
        Assert.AreEqual("Line 2", itemLines.Name);

        var itemLinesUpdated = itemLinesService.GetAllItemlines();
        Assert.AreEqual(2, itemLinesUpdated.Count);
    }

    [TestMethod]
    public void CreateItemLineService_Test_Empty()
    {
        var itemLinesService = new ItemLineService();
        itemLinesService.DeleteItemLine(1);
        var itemLinesUpdated = itemLinesService.GetAllItemlines();
        Assert.AreEqual(0, itemLinesUpdated.Count);
        
        var itemLine = new ItemLineCS { Id = 1, Name = "Line 2", Description = "Cool items 2", created_at = DateTime.Now, updated_at = DateTime.Now };
        var itemLines = itemLinesService.AddItemLine(itemLine);
        Assert.IsNotNull(itemLines);
        Assert.AreEqual("Line 2", itemLines.Name);

        var itemLinesUpdatedAgain = itemLinesService.GetAllItemlines();
        Assert.AreEqual(1, itemLinesUpdatedAgain.Count);
    }

    [TestMethod]
    public void CreateMultipleItemLineService_Test()
    {
        var itemLine = new List<ItemLineCS> {
                new ItemLineCS { Id = 2, Name = "Line 2", Description = "Cool items 2", created_at = DateTime.Now, updated_at = DateTime.Now },
                new ItemLineCS { Id = 3, Name = "Line 3", Description = "Cool items 3", created_at = DateTime.Now, updated_at = DateTime.Now }
            };
        var itemLinesService = new ItemLineService();
        var itemLines = itemLinesService.CreateMultipleItemLines(itemLine);
        Assert.IsNotNull(itemLines);

        var itemLinesUpdated = itemLinesService.GetAllItemlines();
        Assert.AreEqual(3, itemLinesUpdated.Count);
    }

    [TestMethod]
    public void UpdateItemLineService_Test()
    {
        var itemLine = new ItemLineCS { Id = 1, Name = "Updated Line", Description = "Cool items 2", created_at = DateTime.Now, updated_at = DateTime.Now };
        var itemLinesService = new ItemLineService();
        var itemLines = itemLinesService.UpdateItemLine(1, itemLine);
        Assert.IsNotNull(itemLines);
        Assert.AreEqual("Updated Line", itemLines.Name);
    }

    [TestMethod]
    public void UpdateItemLineService_Test_Failed()
    {
        var itemLine = new ItemLineCS { Id = 1, Name = "Updated Line", Description = "Cool items 2", created_at = DateTime.Now, updated_at = DateTime.Now };
        var itemLinesService = new ItemLineService();
        var itemLines = itemLinesService.UpdateItemLine(2, itemLine);
        Assert.IsNull(itemLines);
    }

    [TestMethod]
    public void DeleteItemLineService_Test()
    {
        var itemLinesService = new ItemLineService();
        itemLinesService.DeleteItemLine(1);
        var itemLinesUpdated = itemLinesService.GetAllItemlines();
        Assert.AreEqual(0, itemLinesUpdated.Count);
    }

    [TestMethod]
    public void DeleteItemLineService_Test_Failed()
    {
        var itemLinesService = new ItemLineService();
        itemLinesService.DeleteItemLine(4);
        var itemLinesUpdated = itemLinesService.GetAllItemlines();
        Assert.AreEqual(1, itemLinesUpdated.Count);
    }

    [TestMethod]
    public void DeleteMultipleItemLineService_Test()
    {
        var itemLinesService = new ItemLineService();
        var itemLine = new ItemLineCS { Id = 1, Name = "Line 2", Description = "Cool items 2", created_at = DateTime.Now, updated_at = DateTime.Now };
        var itemLines = itemLinesService.AddItemLine(itemLine);
        Assert.IsNotNull(itemLines);
        Assert.AreEqual("Line 2", itemLines.Name);

        var itemLinesUpdated = itemLinesService.GetAllItemlines();
        Assert.AreEqual(2, itemLinesUpdated.Count);

        List<int> ItemLinesToDelete = new List<int> { 1, 2 };
        itemLinesService.DeleteItemLines(ItemLinesToDelete);
        var itemLinesUpdatedAgain = itemLinesService.GetAllItemlines();
        Assert.AreEqual(0, itemLinesUpdatedAgain.Count);
    }

    [TestMethod]
    public void PatchItemLineService_Test()
    {
         var itemLinesService = new ItemLineService();
         var ItemLines = itemLinesService.PatchItemLine(1, "Name", "Updated Name");
         ItemLines = itemLinesService.PatchItemLine(1, "Description", "Updated Description");
         var ItemLinesGoneWrong = itemLinesService.PatchItemLine(2, "Name", "Updated Name");
        Assert.IsNotNull(ItemLines);
        Assert.IsNull(ItemLinesGoneWrong);
        Assert.AreEqual("Updated Name", ItemLines.Name);
        Assert.AreEqual("Updated Description", ItemLines.Description);
    }


}
