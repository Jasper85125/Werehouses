using Microsoft.VisualStudio.TestTools.UnitTesting;
using Services;
using Moq;
using Controllers;
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
    public void AddItemLineTest_ValidItem()
    {
        // Arrange
        var newItemLine = new ItemLineCS { Id = 1, Description = "New Item" };
        _mockItemLineService.Setup(service => service.AddItemLine(newItemLine)).Returns(newItemLine);

        // Act
        var value = _itemLineController.AddItemLine(newItemLine);
        var createdResult = value as CreatedAtActionResult;
        var returnedItem = createdResult.Value as ItemLineCS;

        // Assert
        Assert.IsNotNull(createdResult);
        Assert.AreEqual(newItemLine.Description, returnedItem.Description);
    }

    [TestMethod]
    public void AddItemLineTest_NullItem()
    {
        // Act
        var value = _itemLineController.AddItemLine(null);
        var createdResult = value as BadRequestObjectResult;

        // Assert
        Assert.IsInstanceOfType(createdResult, typeof(BadRequestObjectResult));
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
    }

    [TestMethod]
    public void UpdateItemLineTest_ValidItem()
    {
        // Arrange
        var existingItemLine = new ItemLineCS { Id = 1, Description = "Existing Item" };
        var updatedItemLine = new ItemLineCS { Id = 1, Description = "Updated Item" };
        _mockItemLineService.Setup(service => service.GetItemLineById(1)).Returns(existingItemLine);
        _mockItemLineService.Setup(service => service.UpdateItemLine(1, updatedItemLine)).Returns(updatedItemLine);

        // Act
        var value = _itemLineController.UpdateItemLine(1, updatedItemLine);
        var okResult = value.Result as OkObjectResult;
        var returnedItem = okResult.Value as ItemLineCS;

        // Assert
        Assert.IsNotNull(okResult);
        Assert.AreEqual(updatedItemLine.Description, returnedItem.Description);
    }

    [TestMethod]
    public void UpdateItemLineTest_WrongId()
    {
        // Arrange
        var updatedItemLine = new ItemLineCS { Id = 1, Description = "Updated Item" };
        _mockItemLineService.Setup(service => service.GetItemLineById(1)).Returns((ItemLineCS)null);

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
    [TestMethod]
    public void DeleteItemLinesTest_Succes()
    {
        //Arrange
        var idstodel = new List<int>() { 1, 2, 3 };
        //Act
        var result = _itemLineController.DeleteItemLines(idstodel);
        var resultok = result as OkObjectResult;
        //Assert
        Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        Assert.AreEqual(resultok.StatusCode, 200);
    }

    [TestMethod]
    public void PatchItemLineTest_Success()
    {
        // Arrange
        var existingItemLine = new ItemLineCS { Id = 1, Description = "Existing Description" };
        var patchItemLine = new ItemLineCS { Description = "Updated Description" };

        _mockItemLineService.Setup(service => service.GetItemLineById(1)).Returns(existingItemLine);
        _mockItemLineService.Setup(service => service.PatchItemLine(1, patchItemLine)).Returns(patchItemLine);

        // Act
        var result = _itemLineController.PatchItemLine(1, patchItemLine);

        // Assert
        Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
        var okResult = result.Result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.IsInstanceOfType(okResult.Value, typeof(ItemLineCS));
        var returnedItemLine = okResult.Value as ItemLineCS;
        Assert.AreEqual(patchItemLine.Description, returnedItemLine.Description);
    }

    [TestMethod]
    public void PatchItemLineTest_NotFound()
    {
        // Arrange
        var patchItemLine = new ItemLineCS { Description = "Updated Description" };

        _mockItemLineService.Setup(service => service.GetItemLineById(1)).Returns((ItemLineCS)null);

        // Act
        var result = _itemLineController.PatchItemLine(1, patchItemLine);

        // Assert
        Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));
    }
}
