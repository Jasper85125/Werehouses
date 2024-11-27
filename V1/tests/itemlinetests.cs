using Microsoft.VisualStudio.TestTools.UnitTesting;
using ServicesV1;
using Moq;
using ControllersV1;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace Tests;

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

        // Act
        var value = _itemLineController.GetAllItemLines();

        // Assert
        var okResult = value.Result as OkObjectResult;
        var returnedItems = okResult.Value as IEnumerable<ItemLineCS>;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(2, returnedItems.Count());
    }

    [TestMethod]
    public void GetItemLineByIdTest_Exists()
    {
        // Arrange
        var itemLine = new ItemLineCS { Id = 1, Description = "Item 1" };
        _mockItemLineService.Setup(service => service.GetItemLineById(1)).Returns(itemLine);

        // Act
        var value = _itemLineController.GetItemLineById(1);

        // Assert
        var okResult = value.Result as OkObjectResult;
        var returnedItem = okResult.Value as ItemLineCS;
        Assert.IsNotNull(okResult);
        Assert.IsNotNull(okResult.Value);
        Assert.AreEqual(itemLine.Description, returnedItem.Description);
    }

    [TestMethod]
    public void GetItemLineByIdTest_WrongId()
    {
        // Arrange
        _mockItemLineService.Setup(service => service.GetItemLineById(1)).Returns((ItemLineCS)null);

        // Act
        var value = _itemLineController.GetItemLineById(1);

        // Assert
        Assert.IsInstanceOfType(value.Result, typeof(NotFoundResult));
    }

    [TestMethod]
    public async Task AddItemLineTest_ValidItem()
    {
        // Arrange
        var newItemLine = new ItemLineCS { Id = 1, Description = "New Item" };
        _mockItemLineService.Setup(service => service.AddItemLine(newItemLine)).ReturnsAsync(newItemLine);

        // Act
        var value = await _itemLineController.AddItemLine(newItemLine);

        // Assert
        var createdResult = value.Result as CreatedAtActionResult;
        var returnedItem = createdResult.Value as ItemLineCS;
        Assert.IsNotNull(createdResult);
        Assert.AreEqual(newItemLine.Description, returnedItem.Description);
    }

    [TestMethod]
    public async Task AddItemLineTest_NullItem()
    {
        // Act
        var value = await _itemLineController.AddItemLine(null);

        // Assert
        Assert.IsInstanceOfType(value.Result, typeof(BadRequestObjectResult));
    }

    [TestMethod]
    public async Task UpdateItemLineTest_ValidItem()
    {
        // Arrange
        var existingItemLine = new ItemLineCS { Id = 1, Description = "Existing Item" };
        var updatedItemLine = new ItemLineCS { Id = 1, Description = "Updated Item" };
        _mockItemLineService.Setup(service => service.GetItemLineById(1)).Returns(existingItemLine);
        _mockItemLineService.Setup(service => service.UpdateItemLine(1, updatedItemLine)).ReturnsAsync(updatedItemLine);

        // Act
        var value = await _itemLineController.UpdateItemLine(1, updatedItemLine);

        // Assert
        var okResult = value.Result as OkObjectResult;
        var returnedItem = okResult.Value as ItemLineCS;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(updatedItemLine.Description, returnedItem.Description);
    }

    [TestMethod]
    public async Task UpdateItemLineTest_WrongId()
    {
        // Arrange
        var updatedItemLine = new ItemLineCS { Id = 1, Description = "Updated Item" };
        _mockItemLineService.Setup(service => service.GetItemLineById(1)).Returns((ItemLineCS)null);

        // Act
        var value = await _itemLineController.UpdateItemLine(1, updatedItemLine);

        // Assert
        Assert.IsInstanceOfType(value.Result, typeof(NotFoundResult));
    }

    [TestMethod]
    public async Task UpdateItemLineTest_IdMismatch()
    {
        // Arrange
        var updatedItemLine = new ItemLineCS { Id = 2, Description = "Updated Item" };

        // Act
        var value = await _itemLineController.UpdateItemLine(1, updatedItemLine);

        // Assert
        Assert.IsInstanceOfType(value.Result, typeof(BadRequestResult));
    }
    
    [TestMethod]
    public void DeleteItemLineTest_Exists()
    {
        // Arrange
        var itemLine = new ItemLineCS { Id = 1, Description = "Item 1" };
        _mockItemLineService.Setup(service => service.GetItemLineById(1)).Returns(itemLine);

        // Act
        var value = _itemLineController.DeleteItemLine(1);

        // Assert
        Assert.IsInstanceOfType(value, typeof(OkResult));
    }


    [TestMethod]
    public void GetItemsByItemLineId_ExistingId()
    {
        // Arrange: Mock the service responses to ensure the controller returns the expected items
        var itemLine = new ItemLineCS { Id = 1 };  // Create a mock item line
        _mockItemLineService.Setup(service => service.GetItemLineById(1)).Returns(itemLine);

        var items = new List<ItemCS>
        {
            new ItemCS { uid = "U001", code = "Item1", item_line = 1 },
            new ItemCS { uid = "U002", code = "Item2", item_line = 1 }
        };
        _mockItemLineService.Setup(service => service.GetItemsByItemLineId(1)).Returns(items);

        // Act: Call the controller method
        var result = _itemLineController.GetItemsByItemLineId(1);

        // Assert: Verify the result
        Assert.IsNotNull(result);
        var okResult = result.Result as OkObjectResult;
        Assert.IsNotNull(okResult, "Expected OkObjectResult, but got null.");

        var returnedItems = okResult.Value as IEnumerable<ItemCS>;
        Assert.IsNotNull(returnedItems, "Expected returnedItems to be non-null.");
        Assert.AreEqual(2, returnedItems.Count(), "Expected returnedItems to contain 2 items.");
    }


    [TestMethod]
    public void GetItemsByItemLineIdTest_WrongId()
    {
        // Arrange
        _mockItemLineService.Setup(service => service.GetItemsByItemLineId(1)).Returns((List<ItemCS>)null);

        // Act
        var value = _itemLineController.GetItemsByItemLineId(1);

        // Assert
        Assert.IsInstanceOfType(value.Result, typeof(NotFoundResult));
    }
}
