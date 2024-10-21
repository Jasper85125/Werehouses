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

}
