using Microsoft.VisualStudio.TestTools.UnitTesting;
using ServicesV2;
using Moq;
using ControllersV2;
using System.Data.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace TestsV2
{
    [TestClass]
    public class TransfersTests
    {
        private Mock<ITransferService> _mockTransferService;
        private TransferController _transferController;

        [TestInitialize]
        public void Setup()
        {
            _mockTransferService = new Mock<ITransferService>();
            _transferController = new TransferController(_mockTransferService.Object);
        }

        [TestMethod]
        public void GetTransfersTest_Exists()
        {
            //arrange
            var transfers = new List<TransferCS>
            {
                new TransferCS { Id = 1, Reference = "JoJo", transfer_from = 9292, transfer_to = null, transfer_status = "completed", created_at = default, updated_at = default, Items = new List<ItemIdAndAmount> ()},
                new TransferCS { Id = 2, Reference = "JoJo part 2", transfer_from = null, transfer_to = 1, transfer_status = "processing", created_at = default, updated_at = default, Items = new List<ItemIdAndAmount> ()}
            };
            _mockTransferService.Setup(service => service.GetAllTransfers()).Returns(transfers);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";  // Set the UserRole in HttpContext

            // Assign HttpContext to the controller
            _transferController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            //Act
            var value = _transferController.GetAllTransfers();
            var okResult = value.Result as OkObjectResult;
            var returnedItems = okResult.Value as IEnumerable<TransferCS>;

            //Assert
            Assert.IsNotNull(okResult);
            Assert.AreEqual(2, returnedItems.Count());
        }

        [TestMethod]
        public void GetTransferByIdTest_Exists()
        {
            //arrange
            var transfers = new List<TransferCS>
            {
                new TransferCS { Id = 1, Reference = "JoJo", transfer_from = 9292, transfer_to = null, transfer_status = "completed", created_at = default, updated_at = default, Items = new List<ItemIdAndAmount> ()},
                new TransferCS { Id = 2, Reference = "JoJo part 2", transfer_from = null, transfer_to = 1, transfer_status = "processing", created_at = default, updated_at = default, Items = new List<ItemIdAndAmount> ()}
            };
            _mockTransferService.Setup(service => service.GetTransferById(1)).Returns(transfers[0]);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";  // Set the UserRole in HttpContext

            // Assign HttpContext to the controller
            _transferController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            //Act
            var value = _transferController.GetTransferById(1);
            var okResult = value.Result as OkObjectResult;
            var returnedItems = okResult.Value as TransferCS;

            //Assert
            Assert.IsNotNull(okResult);
            Assert.IsNotNull(okResult.Value);
            Assert.AreEqual(transfers[0].transfer_from, returnedItems.transfer_from);
        }

        [TestMethod]
        public void GetTransferByIdTest_WrongId()
        {
            //arrange
            _mockTransferService.Setup(service => service.GetTransferById(1)).Returns((TransferCS)null);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";  // Set the UserRole in HttpContext

            // Assign HttpContext to the controller
            _transferController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            //Act
            var value = _transferController.GetTransferById(1);

            //Assert
            Assert.IsInstanceOfType(value.Result, typeof(NotFoundResult));
        }

        /*
[TestMethod]
        public void GetItemsInShipmentTest_Exists()
        {
            //arrange
            var items = new List<ItemIdAndAmount>
            {
                new ItemIdAndAmount { item_id = "P01", amount = 23 },
                new ItemIdAndAmount { item_id = "P02", amount = 12 },
            };
            _mockShipmentService.Setup(service => service.GetItemsInShipment(1)).Returns(items);

            //Act
            var value = _shipmentController.GetItemsInShipment(1);

            //Assert
            var okResult = value.Result as OkObjectResult;
            var returnedItems = okResult.Value as IEnumerable<ItemIdAndAmount>;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(2, returnedItems.Count());
        }
        */
        [TestMethod]
        public void GetItemsInTransferTest_Exists()
        {
            //arrange
            var items = new List<ItemIdAndAmount>
            {
                new ItemIdAndAmount { item_id = "P01", amount = 23 },
                new ItemIdAndAmount { item_id = "P02", amount = 12 },
            };
            _mockTransferService.Setup(service => service.GetItemsInTransfer(1)).Returns(items);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";  // Set the UserRole in HttpContext

            // Assign HttpContext to the controller
            _transferController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            //Act
            var value = _transferController.GetItemsInTransfer(1);
            var okResult = value.Result as OkObjectResult;
            var returnedItems = okResult.Value as IEnumerable<ItemIdAndAmount>;

            //Assert
            Assert.IsNotNull(okResult);
            Assert.AreEqual(2, returnedItems.Count());
        }
        [TestMethod]
        public void CreateTransfer_ReturnsCreatedAtActionResult_WithNewTransfer()
        {
            // Arrange
            var transfer = new TransferCS
            {
                Id = 1,
                Reference = "X",
                transfer_from = 5050,
                transfer_to = 9292,
                transfer_status = "Completed",
                Items = new List<ItemIdAndAmount> { new ItemIdAndAmount { item_id = "P007435", amount = 23 } }
            };
            _mockTransferService.Setup(service => service.CreateTransfer(transfer)).Returns(transfer);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";  // Set the UserRole in HttpContext

            // Assign HttpContext to the controller
            _transferController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = _transferController.CreateTransfer(transfer);
            var createdAtActionResult = result.Result as CreatedAtActionResult;
            var returnedTransfer = createdAtActionResult.Value as TransferCS;

            // Assert
            Assert.IsNotNull(createdAtActionResult);
            Assert.IsNotNull(returnedTransfer);
            Assert.AreEqual(1, returnedTransfer.Id);
        }

        [TestMethod]
        public void CreateMultipleTransfers_ReturnsCreatedResult_WithNewTransfers()
        {
            // Arrange
            var transfers = new List<TransferCS>
            {
                new TransferCS { Id= 1, Reference= "X", transfer_from= 5050, transfer_to= 9292, transfer_status= "Completed",
                                            Items = new List<ItemIdAndAmount> { new ItemIdAndAmount { item_id = "P007435", amount = 23 }}},
                new TransferCS { Id= 2, Reference= "Y", transfer_from= 50, transfer_to= 92, transfer_status= "Completed",
                                            Items = new List<ItemIdAndAmount> { new ItemIdAndAmount { item_id = "P007435", amount = 3 }}}
            };
            _mockTransferService.Setup(service => service.CreateMultipleTransfers(transfers)).Returns(transfers);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";  // Set the UserRole in HttpContext

            // Assign HttpContext to the controller
            _transferController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = _transferController.CreateMultipleTransfers(transfers);
            var createdResult = result.Result as ObjectResult;
            var returnedItems = createdResult.Value as List<TransferCS>;
            var firstTransfer = returnedItems[0];

            // Assert
            Assert.IsNotNull(createdResult);
            Assert.IsNotNull(returnedItems);
            Assert.AreEqual(transfers[0].transfer_from, firstTransfer.transfer_from);
            Assert.AreEqual(transfers[0].transfer_to, firstTransfer.transfer_to);
        }

        [TestMethod]
        public void UpdatedTransferTest_Success()
        {
            // Arrange
            var updatedTransfer = new TransferCS
            {
                Id = 1,
                Reference = "X",
                transfer_from = 5050,
                transfer_to = 9292,
                transfer_status = "Completed",
                Items = new List<ItemIdAndAmount> { new ItemIdAndAmount { item_id = "P007435", amount = 23 } }
            };

            _mockTransferService.Setup(service => service.UpdateTransfer(1, updatedTransfer)).Returns(updatedTransfer);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";  // Set the UserRole in HttpContext

            // Assign HttpContext to the controller
            _transferController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = _transferController.UpdateTransfer(1, updatedTransfer);
            var createdResult = result.Result as OkObjectResult;
            var returnedTransfer = createdResult.Value as TransferCS;

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            Assert.IsNotNull(createdResult);
            Assert.IsInstanceOfType(createdResult.Value, typeof(TransferCS));
            Assert.AreEqual(updatedTransfer.Reference, returnedTransfer.Reference);
            Assert.AreEqual(updatedTransfer.transfer_status, returnedTransfer.transfer_status);
        }

        [TestMethod]
        public void UpdatedTransferTest_Failed()
        {
            // Arrange
            var updatedTransfer = new TransferCS { Id = 1, Reference = "X", transfer_from = 5050, transfer_to = 9292, transfer_status = "Completed" };

            _mockTransferService.Setup(service => service.UpdateTransfer(0, updatedTransfer)).Returns((TransferCS)null);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";  // Set the UserRole in HttpContext

            // Assign HttpContext to the controller
            _transferController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = _transferController.UpdateTransfer(0, updatedTransfer);
            var createdResult = result.Result as NotFoundObjectResult;
            var returnedTransfer = createdResult.Value as TransferCS;

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(NotFoundObjectResult));
            Assert.IsNull(returnedTransfer);
        }
        [TestMethod]
        public void PatchTransfer_succes(){
            //Arrange
            var data = new TransferCS { Id = 1, Reference = "WWWWWW", transfer_from = null, transfer_to = 9292, transfer_status = "Completed" };
            _mockTransferService.Setup(service => service.PatchTransfer(1, "Reference", "WWWWWW")).Returns(data);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";  // Set the UserRole in HttpContext
            // Assign HttpContext to the controller
            _transferController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            //Act
            var result = _transferController.PatchTransfer(1, "Reference", "WWWWWW");
            var resultOk = result.Result as OkObjectResult;
            var value = resultOk.Value as TransferCS;
            //Assert
            Assert.AreEqual(resultOk.StatusCode, 200);
            Assert.AreEqual(typeof(TransferCS), value.GetType());
            Assert.AreEqual(value.Reference, "WWWWWW");
        }

        [TestMethod]
        public void UpdateTransferCommitTest()
        {
            // Arrange
            var transfer = new TransferCS
            {
                Id = 119241,
                Reference = "X",
                transfer_from = 100,
                transfer_to = null,
                transfer_status = "Completed",
                Items = new List<ItemIdAndAmount> { new ItemIdAndAmount { item_id = "P007435", amount = 23 } }
            };
            var transferCommitted = new TransferCS
            {
                Id = 119241,
                Reference = "X",
                transfer_from = 100,
                transfer_to = null,
                transfer_status = "Processed",
                Items = new List<ItemIdAndAmount> { new ItemIdAndAmount { item_id = "P007435", amount = 23 } }
            };
            _mockTransferService.Setup(service => service.CreateTransfer(transfer)).Returns(transfer);
            _mockTransferService.Setup(service => service.CommitTransfer(transfer.Id)).Returns(transferCommitted);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";  // Set the UserRole in HttpContext

            // Assign HttpContext to the controller
            _transferController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = _transferController.CreateTransfer(transfer);
            var result2 = _transferController.CommitTransfer(119241);
            var createdResult2 = result2.Result as OkObjectResult;
            var returnedTransfer2 = createdResult2.Value as TransferCS;

            // Assert
            Assert.IsInstanceOfType(result2.Result, typeof(OkObjectResult));
            Assert.IsNotNull(createdResult2);
            Assert.IsInstanceOfType(createdResult2.Value, typeof(TransferCS));
            Assert.AreEqual(transferCommitted.transfer_status, returnedTransfer2.transfer_status);
        }

        [TestMethod]
        public void DeleteTransferTest_Exist()
        {
            /*
public void DeleteWarehouseTest_Success()
        {
            // Arrange
            var warehouse = new WarehouseCS { Id = 1, Address = "Straat 1" };
            _mockWarehouseService.Setup(service => service.GetWarehouseById(1)).Returns(warehouse);
            
            // Act
            var result = _warehouseController.DeleteWarehouse(1);
            
            // Assert
            Assert.IsInstanceOfType(result, typeof(OkResult));
        }
            */
            //arrange
            var transfers = new TransferCS { Id = 1, Reference = "JoJo", transfer_from = 9292, transfer_to = null, transfer_status = "completed", created_at = default, updated_at = default, Items = new List<ItemIdAndAmount>() };
            _mockTransferService.Setup(service => service.GetTransferById(1)).Returns(transfers);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";  // Set the UserRole in HttpContext

            // Assign HttpContext to the controller
            _transferController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = _transferController.DeleteTransfer(1);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkResult));
        }

        [TestMethod]
        public void DeleteTransfersTest_Succes()
        {
            //Arrange
            var transfersToDelete = new List<int>() { 1, 2, 3 };

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";  // Set the UserRole in HttpContext

            // Assign HttpContext to the controller
            _transferController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            //Act
            var result = _transferController.DeleteTransfers(transfersToDelete);
            var resultok = result as OkObjectResult;

            //Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            Assert.AreEqual(resultok.StatusCode, 200);
        }

        [TestMethod]
        public void GetLatestTransfersTest()
        {
            //arrange
            var transfers = new List<TransferCS>
            {
                new TransferCS { Id = 1, Reference = "JoJo", transfer_from = 9292, transfer_to = null, transfer_status = "completed", created_at = default, updated_at = default, Items = new List<ItemIdAndAmount> ()},
                new TransferCS { Id = 2, Reference = "JoJo part 2", transfer_from = null, transfer_to = 1, transfer_status = "processing", created_at = default, updated_at = default, Items = new List<ItemIdAndAmount> ()}
            };
            _mockTransferService.Setup(service => service.GetLatestTransfers(5)).Returns(transfers);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";  // Set the UserRole in HttpContext

            // Assign HttpContext to the controller
            _transferController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            //Act
            var value = _transferController.GetLatestTransfers();
            var okResult = value.Result as OkObjectResult;
            var returnedItems = okResult.Value as IEnumerable<TransferCS>;

            //Assert
            Assert.IsNotNull(okResult);
            Assert.AreEqual(2, returnedItems.Count());
        }
    }
}

