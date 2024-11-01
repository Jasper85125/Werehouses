using Microsoft.VisualStudio.TestTools.UnitTesting;
using Services;
using Moq;
using Controllers;
using System.Data.Common;
using Microsoft.AspNetCore.Mvc;

namespace Tests
{
    [TestClass]
    public class SuppliersTests
    {
        private Mock<ISupplierService> _mockSupplierService;
        private SupplierController _supplierController;

        [TestInitialize]
        public void Setup()
        {
            _mockSupplierService = new Mock<ISupplierService>();
            _supplierController = new SupplierController(_mockSupplierService.Object);
        }

        [TestMethod]
        public void GetSuppliersTest_Exists()
        {
            //arrange
            var suppliers = new List<SupplierCS>
            {
                new SupplierCS { Id = 1, Code = "5KR3T", Name = "Jonathan", Address = "Smokey 404"},
                new SupplierCS { Id = 2, Code = "H1M12", Name = "Joseph", Address = "Lissabon 402"}
            };
            _mockSupplierService.Setup(service => service.GetAllSuppliers()).Returns(suppliers);
            
            //Act
            var value = _supplierController.GetAllSuppliers();
            
            //Assert
            var okResult = value.Result as OkObjectResult;
            var returnedItems = okResult.Value as IEnumerable<SupplierCS>;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(2, returnedItems.Count());
        }

        [TestMethod]
        public void GetSupplierByIdTest_Exists()
        {
            //arrange
            var suppliers = new List<SupplierCS>
            {
                new SupplierCS { Id = 1, Code = "5KR3T", Name = "Jonathan", Address = "Smokey 404"},
                new SupplierCS { Id = 2, Code = "H1M12", Name = "Joseph", Address = "Lissabon 402"}
            };
            _mockSupplierService.Setup(service => service.GetSupplierById(1)).Returns(suppliers[0]);
            
            //Act
            var value = _supplierController.GetSupplierById(1);
            
            //Assert
            var okResult = value.Result as OkObjectResult;
            var returnedItems = okResult.Value as SupplierCS;
            Assert.IsNotNull(okResult);
            Assert.IsNotNull(okResult.Value);
            Assert.AreEqual(suppliers[0].Code, returnedItems.Code);
        }

        [TestMethod]
        public void GetSupplierByIdTest_WrongId()
        {
            //arrange
            _mockSupplierService.Setup(service => service.GetSupplierById(1)).Returns((SupplierCS)null);
            
            //Act
            var value = _supplierController.GetSupplierById(1);
            
            //Assert
            Assert.IsInstanceOfType(value.Result, typeof(NotFoundResult));
        }

        [TestMethod]
        public void CreateSupplierTest_Success()
        {
            // Arrange
            var newSupplier = new SupplierCS { Id = 1, Code = "5KR3T", Name = "Jonathan", Address = "Smokey 404"};
            var createdSupplier = new SupplierCS { Id = 2, Code = "H1M12", Name = "Joseph", Address = "Lissabon 402"};

            // Set up the mock service to return the created order
            _mockSupplierService.Setup(service => service.CreateSupplier(newSupplier)).Returns(createdSupplier);

            // Act
            var result = _supplierController.CreateSupplier(newSupplier);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(CreatedAtActionResult));
            var createdResult = result.Result as CreatedAtActionResult;
            Assert.IsNotNull(createdResult);
            Assert.IsInstanceOfType(createdResult.Value, typeof(SupplierCS));
            var returnedSupplier = createdResult.Value as SupplierCS;
            Assert.AreEqual("H1M12", returnedSupplier.Code);
            Assert.AreEqual("Joseph", returnedSupplier.Name);
            Assert.AreEqual("Lissabon 402", returnedSupplier.Address);
        }

        [TestMethod]
        public void UpdatedSupplierTest_Success()
        {
            // Arrange
             var updatedSupplier = new SupplierCS { Id= 1, Code= "SUP0373", Name= "Supp & liers", Address= "Wall Street 181", address_extra = "Apt. 6996", City= "Houston", zip_code = "4002 AZ",  Province= "Texas",
                                                    Country= "USA", contact_name = "Fem Keijzer", PhoneNumber = "(078) 0013363", Reference = "LPaJ-SUP0001"};

             _mockSupplierService.Setup(service => service.UpdateSupplier(1, updatedSupplier)).Returns(updatedSupplier);

            // Act
            var result = _supplierController.UpdateSupplier(1, updatedSupplier);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var createdResult = result.Result as OkObjectResult;
            Assert.IsNotNull(createdResult);
            Assert.IsInstanceOfType(createdResult.Value, typeof(SupplierCS));
            var returnedSupplier = createdResult.Value as SupplierCS;
            Assert.AreEqual(updatedSupplier.Code, returnedSupplier.Code);
            Assert.AreEqual(updatedSupplier.Address, returnedSupplier.Address);
            Assert.AreEqual(updatedSupplier.contact_name, returnedSupplier.contact_name);
            Assert.AreEqual(updatedSupplier.PhoneNumber, returnedSupplier.PhoneNumber);
        }

        [TestMethod]
        public void UpdatedSupplierTest_Failed()
        {
            // Arrange
            var updatedSupplier = new SupplierCS { Id= 1, Code= "SUP0373", Name= "Supp & liers", Address= "Wall Street 181", address_extra = "Apt. 6996", City= "Houston", zip_code = "4002 AZ",  Province= "Texas",
                                                    Country= "USA", contact_name = "Fem Keijzer", PhoneNumber = "(078) 0013363", Reference = "LPaJ-SUP0001"};

             _mockSupplierService.Setup(service => service.UpdateSupplier(0, updatedSupplier)).Returns((SupplierCS)null);

            // Act
            var result = _supplierController.UpdateSupplier(0, updatedSupplier);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(BadRequestObjectResult));
            var createdResult = result.Result as BadRequestObjectResult;
            var returnedSupplier = createdResult.Value as SupplierCS;
            Assert.IsNull(returnedSupplier);
        }
    }
}

