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
    public class SuppliersTests
    {
        private Mock<ISupplierService> _mockSupplierService;
        private SupplierController _supplierController;

        [TestInitialize]
        public void Setup()
        {
            _mockSupplierService = new Mock<ISupplierService>();
            _supplierController = new SupplierController(_mockSupplierService.Object);

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "../../data/suppliers.json");
            var supplier = new SupplierCS
            {
                Id = 1,
                Code = "SUP0001",
                Name = "Lee, Parks and Johnson",
                Address = "5989 Sullivan Drives",
                address_extra = "Apt. 996",
                City = "Port Anitaburgh",
                zip_code = "91688",
                Province = "Illinois",
                Country = "Czech Republic",
                contact_name = "Toni Barnett",
                PhoneNumber = "363.541.7282x36825",
                Reference = "LPaJ-SUP0001",
                created_at = DateTime.Now,
                updated_at = DateTime.Now
            };

            var supplierlist = new List<SupplierCS> { supplier };
            var json = JsonConvert.SerializeObject(supplierlist, Formatting.Indented);

            // Create directory if it does not exist
            var directory = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            // Write the JSON data to the file
            File.WriteAllText(filePath, json);
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
            Assert.IsNotNull(returnedItems);
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
            var newSupplier = new SupplierCS { Id = 1, Code = "5KR3T", Name = "Jonathan", Address = "Smokey 404" };
            var createdSupplier = new SupplierCS { Id = 2, Code = "H1M12", Name = "Joseph", Address = "Lissabon 402" };
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
            var updatedSupplier = new SupplierCS
            {
                Id = 1,
                Code = "SUP0373",
                Name = "Supp & liers",
                Address = "Wall Street 181",
                address_extra = "Apt. 6996",
                City = "Houston",
                zip_code = "4002 AZ",
                Province = "Texas",
                Country = "USA",
                contact_name = "Fem Keijzer",
                PhoneNumber = "(078) 0013363",
                Reference = "LPaJ-SUP0001"
            };

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
            var updatedSupplier = new SupplierCS
            {
                Id = 1,
                Code = "SUP0373",
                Name = "Supp & liers",
                Address = "Wall Street 181",
                address_extra = "Apt. 6996",
                City = "Houston",
                zip_code = "4002 AZ",
                Province = "Texas",
                Country = "USA",
                contact_name = "Fem Keijzer",
                PhoneNumber = "(078) 0013363",
                Reference = "LPaJ-SUP0001"
            };

            _mockSupplierService.Setup(service => service.UpdateSupplier(0, updatedSupplier)).Returns((SupplierCS)null);

            // Act
            var result = _supplierController.UpdateSupplier(0, updatedSupplier);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(BadRequestObjectResult));
            var createdResult = result.Result as BadRequestObjectResult;
            var returnedSupplier = createdResult.Value as SupplierCS;
            Assert.IsNull(returnedSupplier);
        }

        [TestMethod]
        public void DeleteSupplierTest_Success()
        {
            // Arrange
            var supplier = new SupplierCS { Id = 1, Code = "5KR3T", Name = "Jonathan", Address = "Smokey 404" };
            _mockSupplierService.Setup(service => service.GetSupplierById(1)).Returns(supplier);

            // Act
            var result = _supplierController.DeleteSupplier(1);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkResult));
        }

        [TestMethod]
        public void GetItemsBySupplierId_ExistingId()
        {
            // Arrange
            var supplier = new SupplierCS { Id = 1 };
            _mockSupplierService.Setup(service => service.GetSupplierById(1)).Returns(supplier);

            var items = new List<ItemCS>
            {
                new ItemCS { uid = "U001", code = "Item1", supplier_id = 1 },
                new ItemCS { uid = "U002", code = "Item2", supplier_id = 1 }
            };
            _mockSupplierService.Setup(service => service.GetItemsBySupplierId(1)).Returns(items);

            // Act
            var result = _supplierController.GetItemsBySupplierId(1);

            // Assert
            Assert.IsNotNull(result);
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult, "Expected OkObjectResult, but got null.");

            var returnedItems = okResult.Value as IEnumerable<ItemCS>;
            Assert.IsNotNull(returnedItems, "Expected returnedItems to be non-null.");
            Assert.AreEqual(2, returnedItems.Count(), "Expected returnedItems to contain 2 items.");
        }

        [TestMethod]
        public void GetItemsBySupplierIdTest_WrongId()
        {
            // Arrange
            _mockSupplierService.Setup(service => service.GetItemsBySupplierId(1)).Returns((List<ItemCS>)null);

            // Act
            var result = _supplierController.GetItemsBySupplierId(1);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));
        }

        [TestMethod]
        public void GetAllSuppliersService_Test()
        {

            var supplierService = new SupplierService();
            var suppliers = supplierService.GetAllSuppliers();
            Assert.IsNotNull(suppliers);
            Assert.AreEqual(1, suppliers.Count);
        }

        [TestMethod]
        public void GetSupplierByIdService_Test()
        {

            var supplierService = new SupplierService();
            var supplier = supplierService.GetSupplierById(1);
            Assert.IsNotNull(supplier);
            Assert.AreEqual("Lee, Parks and Johnson", supplier.Name);
        }

        [TestMethod]
        public void CreateSupplierService_Test()
        {

            var supplierService = new SupplierService();
            var supplier = new SupplierCS
            {
                Id = 2,
                Code = "SUP0002",
                Name = "Daniel Inc",
                Address = "1296 Daniel Road Apt. 349",
                address_extra = "Apt. 349",
                City = "Pierceview",
                zip_code = "28301",
                Province = "Colorado",
                Country = "United States",
                contact_name = "Bryan Clark",
                PhoneNumber = "242.732.3483x2573x2573",
                Reference = "DInc-SUP0002",
                created_at = DateTime.Now,
                updated_at = DateTime.Now
            };
            var suppliers = supplierService.CreateSupplier(supplier);
            Assert.IsNotNull(suppliers);
            Assert.AreEqual("Daniel Inc", suppliers.Name);

            var suppliersUpdated = supplierService.GetAllSuppliers();
            Assert.AreEqual(2, suppliersUpdated.Count);
        }

        [TestMethod]
        public void UpdateSupplierService_Test()
        {
            var supplierService = new SupplierService();
            var supplier = new SupplierCS
            {
                Id = 1,
                Code = "SUP0001",
                Name = "Lee, Par",
                Address = "5989 Sullivan Drives",
                address_extra = "Apt. 996",
                City = "Port Anitaburgh",
                zip_code = "91688",
                Province = "Illinois",
                Country = "Czech Republic",
                contact_name = "Toni Barnett",
                PhoneNumber = "363.541.7282x36825",
                Reference = "LPaJ-SUP0001",
                created_at = DateTime.Now,
                updated_at = DateTime.Now
            };
            var supplierUpdated = supplierService.UpdateSupplier(1, supplier);
            Assert.IsNotNull(supplierUpdated);
            Assert.AreEqual("Lee, Par", supplierUpdated.Name);
        }

        [TestMethod]
        public void DeleteSupplierService_Test()
        {
            var supplierService = new SupplierService();
            supplierService.DeleteSupplier(1);
            var suppliers = supplierService.GetAllSuppliers();
            Assert.AreEqual(0, suppliers.Count);
        }

        [TestMethod]
        public void DeleteSupplierService_FailTest()
        {
            var supplierService = new SupplierService();
            supplierService.DeleteSupplier(2);
            var suppliers = supplierService.GetAllSuppliers();
            Assert.AreEqual(1, suppliers.Count);
        }

        [TestMethod]
        public void GetItemsBySupplierIdService_Test()
        {
            var supplierService = new SupplierService();
            var items = supplierService.GetItemsBySupplierId(1);
            Assert.IsNotNull(items);
            Assert.AreEqual(0, items.Count);
        }

        

        [TestCleanup]
        public void Cleanup()
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "../../data/suppliers.json");
            File.Delete(filePath);
        }
    }
}


