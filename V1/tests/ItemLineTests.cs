using Microsoft.VisualStudio.TestTools.UnitTesting;
using ServicesV1;
using Moq;
using ControllersV1;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace TestsV1;

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
        var itemLine = new ItemLineCS
        {
            Id= 0,
            Name= "Tech Gadgets",
            Description= "",
            created_at=DateTime.Now,
            updated_at=DateTime.Now
        };
        var itemlineslist = new List<ItemLineCS>(){ itemLine };
        var directory = Path.GetDirectoryName(filePath);
        var json = JsonConvert.SerializeObject(itemlineslist, Formatting.Indented);
        if(!Directory.Exists(directory)){
            Directory.CreateDirectory(directory);
        }
        File.WriteAllText(filePath, json);
    }
    [TestMethod]
    public void GetAllItemlinesService_Test_Succes(){
        var itemlineservice = new ItemLineService();
        var result = itemlineservice.GetAllItemlines();
        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Count);
    }
    
    [TestMethod]
    public void GetItemLineByIdService_Test_Succes(){
        var itemlineservice = new ItemLineService();
        var result = itemlineservice.GetItemLineById(0);
        Assert.IsNotNull(result);
        Assert.AreEqual("Tech Gadgets", result.Name);
    }

    [TestMethod]
    public void GetItemLineByIdService_Test_Fail(){
        var itemlineservice = new ItemLineService();
        var result = itemlineservice.GetItemLineById(-1);
        Assert.IsNull(result);
    }

    [TestMethod]
    public void AddItemLineService_Test_Succes(){
        var newitemline = new ItemLineCS(){
            Name="Mountaineering Gear",
            Description="",
            created_at=DateTime.Now,
            updated_at=DateTime.Now
        };
        var itemlineservice = new ItemLineService();
        var result = itemlineservice.AddItemLine(newitemline);
        var check = itemlineservice.GetAllItemlines();
        Assert.IsNotNull(result);
        Assert.AreEqual(2, check.Count);
    }

    public void AddItemLineService_Test_EmptyListFirst(){
        var newitemline = new ItemLineCS(){
            Name="Mountaineering Gear",
            Description="",
            created_at=DateTime.Now,
            updated_at=DateTime.Now
        };
        var itemlineservice = new ItemLineService();
        itemlineservice.DeleteItemLine(0);
        var result = itemlineservice.AddItemLine(newitemline);
        var check = itemlineservice.GetAllItemlines();
        Assert.IsNotNull(result);
        Assert.AreEqual(1, check.Count);
        Assert.AreEqual(0, check[0].Id);
    }
    
    [TestMethod]
    public void UpdateItemLineService_Test_Succes(){
        var updatedItemLine = new ItemLineCS(){
            Name="Bouldering",
            Description="",
            created_at= DateTime.Now,
            updated_at= DateTime.Now
        };
        var itemlineservice = new ItemLineService();
        var result = itemlineservice.UpdateItemLine(0, updatedItemLine);
        var check = itemlineservice.GetAllItemlines();
        Assert.IsNotNull(check);
        Assert.IsNotNull(result);
        Assert.AreEqual("Bouldering", check[0].Name);
    }

    [TestMethod]
    public void UpdateItemLineService_Test_Fail(){
        var updatedItemLine = new ItemLineCS(){
            Name="Bouldering",
            Description="",
            created_at= DateTime.Now,
            updated_at= DateTime.Now
        };
        var itemlineservice = new ItemLineService();
        var result = itemlineservice.UpdateItemLine(-1, updatedItemLine);
        var check = itemlineservice.GetAllItemlines();
        Assert.IsNull(result);
        Assert.AreEqual("Tech Gadgets", check[0].Name);
    }
    
    [TestMethod]
    public void DeleteItemLineService_Test_Succes(){
        var itemlineservice = new ItemLineService();
        itemlineservice.DeleteItemLine(0);
        var check = itemlineservice.GetAllItemlines();
        Assert.IsNotNull(check);
        Assert.AreEqual(0, check.Count);
    }
    
    [TestMethod]
    public void DeleteItemLineService_Test_Fail(){
        var itemlineservice = new ItemLineService();
        itemlineservice.DeleteItemLine(-1);
        var check = itemlineservice.GetAllItemlines();
        Assert.IsNotNull(check);
        Assert.AreEqual(1, check.Count);
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
        var okResult = value.Result as OkObjectResult;
        var returnedItem = okResult.Value as ItemLineCS;

        // Assert
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
        var createdResult = value.Result as CreatedAtActionResult;
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

        // Assert
        Assert.IsInstanceOfType(value.Result, typeof(BadRequestObjectResult));
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
        // Arrange
        var itemLine = new ItemLineCS { Id = 1 };
        _mockItemLineService.Setup(service => service.GetItemLineById(1)).Returns(itemLine);

        var items = new List<ItemCS>
        {
            new ItemCS { uid = "U001", code = "Item1", item_line = 1 },
            new ItemCS { uid = "U002", code = "Item2", item_line = 1 }
        };
        _mockItemLineService.Setup(service => service.GetItemsByItemLineId(1)).Returns(items);

        // Act
        var result = _itemLineController.GetItemsByItemLineId(1);

        // Assert
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
