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

        [TestMethod]
        public void UpdatedTransferTest_Success()
        {
            // Arrange
            var updatedTransfer = new TransferCS { Id= 1, Reference= "X", transfer_from= 5050, transfer_to= 9292, transfer_status= "Completed",
                                                    Items = new List<ItemIdAndAmount> { new ItemIdAndAmount { item_id = "P007435", amount = 23 }}};

             _mockTransferService.Setup(service => service.UpdateTransfer(1, updatedTransfer)).Returns(updatedTransfer);

            // Act
            var result = _transferController.UpdateTransfer(1, updatedTransfer);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var createdResult = result.Result as OkObjectResult;
            Assert.IsNotNull(createdResult);
            Assert.IsInstanceOfType(createdResult.Value, typeof(TransferCS));
            var returnedTransfer = createdResult.Value as TransferCS;
            Assert.AreEqual(updatedTransfer.Reference, returnedTransfer.Reference);
            Assert.AreEqual(updatedTransfer.transfer_status, returnedTransfer.transfer_status);
        }

        [TestMethod]
        public void UpdatedTransferTest_Failed()
        {
            // Arrange
            var updatedTransfer = new TransferCS { Id= 1, Reference= "X", transfer_from= 5050, transfer_to= 9292, transfer_status= "Completed"};

             _mockTransferService.Setup(service => service.UpdateTransfer(0, updatedTransfer)).Returns((TransferCS)null);

            // Act
            var result = _transferController.UpdateTransfer(0, updatedTransfer);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(NotFoundObjectResult));
            var createdResult = result.Result as NotFoundObjectResult;
            var returnedTransfer = createdResult.Value as TransferCS;
            Assert.IsNull(returnedTransfer);
        }

        [TestMethod]
        public void DeleteTransferTest_Exist(){
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
            var transfers = new TransferCS { Id = 1, Reference = "JoJo", transfer_from = 9292, transfer_to = null, transfer_status = "completed", created_at = default, updated_at = default, Items = new List<ItemIdAndAmount> ()};
            _mockTransferService.Setup(service => service.GetTransferById(1)).Returns(transfers);

            // Act
            var result = _transferController.DeleteTransfer(1);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkResult));
        }
    }
}

