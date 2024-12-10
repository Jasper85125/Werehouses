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
    public class LocationTest
    {
        private Mock<ILocationService> _mockLocationService;
        private LocationController _locationController;

        [TestInitialize]
        public void Setup()
        {
            _mockLocationService = new Mock<ILocationService>();
            _locationController = new LocationController(_mockLocationService.Object);
        }

        [TestMethod]
        public void GetLocationsTest_Exists()
        {
            // Arrange
            var locations = new List<LocationCS>
            {
                new LocationCS { Id = 1, warehouse_id = 1, code = "B.2.1" },
                new LocationCS { Id = 2, warehouse_id = 1, code = "C.3.2" }
            };
            _mockLocationService.Setup(service => service.GetAllLocations()).Returns(locations);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";  // Set the UserRole in HttpContext
            httpContext.Items["WarehouseID"] = 0;

            // Assign HttpContext to the controller
            _locationController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var value = _locationController.GetAllLocations();
            var okResult = value.Result as OkObjectResult;
            var returnedItems = okResult.Value as IEnumerable<LocationCS>;

            // Assert
            Assert.IsNotNull(okResult);
            Assert.AreEqual(2, returnedItems.Count());
        }

        [TestMethod]
        public void GetLocationByIdTest_Exists()
        {
            // Arrange
            var locations = new List<LocationCS>
            {
                new LocationCS { Id = 1, warehouse_id = 1, code = "B.2.1" },
                new LocationCS { Id = 2, warehouse_id = 1, code = "C.3.2" }
            };
            _mockLocationService.Setup(service => service.GetLocationById(1)).Returns(locations[0]);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";  // Set the UserRole in HttpContext

            // Assign HttpContext to the controller
            _locationController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var value = _locationController.GetLocationById(1);
            var okResult = value.Result as OkObjectResult;
            var returnedItems = okResult.Value as LocationCS;

            // Assert
            Assert.IsNotNull(okResult);
            Assert.IsNotNull(okResult.Value);
            Assert.AreEqual(locations[0].code, returnedItems.code);
        }

        [TestMethod]
        public void GetLocationByIdTest_WrongId()
        {
            // Arrange
            _mockLocationService.Setup(service => service.GetLocationById(1)).Returns((LocationCS)null);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";  // Set the UserRole in HttpContext

            // Assign HttpContext to the controller
            _locationController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var value = _locationController.GetLocationById(1);

            // Assert
            Assert.IsInstanceOfType(value.Result, typeof(NotFoundResult));
        }

        // GET: /locations/warehouse/{warehouse_id}
        //GetLocationsByWarehouseId  
        [TestMethod]
        public void GetLocationsByWarehouseIdTest_Exists()
        {
            // Arrange
            var locations = new List<LocationCS>
            {
                new LocationCS { Id = 1, warehouse_id = 1, code = "B.2.1" },
                new LocationCS { Id = 2, warehouse_id = 1, code = "C.3.2" }
            };
            _mockLocationService.Setup(service => service.GetLocationsByWarehouseId(1)).Returns(locations);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";  // Set the UserRole in HttpContext

            // Assign HttpContext to the controller
            _locationController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var value = _locationController.GetLocationsByWarehouseId(1);
            var okResult = value.Result as OkObjectResult;
            var returnedItems = okResult.Value as IEnumerable<LocationCS>;

            // Assert
            Assert.IsNotNull(okResult);
            Assert.AreEqual(2, returnedItems.Count());
        }

        [TestMethod]
        public void CreateLocationTest_Success()
        {
            // Arrange
            var createdLocation = new LocationCS { Id = 2, warehouse_id = 5, code = "C.3.2" };

            // Set up the mock service to return the created order
            _mockLocationService.Setup(service => service.CreateLocation(createdLocation)).Returns(createdLocation);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";  // Set the UserRole in HttpContext

            // Assign HttpContext to the controller
            _locationController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = _locationController.CreateLocation(createdLocation);
            var createdResult = result.Result as CreatedAtActionResult;
            var returnedLocation = createdResult.Value as LocationCS;

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(CreatedAtActionResult));
            Assert.IsNotNull(createdResult);
            Assert.IsInstanceOfType(createdResult.Value, typeof(LocationCS));
            Assert.AreEqual("C.3.2", returnedLocation.code);
            Assert.AreEqual(5, returnedLocation.warehouse_id);
        }

        [TestMethod]
        public void CreateMultipleLocations_ReturnsCreatedResult_WithNewLocations()
        {
            // Arrange
            var locations = new List<LocationCS>
            {
                new LocationCS { Id = 2, warehouse_id = 5, code = "C.3.2", name =  "Row: C, Rack: 3, Shelf: 2"},
                new LocationCS { Id = 2, warehouse_id = 5, code = "C.3.2", name =  "Row: C, Rack: 3, Shelf: 2"}
            };
            _mockLocationService.Setup(service => service.CreateMultipleLocations(locations)).Returns(locations);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";  // Set the UserRole in HttpContext

            // Assign HttpContext to the controller
            _locationController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = _locationController.CreateMultipleLocations(locations);
            var createdResult = result.Result as ObjectResult;
            var returnedItems = createdResult.Value as List<LocationCS>;
            var firstLocation = returnedItems[0];

            // Assert
            Assert.IsNotNull(createdResult);
            Assert.IsNotNull(returnedItems);
            Assert.AreEqual(locations[0].name, firstLocation.name);
            Assert.AreEqual(locations[0].code, firstLocation.code);
        }

        [TestMethod]
        public void UpdatedLocationTest_Success()
        {
            // Arrange
            var updatedLocation = new LocationCS { Id = 1, warehouse_id = 5, code = "C.3.2" };

            // Set up the mock service to return the created order
            _mockLocationService.Setup(service => service.UpdateLocation(updatedLocation, 1)).Returns(updatedLocation);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";  // Set the UserRole in HttpContext

            // Assign HttpContext to the controller
            _locationController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = _locationController.UpdateLocation(1, updatedLocation);
            var createdResult = result.Result as OkObjectResult;
            var returnedLocation = createdResult.Value as LocationCS;

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            Assert.IsNotNull(createdResult);
            Assert.IsInstanceOfType(createdResult.Value, typeof(LocationCS));
            Assert.AreEqual("C.3.2", returnedLocation.code);
            Assert.AreEqual(5, returnedLocation.warehouse_id);
        }

        [TestMethod]
        public void UpdatedLocationTest_Failed()
        {
            // Arrange
            var updatedLocation = new LocationCS { Id = 1, warehouse_id = 5, code = "C.3.2" };

            // Set up the mock service to return the created order
            _mockLocationService.Setup(service => service.UpdateLocation(updatedLocation, 0)).Returns((LocationCS)null);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";  // Set the UserRole in HttpContext

            // Assign HttpContext to the controller
            _locationController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = _locationController.UpdateLocation(0, updatedLocation);
            var createdResult = result.Result as BadRequestObjectResult;
            var returnedLocation = createdResult.Value as LocationCS;

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(BadRequestObjectResult));
            Assert.IsNull(returnedLocation);
        }
        public void PatchLocation_Succes()
        {
            //Arrange
            var patchedlocation = new LocationCS() { Id = 1, name = "ASS" };
            _mockLocationService.Setup(_ => _.PatchLocation(1, "name", "ASS")).Returns(patchedlocation);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";  // Set the UserRole in HttpContext

            // Assign HttpContext to the controller
            _locationController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            //Act
            var result = _locationController.PatchLocation(1, "name", "ASS");
            var resultok = result.Result as OkObjectResult;
            var value = resultok.Value as LocationCS;

            //Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(resultok);
            Assert.IsNotNull(value);
            Assert.AreEqual(resultok.StatusCode, 200);
            Assert.AreEqual(value.name, patchedlocation.name);
        }
        
        [TestMethod]
        public void DeleteLocationTest_Exists()
        {
            // Arrange
            var location = new LocationCS { Id = 1, warehouse_id = 1, code = "B.2.1" };
            _mockLocationService.Setup(service => service.GetLocationById(1)).Returns(location);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";  // Set the UserRole in HttpContext

            // Assign HttpContext to the controller
            _locationController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = _locationController.DeleteLocation(1);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkResult));
        }

        [TestMethod]
        public void DeleteLocationsTest_Succes()
        {
            //Arrange
            var locationsToDelete = new List<int>() { 1, 2, 3 };

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";  // Set the UserRole in HttpContext

            // Assign HttpContext to the controller
            _locationController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            //Act
            var result = _locationController.DeleteLocations(locationsToDelete);
            var resultok = result as OkObjectResult;

            //Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            Assert.AreEqual(resultok.StatusCode, 200);
        }
    }
}

