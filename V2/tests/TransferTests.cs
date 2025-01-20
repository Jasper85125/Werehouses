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
            httpContext.Items["UserRole"] = "Admin";

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
            httpContext.Items["UserRole"] = "Admin";

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

            httpContext.Items["UserRole"] = "User";
            _transferController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            //act
            var result = _transferController.GetAllTransfers();

            //assert
            var unauthorizedResult = result.Result as UnauthorizedResult;
            Assert.IsNotNull(unauthorizedResult);
            Assert.AreEqual(401, unauthorizedResult.StatusCode);
        }

        [TestMethod]
        public void GetTransferByIdTest_WrongId()
        {
            //arrange
            _mockTransferService.Setup(service => service.GetTransferById(1)).Returns((TransferCS)null);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";  

            _transferController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            //Act
            var value = _transferController.GetTransferById(1);

            //Assert
            Assert.IsInstanceOfType(value.Result, typeof(NotFoundResult));

            httpContext.Items["UserRole"] = "User";
            _transferController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            //act
            var result = _transferController.GetTransferById(1);

            //assert
            var unauthorizedResult = result.Result as UnauthorizedResult;
            Assert.IsNotNull(unauthorizedResult);
            Assert.AreEqual(401, unauthorizedResult.StatusCode);
        }

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
            httpContext.Items["UserRole"] = "Admin";

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

            httpContext.Items["UserRole"] = "User";
            _transferController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            //act
            var result = _transferController.GetItemsInTransfer(1);

            //assert
            var unauthorizedResult = result.Result as UnauthorizedResult;
            Assert.IsNotNull(unauthorizedResult);
            Assert.AreEqual(401, unauthorizedResult.StatusCode);
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
            httpContext.Items["UserRole"] = "Admin";

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

            httpContext.Items["UserRole"] = "User";
            _transferController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            //act
            result = _transferController.CreateTransfer(transfer);

            //assert
            var unauthorizedResult = result.Result as UnauthorizedResult;
            Assert.IsNotNull(unauthorizedResult);
            Assert.AreEqual(401, unauthorizedResult.StatusCode);
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
            httpContext.Items["UserRole"] = "Admin";

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

            httpContext.Items["UserRole"] = "User";
            _transferController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            //act
            result = _transferController.CreateMultipleTransfers(transfers);

            //assert
            var unauthorizedResult = result.Result as UnauthorizedResult;
            Assert.IsNotNull(unauthorizedResult);
            Assert.AreEqual(401, unauthorizedResult.StatusCode);
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
            httpContext.Items["UserRole"] = "Admin";

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

            httpContext.Items["UserRole"] = "User";
            _transferController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            //act
            result = _transferController.UpdateTransfer(1, updatedTransfer);

            //assert
            var unauthorizedResult = result.Result as UnauthorizedResult;
            Assert.IsNotNull(unauthorizedResult);
            Assert.AreEqual(401, unauthorizedResult.StatusCode);
        }

        [TestMethod]
        public void UpdatedTransferTest_Failed()
        {
            // Arrange
            var updatedTransfer = new TransferCS { Id = 1, Reference = "X", transfer_from = 5050, transfer_to = 9292, transfer_status = "Completed" };

            _mockTransferService.Setup(service => service.UpdateTransfer(0, updatedTransfer)).Returns((TransferCS)null);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";

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

            httpContext.Items["UserRole"] = "User";
            _transferController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            //act
            result = _transferController.UpdateTransfer(1, updatedTransfer);

            //assert
            var unauthorizedResult = result.Result as UnauthorizedResult;
            Assert.IsNotNull(unauthorizedResult);
            Assert.AreEqual(401, unauthorizedResult.StatusCode);
        }
        [TestMethod]
        public void PatchTransfer_succes(){
            //Arrange
            var data = new TransferCS { Id = 1, Reference = "WWWWWW", transfer_from = null, transfer_to = 9292, transfer_status = "Completed" };
            _mockTransferService.Setup(service => service.PatchTransfer(1, "Reference", "WWWWWW")).Returns(data);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";

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

            httpContext.Items["UserRole"] = "User";
            _transferController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            //act
            result = _transferController.PatchTransfer(1, "Reference", "WWWWWW");

            //assert
            var unauthorizedResult = result.Result as UnauthorizedResult;
            Assert.IsNotNull(unauthorizedResult);
            Assert.AreEqual(401, unauthorizedResult.StatusCode);
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
            httpContext.Items["UserRole"] = "Admin";

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

            httpContext.Items["UserRole"] = "User";
            _transferController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            //act
            result = _transferController.CreateTransfer(transfer);
            var result1 = _transferController.CommitTransfer(119241);

            //assert
            var unauthorizedResult = result.Result as UnauthorizedResult;
            Assert.IsNotNull(unauthorizedResult);
            Assert.AreEqual(401, unauthorizedResult.StatusCode);
            var unauthorizedResult1 = result1.Result as UnauthorizedResult;
            Assert.IsNotNull(unauthorizedResult1);
            Assert.AreEqual(401, unauthorizedResult1.StatusCode);
        }

        [TestMethod]
        public void DeleteTransferTest_Exist()
        {
            //arrange
            var transfers = new TransferCS { Id = 1, Reference = "JoJo", transfer_from = 9292, transfer_to = null, transfer_status = "completed", created_at = default, updated_at = default, Items = new List<ItemIdAndAmount>() };
            _mockTransferService.Setup(service => service.GetTransferById(1)).Returns(transfers);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";

            _transferController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = _transferController.DeleteTransfer(1);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkResult));

            httpContext.Items["UserRole"] = "User";
            _transferController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            //act
            result = _transferController.DeleteTransfer(1);

            //assert
            var unauthorizedResult = result as UnauthorizedResult;
            Assert.IsNotNull(unauthorizedResult);
            Assert.AreEqual(401, unauthorizedResult.StatusCode);
        }

        [TestMethod]
        public void DeleteTransfersTest_Succes()
        {
            //Arrange
            var transfersToDelete = new List<int>() { 1, 2, 3 };

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";

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

            httpContext.Items["UserRole"] = "User";
            _transferController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            //act
            result = _transferController.DeleteTransfers(transfersToDelete);

            //assert
            var unauthorizedResult = result as UnauthorizedResult;
            Assert.IsNotNull(unauthorizedResult);
            Assert.AreEqual(401, unauthorizedResult.StatusCode);
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
            httpContext.Items["UserRole"] = "Admin";  

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

            httpContext.Items["UserRole"] = "User";
            _transferController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            //act
            var result = _transferController.GetLatestTransfers();

            //assert
            var unauthorizedResult = result.Result as UnauthorizedResult;
            Assert.IsNotNull(unauthorizedResult);
            Assert.AreEqual(401, unauthorizedResult.StatusCode);
        }
    }
}

