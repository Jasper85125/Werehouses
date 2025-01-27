using Microsoft.VisualStudio.TestTools.UnitTesting;
using ServicesV2;
using Moq;
using ControllersV2;
using System.Data.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace TestsV2
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
            var supplier = new SupplierCS { 
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

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";

            _supplierController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            //Act
            var value = _supplierController.GetAllSuppliers();

            //Assert
            var okResult = value.Result as OkObjectResult;
            var returnedItems = okResult.Value as IEnumerable<SupplierCS>;
            Assert.IsNotNull(okResult);
            Assert.IsNotNull(returnedItems);
            Assert.AreEqual(2, returnedItems.Count());

            httpContext.Items["UserRole"] = "Bee";
            _supplierController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            //act
            var result = _supplierController.GetAllSuppliers();

            //assert
            var unauthorizedResult = result.Result as UnauthorizedResult;
            Assert.IsNotNull(unauthorizedResult);
            Assert.AreEqual(401, unauthorizedResult.StatusCode);
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

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";

            _supplierController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            //Act
            var value = _supplierController.GetSupplierById(1);

            //Assert
            var okResult = value.Result as OkObjectResult;
            var returnedItems = okResult.Value as SupplierCS;
            Assert.IsNotNull(okResult);
            Assert.IsNotNull(okResult.Value);
            Assert.AreEqual(suppliers[0].Code, returnedItems.Code);

            httpContext.Items["UserRole"] = "Bee";
            _supplierController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            //act
            var result = _supplierController.GetSupplierById(1);

            //assert
            var unauthorizedResult = result.Result as UnauthorizedResult;
            Assert.IsNotNull(unauthorizedResult);
            Assert.AreEqual(401, unauthorizedResult.StatusCode);
        }

        [TestMethod]
        public void GetSupplierByIdTest_WrongId()
        {
            //arrange
            _mockSupplierService.Setup(service => service.GetSupplierById(1)).Returns((SupplierCS)null);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";

            _supplierController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            //Act
            var value = _supplierController.GetSupplierById(1);

            //Assert
            Assert.IsInstanceOfType(value.Result, typeof(NotFoundResult));

            httpContext.Items["UserRole"] = "Bee";
            _supplierController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            //act
            var result = _supplierController.GetSupplierById(1);

            //assert
            var unauthorizedResult = result.Result as UnauthorizedResult;
            Assert.IsNotNull(unauthorizedResult);
            Assert.AreEqual(401, unauthorizedResult.StatusCode);
        }

        [TestMethod]
        public void CreateSupplierTest_Success()
        {
            // Arrange
            var newSupplier = new SupplierCS { Id = 1, Code = "5KR3T", Name = "Jonathan", Address = "Smokey 404" };
            var createdSupplier = new SupplierCS { Id = 2, Code = "H1M12", Name = "Joseph", Address = "Lissabon 402" };

            _mockSupplierService.Setup(service => service.CreateSupplier(newSupplier)).Returns(createdSupplier);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";

            _supplierController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

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

            httpContext.Items["UserRole"] = "Bee";
            _supplierController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            //act
            result = _supplierController.CreateSupplier(newSupplier);

            //assert
            var unauthorizedResult = result.Result as UnauthorizedResult;
            Assert.IsNotNull(unauthorizedResult);
            Assert.AreEqual(401, unauthorizedResult.StatusCode);
        }

        [TestMethod]
        public void CreateMultipleSuppliers_ReturnsCreatedResult_WithNewSuppliers()
        {
            // Arrange
            var suppliers = new List<SupplierCS>
            {
                new SupplierCS { Id = 1, Code = "5KR3T", Name = "Jonathan", Address = "Smokey 404", City = "Toronto", zip_code = "2935BK",
                                 Country = "Wano", Province = "Denver", contact_name = "Jeff Bezos", PhoneNumber = "0794327812"},
                new SupplierCS { Id = 2, Code = "5KR3T", Name = "Jonathan", Address = "Smokey 404", City = "Toronto", zip_code = "2935BK",
                                 Country = "Wano", Province = "Denver", contact_name = "Jeff Bezos", PhoneNumber = "0794327812"}
            };
            _mockSupplierService.Setup(service => service.CreateMultipleSuppliers(suppliers)).Returns(suppliers);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin"; 

            _supplierController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = _supplierController.CreateMultipleSuppliers(suppliers);
            var createdResult = result.Result as ObjectResult;
            var returnedItems = createdResult.Value as List<SupplierCS>;
            var firstSupplier = returnedItems[0];

            // Assert
            Assert.IsNotNull(createdResult);
            Assert.IsNotNull(returnedItems);
            Assert.AreEqual(suppliers[0].Name, firstSupplier.Name);
            Assert.AreEqual(suppliers[0].Code, firstSupplier.Code);

            httpContext.Items["UserRole"] = "Bee";
            _supplierController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            //act
            result = _supplierController.CreateMultipleSuppliers(suppliers);

            //assert
            var unauthorizedResult = result.Result as UnauthorizedResult;
            Assert.IsNotNull(unauthorizedResult);
            Assert.AreEqual(401, unauthorizedResult.StatusCode);
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

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";  

            _supplierController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = _supplierController.UpdateSupplier(1, updatedSupplier);
            var createdResult = result.Result as OkObjectResult;
            var returnedSupplier = createdResult.Value as SupplierCS;

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            Assert.IsNotNull(createdResult);
            Assert.IsInstanceOfType(createdResult.Value, typeof(SupplierCS));
            Assert.AreEqual(updatedSupplier.Code, returnedSupplier.Code);
            Assert.AreEqual(updatedSupplier.Address, returnedSupplier.Address);
            Assert.AreEqual(updatedSupplier.contact_name, returnedSupplier.contact_name);
            Assert.AreEqual(updatedSupplier.PhoneNumber, returnedSupplier.PhoneNumber);

            httpContext.Items["UserRole"] = "Bee";
            _supplierController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            //act
            result = _supplierController.UpdateSupplier(1, updatedSupplier);

            //assert
            var unauthorizedResult = result.Result as UnauthorizedResult;
            Assert.IsNotNull(unauthorizedResult);
            Assert.AreEqual(401, unauthorizedResult.StatusCode);
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

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";  

            _supplierController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = _supplierController.UpdateSupplier(0, updatedSupplier);
            var createdResult = result.Result as BadRequestObjectResult;
            var returnedSupplier = createdResult.Value as SupplierCS;

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(BadRequestObjectResult));
            Assert.IsNull(returnedSupplier);

            httpContext.Items["UserRole"] = "Bee";
            _supplierController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            //act
            result = _supplierController.UpdateSupplier(1, updatedSupplier);

            //assert
            var unauthorizedResult = result.Result as UnauthorizedResult;
            Assert.IsNotNull(unauthorizedResult);
            Assert.AreEqual(401, unauthorizedResult.StatusCode);
        }

        [TestMethod]
        public void DeleteSupplierTest_Success()
        {
            // Arrange
            var supplier = new SupplierCS { Id = 1, Code = "5KR3T", Name = "Jonathan", Address = "Smokey 404" };
            _mockSupplierService.Setup(service => service.GetSupplierById(1)).Returns(supplier);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";

            _supplierController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = _supplierController.DeleteSupplier(1);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkResult));

            httpContext.Items["UserRole"] = "Bee";
            _supplierController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            //act
            result = _supplierController.DeleteSupplier(1);

            //assert
            var unauthorizedResult = result as UnauthorizedResult;
            Assert.IsNotNull(unauthorizedResult);
            Assert.AreEqual(401, unauthorizedResult.StatusCode);
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

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";

            _supplierController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = _supplierController.GetItemsBySupplierId(1);
            var okResult = result.Result as OkObjectResult;
            var returnedItems = okResult.Value as IEnumerable<ItemCS>;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(okResult, "Expected OkObjectResult, but got null.");
            Assert.IsNotNull(returnedItems, "Expected returnedItems to be non-null.");
            Assert.AreEqual(2, returnedItems.Count(), "Expected returnedItems to contain 2 items.");

            httpContext.Items["UserRole"] = "Bee";
            _supplierController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            //act
            result = _supplierController.GetItemsBySupplierId(1);

            //assert
            var unauthorizedResult = result.Result as UnauthorizedResult;
            Assert.IsNotNull(unauthorizedResult);
            Assert.AreEqual(401, unauthorizedResult.StatusCode);
        }

        [TestMethod]
        public void GetItemsBySupplierIdTest_WrongId()
        {
            // Arrange
            _mockSupplierService.Setup(service => service.GetItemsBySupplierId(1)).Returns((List<ItemCS>)null);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";

            _supplierController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = _supplierController.GetItemsBySupplierId(1);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));

            httpContext.Items["UserRole"] = "Bee";
            _supplierController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            //act
            result = _supplierController.GetItemsBySupplierId(1);

            //assert
            var unauthorizedResult = result.Result as UnauthorizedResult;
            Assert.IsNotNull(unauthorizedResult);
            Assert.AreEqual(401, unauthorizedResult.StatusCode);
        }

        [TestMethod]
        public void PatchSupplierTest_Success()
        {
            // Arrange
            var patchSupplier = new SupplierCS
            {
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

            var existingSupplier = new SupplierCS
            {
                Code = "SUP0373",
                Name = "dag & nacht",
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

            _mockSupplierService.Setup(service => service.GetSupplierById(1)).Returns(existingSupplier);
            _mockSupplierService.Setup(service => service.PatchSupplier(1, "Name", "Supp & liers")).Returns(patchSupplier);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";

            _supplierController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = _supplierController.PatchSupplier(1, "Name", "Supp & liers");
            var okResult = result.Result as OkObjectResult;
            var returnedSupplier = okResult.Value as SupplierCS;

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            Assert.IsNotNull(okResult);
            Assert.IsInstanceOfType(okResult.Value, typeof(SupplierCS));
            Assert.AreEqual(patchSupplier.Code, returnedSupplier.Code);
            Assert.AreEqual(patchSupplier.Address, returnedSupplier.Address);
            Assert.AreEqual(patchSupplier.contact_name, returnedSupplier.contact_name);
            Assert.AreEqual(patchSupplier.PhoneNumber, returnedSupplier.PhoneNumber);

            httpContext.Items["UserRole"] = "Bee";
            _supplierController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            //act
            result = _supplierController.PatchSupplier(1, "Name", "Supp & liers");

            //assert
            var unauthorizedResult = result.Result as UnauthorizedResult;
            Assert.IsNotNull(unauthorizedResult);
            Assert.AreEqual(401, unauthorizedResult.StatusCode);
        }

        [TestMethod]
        public void PatchSupplierTest_Failed()
        {
            // Arrange
            var patchSupplier = new SupplierCS
            {
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

            _mockSupplierService.Setup(service => service.GetSupplierById(1)).Returns((SupplierCS)null);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";

            _supplierController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = _supplierController.PatchSupplier(1, "Name", "Supp & liers");

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));

            httpContext.Items["UserRole"] = "Bee";
            _supplierController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            //act
            result = _supplierController.PatchSupplier(1, "Name", "Supp & liers");

            //assert
            var unauthorizedResult = result.Result as UnauthorizedResult;
            Assert.IsNotNull(unauthorizedResult);
            Assert.AreEqual(401, unauthorizedResult.StatusCode);
        }
        [TestMethod]
        public void DeleteSuppliersTest_Succes()
        {
            //Arrange
            var suppliersToDelete = new List<int>() { 1, 2, 3 };

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";

             _supplierController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            //Act
            var result = _supplierController.DeleteSuppliers(suppliersToDelete);
            var resultok = result as OkObjectResult;

            //Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            Assert.AreEqual(resultok.StatusCode, 200);

            httpContext.Items["UserRole"] = "Bee";
            _supplierController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            //act
            result = _supplierController.DeleteSuppliers(suppliersToDelete);

            //assert
            var unauthorizedResult = result as UnauthorizedResult;
            Assert.IsNotNull(unauthorizedResult);
            Assert.AreEqual(401, unauthorizedResult.StatusCode);
        }

        // Test for the services
        [TestMethod]
        public void GetAllSuppliersService_Test(){

            var supplierService = new SupplierService();
            var suppliers = supplierService.GetAllSuppliers();
            Assert.IsNotNull(suppliers);
            Assert.AreEqual(1, suppliers.Count);
        }

        [TestMethod]
        public void GetSupplierByIdService_Test(){

            var supplierService = new SupplierService();
            var supplier = supplierService.GetSupplierById(1);
            Assert.IsNotNull(supplier);
            Assert.AreEqual("Lee, Parks and Johnson", supplier.Name);
        }

        [TestMethod]
        public void CreateSupplierService_Test(){

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
        public void CreateMultipleSuppliersService_Test(){
            var supplierService = new SupplierService();
            var suppliers = new List<SupplierCS>
            {
                new SupplierCS
                {
                    Id = 3,
                    Code = "SUP0003",
                    Name = "Daniel Inc",
                    Address = "1296 Daniel Road Apt. 349",
                    address_extra = "Apt. 349",
                    City = "Pierceview",
                    zip_code = "28301",
                    Province = "Colorado",
                    Country = "United States",
                    contact_name = "Bryan Clark",
                    PhoneNumber = "242.732.3483x2573x2573",
                    Reference = "DInc-SUP0003",
                    created_at = DateTime.Now,
                    updated_at = DateTime.Now
                },
                new SupplierCS
                {
                    Id = 4,
                    Code = "SUP0004",
                    Name = "Daniel Inc",
                    Address = "1296 Daniel Road Apt. 349",
                    address_extra = "Apt. 349",
                    City = "Pierceview",
                    zip_code = "28301",
                    Province = "Colorado",
                    Country = "United States",
                    contact_name = "Bryan Clark",
                    PhoneNumber = "242.732.3483x2573x2573",
                    Reference = "DInc-SUP0004",
                    created_at = DateTime.Now,
                    updated_at = DateTime.Now
                }
            };
            var suppliersAdded = supplierService.CreateMultipleSuppliers(suppliers);
            Assert.IsNotNull(suppliersAdded);
            Assert.AreEqual(2, suppliersAdded.Count);

            var suppliersUpdated = supplierService.GetAllSuppliers();
            Assert.AreEqual(3, suppliersUpdated.Count);
        }

        [TestMethod]
        public void UpdateSupplierService_Test(){
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
        public void UpdateSupplierService_FailTest(){
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
            var supplierUpdated = supplierService.UpdateSupplier(2, supplier);
            Assert.IsNull(supplierUpdated);
        }  

        [TestMethod]
        public void DeleteSupplierService_Test(){
            var supplierService = new SupplierService();
            supplierService.DeleteSupplier(1);
            var suppliers = supplierService.GetAllSuppliers();
            Assert.AreEqual(0, suppliers.Count);
        }

        [TestMethod]
        public void DeleteSupplierService_FailTest(){
            var supplierService = new SupplierService();
            supplierService.DeleteSupplier(2);
            var suppliers = supplierService.GetAllSuppliers();
            Assert.AreEqual(1, suppliers.Count);
        } 

        [TestMethod]
        public void GetItemsBySupplierIdService_Test(){
            var supplierService = new SupplierService();
            var items = supplierService.GetItemsBySupplierId(1);
            Assert.IsNotNull(items);
            Assert.AreEqual(0, items.Count);
        }

        [TestMethod]
        public void PatchSupplierService_Test(){
 
            var supplierService = new SupplierService();
            var suppliers = supplierService.PatchSupplier(1, "Name", "New Name");
            suppliers = supplierService.PatchSupplier(1, "Code", "New Code");
            suppliers = supplierService.PatchSupplier(1, "Address", "New Address");
            suppliers = supplierService.PatchSupplier(1, "address_extra", "New Address Extra");
            suppliers = supplierService.PatchSupplier(1, "City", "New City");
            suppliers = supplierService.PatchSupplier(1, "zip_code", "New Zip Code");
            suppliers = supplierService.PatchSupplier(1, "Province", "New Province");
            suppliers = supplierService.PatchSupplier(1, "Country", "New Country");
            suppliers = supplierService.PatchSupplier(1, "contact_name", "New Contact Name");
            suppliers = supplierService.PatchSupplier(1, "PhoneNumber", "New Phone Number");
            suppliers = supplierService.PatchSupplier(1, "Reference", "New Reference");
            Assert.IsNotNull(suppliers);
            Assert.AreEqual("New Name", suppliers.Name);
            Assert.AreEqual("New Code", suppliers.Code);
            Assert.AreEqual("New Address", suppliers.Address);
            Assert.AreEqual("New Address Extra", suppliers.address_extra);
            Assert.AreEqual("New City", suppliers.City);
            Assert.AreEqual("New Zip Code", suppliers.zip_code);
            Assert.AreEqual("New Province", suppliers.Province);
            Assert.AreEqual("New Country", suppliers.Country);
            Assert.AreEqual("New Contact Name", suppliers.contact_name);
            Assert.AreEqual("New Phone Number", suppliers.PhoneNumber);
            Assert.AreEqual("New Reference", suppliers.Reference);
        }

        [TestMethod]
        public void PatchSupplierService_FailTest(){
            var supplierService = new SupplierService();
            var suppliers = supplierService.PatchSupplier(2, "Name", "New Name");
            Assert.IsNull(suppliers);
        }

        [TestMethod]
        public void DeleteSuppliersService_Test(){
            //create and add a list first to delete
            var suppliers = new List<SupplierCS>
            {
                new SupplierCS
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
                },
                new SupplierCS
                {
                    Id = 3,
                    Code = "SUP0003",
                    Name = "Another Supplier",
                    Address = "1234 Another St",
                    address_extra = "Suite 100",
                    City = "Another City",
                    zip_code = "12345",
                    Province = "Another Province",
                    Country = "Another Country",
                    contact_name = "Another Contact",
                    PhoneNumber = "123-456-7890",
                    Reference = "ASUP-SUP0003",
                    created_at = DateTime.Now,
                    updated_at = DateTime.Now
                }
            };
            var supplierService = new SupplierService();
            supplierService.CreateMultipleSuppliers(suppliers);
            var suppliersToDelete = new List<int>() { 1, 2, 3 };
            supplierService.DeleteSuppliers(suppliersToDelete);
            var totalSuppliers = supplierService.GetAllSuppliers();
            Assert.AreEqual(0, totalSuppliers.Count);
        }
    }
}


