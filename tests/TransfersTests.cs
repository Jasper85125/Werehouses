using Microsoft.VisualStudio.TestTools.UnitTesting;
using Services;
using Moq;
using Controllers;
using System.Data.Common;
using Microsoft.AspNetCore.Mvc;

namespace Tests
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
        public void CreateTransfer_ReturnsCreatedAtActionResult_WithNewTransfer()
        {
            // Arrange
            var transfer = new TransferCS { Id = 1, transfer_from = 1, transfer_to = 2 };
            _mockTransferService.Setup(service => service.CreateTransfer(transfer)).Returns(transfer);

            // Act
            var result = _transferController.Post(transfer);

            // Assert
            var createdAtActionResult = result.Result as CreatedAtActionResult;
            var returnedTransfer = createdAtActionResult.Value as TransferCS;
            Assert.IsNotNull(createdAtActionResult);
            Assert.IsNotNull(returnedTransfer);
            Assert.AreEqual(1, returnedTransfer.Id);
        }
    }
}

