using Microsoft.VisualStudio.TestTools.UnitTesting;
using ServicesV1;
using Moq;
using ControllersV1;
using System.Data.Common;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;


namespace TestsV1
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

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "../../data/transfers.json");
            var transfer = new TransferCS
            {
                Id = 1,
                Reference = "JoJo",
                transfer_from = 1,
                transfer_to = 1,
                transfer_status = "completed",
                created_at = default,
                updated_at = default,
                Items = new List<ItemIdAndAmount>
                {
                    new ItemIdAndAmount { item_id = "P01", amount = 23 }
                }
            };

            var transferList = new List<TransferCS> { transfer };
            var json = JsonConvert.SerializeObject(transferList, Formatting.Indented);

            var directory = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            File.WriteAllText(filePath, json);
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

            //Act
            var value = _transferController.GetAllTransfers();

            //Assert
            var okResult = value.Result as OkObjectResult;
            var returnedItems = okResult.Value as IEnumerable<TransferCS>;
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

            //Act
            var value = _transferController.GetTransferById(1);

            //Assert
            var okResult = value.Result as OkObjectResult;
            var returnedItems = okResult.Value as TransferCS;
            Assert.IsNotNull(okResult);
            Assert.IsNotNull(okResult.Value);
            Assert.AreEqual(transfers[0].transfer_from, returnedItems.transfer_from);
        }

        [TestMethod]
        public void GetTransferByIdTest_WrongId()
        {
            //arrange
            _mockTransferService.Setup(service => service.GetTransferById(1)).Returns((TransferCS)null);

            //Act
            var value = _transferController.GetTransferById(1);

            //Assert
            Assert.IsInstanceOfType(value.Result, typeof(NotFoundResult));
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

            //Act
            var value = _transferController.GetItemsInTransfer(1);

            //Assert
            var okResult = value.Result as OkObjectResult;
            var returnedItems = okResult.Value as IEnumerable<ItemIdAndAmount>;
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

            // Act
            var result = _transferController.CreateTransfer(transfer);

            // Assert
            var createdAtActionResult = result.Result as CreatedAtActionResult;
            var returnedTransfer = createdAtActionResult.Value as TransferCS;
            Assert.IsNotNull(createdAtActionResult);
            Assert.IsNotNull(returnedTransfer);
            Assert.AreEqual(1, returnedTransfer.Id);
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

            // Act
            var result = _transferController.UpdateTransfer(0, updatedTransfer);
            var createdResult = result.Result as NotFoundObjectResult;
            var returnedTransfer = createdResult.Value as TransferCS;

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(NotFoundObjectResult));
            Assert.IsNull(returnedTransfer);
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
            //arrange
            var transfers = new TransferCS { Id = 1, Reference = "JoJo", transfer_from = 9292, transfer_to = null, transfer_status = "completed", created_at = default, updated_at = default, Items = new List<ItemIdAndAmount>() };
            _mockTransferService.Setup(service => service.GetTransferById(1)).Returns(transfers);

            // Act
            var result = _transferController.DeleteTransfer(1);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkResult));
        }

        [TestMethod]
        public void GetAllTransfersService_Test()
        {
            var transferService = new TransferService();
            var transfers = transferService.GetAllTransfers();
            Assert.IsNotNull(transfers);
            Assert.AreEqual(1, transfers.Count);
        }

        [TestMethod]
        public void GetTransferByIdService_Test()
        {
            var transferService = new TransferService();
            var transfers = transferService.GetTransferById(1);
            Assert.IsNotNull(transfers);
            Assert.AreEqual("JoJo", transfers.Reference);
        }

        [TestMethod]
        public void GetItemsInTransferService_Test()
        {
            var transferService = new TransferService();
            var transfers = transferService.GetItemsInTransfer(1);
            Assert.IsNotNull(transfers);
            Assert.AreEqual("P01", transfers.First().item_id);
        }

        [TestMethod]
        public void GetItemsInTransferService_Test_Failed()
        {
            var transferService = new TransferService();
            var transfers = transferService.GetItemsInTransfer(4);
            Assert.IsNull(transfers);
        }

        [TestMethod]
        public void CreateTransferService_Test()
        {
            var transferService = new TransferService();
            var transfer = new TransferCS
            {
                Id = 1,
                Reference = "JoJo",
                transfer_from = 9292,
                transfer_to = null,
                transfer_status = "completed",
                created_at = default,
                updated_at = default,
                Items = new List<ItemIdAndAmount>
                {
                    new ItemIdAndAmount { item_id = "P01", amount = 23 }
                }
            };
            var result = transferService.CreateTransfer(transfer);
            Assert.IsNotNull(result);

            var resultAgain = transferService.GetAllTransfers();
            Assert.AreEqual(2, resultAgain.Count);
        }

        [TestMethod]
        public void UpdatedTransferService_Test()
        {
            var transferService = new TransferService();
            var transfer = new TransferCS
            {
                Id = 1,
                Reference = "Updated JoJo",
                transfer_from = 9292,
                transfer_to = null,
                transfer_status = "Pending",
                created_at = default,
                updated_at = default,
                Items = new List<ItemIdAndAmount>
                {
                    new ItemIdAndAmount { item_id = "P02", amount = 23 }
                }
            };
            var result = transferService.UpdateTransfer(1, transfer);
            Assert.IsNotNull(result);
            Assert.AreEqual("Updated JoJo", result.Reference);
        }

        [TestMethod]
        public void CommitTransferService_Test_Failed()
        {
            var transferService = new TransferService();
            var result = transferService.CommitTransfer(5);
            Assert.IsNull(result);
        }

        [TestMethod]
        public void DeleteTransferService_Test()
        {
            var transferService = new TransferService();
            transferService.DeleteTransfer(1);
            var resultAgain = transferService.GetAllTransfers();
            Assert.AreEqual(0, resultAgain.Count);
        }

        [TestMethod]
        public void DeleteTransferService_Test_Failed()
        {
            var transferService = new TransferService();
            transferService.DeleteTransfer(5);
            var resultAgain = transferService.GetAllTransfers();
            Assert.AreEqual(1, resultAgain.Count);
        }

        [TestMethod]
        public void CommitTransferTest_Success()
        {
            // Arrange
            var transfer = new TransferCS
            {
                Id = 1,
                Reference = "JoJo",
                transfer_from = 1,
                transfer_to = 1,
                transfer_status = "completed",
                created_at = default,
                updated_at = default,
                Items = new List<ItemIdAndAmount>
                {
                    new ItemIdAndAmount { item_id = "P01", amount = 23 }
                }
            };
            var committedTransfer = new TransferCS
            {
                Id = 1,
                Reference = "JoJo",
                transfer_from = 1,
                transfer_to = 1,
                transfer_status = "Processed",
                created_at = default,
                updated_at = default,
                Items = new List<ItemIdAndAmount>
                {
                    new ItemIdAndAmount { item_id = "P01", amount = 23 }
                }
            };

            _mockTransferService.Setup(service => service.CommitTransfer(1)).Returns(committedTransfer);

            // Act
            var result = _transferController.CommitTransfer(1);
            var okResult = result.Result as OkObjectResult;
            var returnedTransfer = okResult.Value as TransferCS;

            // Assert
            Assert.IsNotNull(okResult);
            Assert.IsNotNull(returnedTransfer);
            Assert.AreEqual("Processed", returnedTransfer.transfer_status);
        }
        
        [TestMethod]
        public void CommitTransferTest_Failed()
        {
            // Arrange
            _mockTransferService.Setup(service => service.CommitTransfer(0)).Returns((TransferCS)null);

            // Act
            var result = _transferController.CommitTransfer(0);
            var notFoundResult = result.Result as NotFoundObjectResult;

            // Assert
            Assert.IsNotNull(notFoundResult);
        }

        [TestMethod]
        public void GetItemsInTransferTest_Failed()
        {
            //arrange
            _mockTransferService.Setup(service => service.GetItemsInTransfer(1)).Returns((List<ItemIdAndAmount>)null);

            //Act
            var value = _transferController.GetItemsInTransfer(1);

            //Assert
            Assert.IsInstanceOfType(value.Result, typeof(NotFoundResult));
        }

        
    }
}

