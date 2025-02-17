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
    public class LocationTest
    {
        private Mock<ILocationService> _mockLocationService;
        private LocationController _locationController;

        [TestInitialize]
        public void Setup()
        {
            _mockLocationService = new Mock<ILocationService>();
            _locationController = new LocationController(_mockLocationService.Object);
            
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "../../data/locations.json");
            var location = new LocationCS { Id = 1, warehouse_id = 1, code = "B.2.1", name = "Row: B, Rack: 2, Shelf: 1", created_at = DateTime.Now, updated_at = DateTime.Now };

            var locationList = new List<LocationCS> { location };
            var json = JsonConvert.SerializeObject(locationList, Formatting.Indented);

            var directory = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            File.WriteAllText(filePath, json);
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
            httpContext.Items["UserRole"] = "Admin";
            httpContext.Items["WarehouseID"] = "1,2,3,4";

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

            httpContext.Items["UserRole"] = "Thief";
            _locationController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            //act
            var result = _locationController.GetAllLocations();

            //assert
            var unauthorizedResult = result.Result as UnauthorizedResult;
            Assert.IsNotNull(unauthorizedResult);
            Assert.AreEqual(401, unauthorizedResult.StatusCode);
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
            httpContext.Items["UserRole"] = "Admin"; 

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

            httpContext.Items["UserRole"] = "Thief";
            _locationController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            //act
            var result = _locationController.GetLocationById(1);

            //assert
            var unauthorizedResult = result.Result as UnauthorizedResult;
            Assert.IsNotNull(unauthorizedResult);
            Assert.AreEqual(401, unauthorizedResult.StatusCode);
        }

        [TestMethod]
        public void GetLocationByIdTest_WrongId()
        {
            // Arrange
            _mockLocationService.Setup(service => service.GetLocationById(1)).Returns((LocationCS)null);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";

            _locationController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var value = _locationController.GetLocationById(1);

            // Assert
            Assert.IsInstanceOfType(value.Result, typeof(NotFoundResult));

            httpContext.Items["UserRole"] = "Thief";
            _locationController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            //act
            var result = _locationController.GetLocationById(1);

            //assert
            var unauthorizedResult = result.Result as UnauthorizedResult;
            Assert.IsNotNull(unauthorizedResult);
            Assert.AreEqual(401, unauthorizedResult.StatusCode);
        }
  
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
            httpContext.Items["UserRole"] = "Admin";

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

            httpContext.Items["UserRole"] = "Thief";
            _locationController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            //act
            var result = _locationController.GetLocationsByWarehouseId(1);

            //assert
            var unauthorizedResult = result.Result as UnauthorizedResult;
            Assert.IsNotNull(unauthorizedResult);
            Assert.AreEqual(401, unauthorizedResult.StatusCode);
        }

        [TestMethod]
        public void CreateLocationTest_Success()
        {
            // Arrange
            var createdLocation = new LocationCS { Id = 2, warehouse_id = 5, code = "C.3.2" };

            _mockLocationService.Setup(service => service.CreateLocation(createdLocation)).Returns(createdLocation);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";

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

            httpContext.Items["UserRole"] = "Thief";
            _locationController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            //act
            result = _locationController.CreateLocation(createdLocation);

            //assert
            var unauthorizedResult = result.Result as UnauthorizedResult;
            Assert.IsNotNull(unauthorizedResult);
            Assert.AreEqual(401, unauthorizedResult.StatusCode);
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
            httpContext.Items["UserRole"] = "Admin";

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

            httpContext.Items["UserRole"] = "Thief";
            _locationController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            //act
            result = _locationController.CreateMultipleLocations(locations);

            //assert
            var unauthorizedResult = result.Result as UnauthorizedResult;
            Assert.IsNotNull(unauthorizedResult);
            Assert.AreEqual(401, unauthorizedResult.StatusCode);
        }

        [TestMethod]
        public void UpdatedLocationTest_Success()
        {
            // Arrange
            var updatedLocation = new LocationCS { Id = 1, warehouse_id = 5, code = "C.3.2" };

            _mockLocationService.Setup(service => service.UpdateLocation(updatedLocation, 1)).Returns(updatedLocation);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin"; 

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

            httpContext.Items["UserRole"] = "Thief";
            _locationController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            //act
            result = _locationController.UpdateLocation(1, updatedLocation);

            //assert
            var unauthorizedResult = result.Result as UnauthorizedResult;
            Assert.IsNotNull(unauthorizedResult);
            Assert.AreEqual(401, unauthorizedResult.StatusCode);
        }

        [TestMethod]
        public void UpdatedLocationTest_Failed()
        {
            // Arrange
            var updatedLocation = new LocationCS { Id = 1, warehouse_id = 5, code = "C.3.2" };

            _mockLocationService.Setup(service => service.UpdateLocation(updatedLocation, 0)).Returns((LocationCS)null);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";

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

            httpContext.Items["UserRole"] = "Thief";
            _locationController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            //act
            result = _locationController.UpdateLocation(1, updatedLocation);

            //assert
            var unauthorizedResult = result.Result as UnauthorizedResult;
            Assert.IsNotNull(unauthorizedResult);
            Assert.AreEqual(401, unauthorizedResult.StatusCode);
        }

        public void PatchLocation_Succes()
        {
            //Arrange
            var patchedlocation = new LocationCS() { Id = 1, name = "ASS" };
            _mockLocationService.Setup(_ => _.PatchLocation(1, "name", "ASS")).Returns(patchedlocation);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";

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

            httpContext.Items["UserRole"] = "Thief";
            _locationController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            //act
            result = _locationController.PatchLocation(1, "name","ASS");

            //assert
            var unauthorizedResult = result.Result as UnauthorizedResult;
            Assert.IsNotNull(unauthorizedResult);
            Assert.AreEqual(401, unauthorizedResult.StatusCode);
        }
        
        [TestMethod]
        public void DeleteLocationTest_Exists()
        {
            // Arrange
            var location = new LocationCS { Id = 1, warehouse_id = 1, code = "B.2.1" };
            _mockLocationService.Setup(service => service.GetLocationById(1)).Returns(location);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";

            _locationController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = _locationController.DeleteLocation(1);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkResult));

            httpContext.Items["UserRole"] = "Thief";
            _locationController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            //act
            result = _locationController.DeleteLocation(1);

            //assert
            var unauthorizedResult = result as UnauthorizedResult;
            Assert.IsNotNull(unauthorizedResult);
            Assert.AreEqual(401, unauthorizedResult.StatusCode);
        }

        [TestMethod]
        public void DeleteLocationsTest_Succes()
        {
            //Arrange
            var locationsToDelete = new List<int>() { 1, 2, 3 };

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";

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

            httpContext.Items["UserRole"] = "Thief";
            _locationController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            //act
            result = _locationController.DeleteLocations(locationsToDelete);

            //assert
            var unauthorizedResult = result as UnauthorizedResult;
            Assert.IsNotNull(unauthorizedResult);
            Assert.AreEqual(401, unauthorizedResult.StatusCode);
        }

        //testing the location service
        [TestMethod]
        public void GetAllLocationsService_Test()
        {
            var locationService = new LocationService();
            var locations = locationService.GetAllLocations();
            Assert.IsNotNull(locations);
            Assert.AreEqual(1, locations.Count);
        }

        [TestMethod]
        public void GetLocationByIdService_Test()
        {
            var locationService = new LocationService();
            var location = locationService.GetLocationById(1);
            Assert.IsNotNull(location);
            Assert.AreEqual(1, location.Id);
        }

        [TestMethod]
        public void GetLocationsByWarehouseIdService_Test()
        {
            var locationService = new LocationService();
            var locations = locationService.GetLocationsByWarehouseId(1);
            Assert.IsNotNull(locations);
            Assert.AreEqual(1, locations.Count);
        }

        [TestMethod]
        public void CreateLocationService_Test()
        {
            var locationService = new LocationService();
            var newLocation = new LocationCS { Id = 2, warehouse_id = 5, code = "C.3.2", name = "Row: C, Rack: 3, Shelf: 2", created_at = DateTime.Now, updated_at = DateTime.Now };
            var createdLocation = locationService.CreateLocation(newLocation);
            Assert.IsNotNull(createdLocation);
            Assert.AreEqual(5, createdLocation.warehouse_id);

            var locationsupdated = locationService.GetAllLocations();
            Assert.AreEqual(2, locationsupdated.Count);
        }

        [TestMethod]
        public void CreateMultipleLocatoinsService_test()
        {
            var locationService = new LocationService();
            var locations = new List<LocationCS>
            {
                new LocationCS { Id = 2, warehouse_id = 6, code = "C.3.2", name =  "Row: C, Rack: 3, Shelf: 2"},
                new LocationCS { Id = 3, warehouse_id = 7, code = "C.3.2", name =  "Row: C, Rack: 3, Shelf: 2"}
            };
            var createdLocations = locationService.CreateMultipleLocations(locations);
            Assert.IsNotNull(createdLocations);
            Assert.AreEqual(2, createdLocations.Count);

            var locationsupdated = locationService.GetAllLocations();
            Assert.AreEqual(3, locationsupdated.Count);
        }

        [TestMethod]
        public void UpdateLocationService_Test()
        {
            var locationService = new LocationService();
            var updatedLocation = new LocationCS { Id = 1, warehouse_id = 3, code = "C.3.2", name = "Row: C, Rack: 3, Shelf: 2", created_at = DateTime.Now, updated_at = DateTime.Now };
            var updatedLocationResult = locationService.UpdateLocation(updatedLocation, 1);
            Assert.IsNotNull(updatedLocationResult);
            Assert.AreEqual(3, updatedLocationResult.warehouse_id);
        }

        [TestMethod]
        public void UpdateLocationService_Failed()
        {
            var locationService = new LocationService();
            var updatedLocation = new LocationCS { Id = 1, warehouse_id = 3, code = "C.3.2", name = "Row: C, Rack: 3, Shelf: 2", created_at = DateTime.Now, updated_at = DateTime.Now };
            var updatedLocationResult = locationService.UpdateLocation(updatedLocation, 0);
            Assert.IsNull(updatedLocationResult);
        }

        [TestMethod]
        public void PatchLocationService_EmptyFail(){
            var LocationService = new LocationService();
            LocationService.DeleteLocation(1);
            var location = LocationService.PatchLocation(1, "name", "New Name");
            Assert.IsNull(location);
        }
        [TestMethod]
        public void DeleteLocationService_Test()
        {
            var locationService = new LocationService();
            var location = locationService.GetLocationById(1);
            Assert.IsNotNull(location);

            locationService.DeleteLocation(1);
            location = locationService.GetLocationById(1);
            Assert.IsNull(location);
        }

        [TestMethod]
        public void DeleteLocationService_Failed()
        {
            var locationService = new LocationService();
            var location = locationService.GetLocationById(0);
            Assert.IsNull(location);
        }

        [TestMethod]
        public void DeleteLocationsService_Test()
        {
            // create multiple locations for id 1, 2, 3
            var locationsToAdd = new List<LocationCS>{
                new LocationCS { Id = 2, warehouse_id = 1, code = "B.2.1", name = "Row: B, Rack: 2, Shelf: 1", created_at = DateTime.Now, updated_at = DateTime.Now },
                new LocationCS { Id = 3, warehouse_id = 1, code = "B.2.2", name = "Row: B, Rack: 2, Shelf: 2", created_at = DateTime.Now, updated_at = DateTime.Now },
            };
            var locationService = new LocationService();
            locationService.CreateMultipleLocations(locationsToAdd);
            var locations = locationService.GetAllLocations();
            Assert.IsNotNull(locations);
            Assert.AreEqual(3, locations.Count);

            locationService.DeleteLocations(new List<int> { 1,2,3 });
            locations = locationService.GetAllLocations();
            Assert.AreEqual(0, locations.Count);
        }

        [TestMethod]
        public void PatchLocationService_Test()
        {
            var locationService = new LocationService();
            var location = locationService.PatchLocation(1, "name", "New Name");
            location = locationService.PatchLocation(1, "code", "New Code");
            location = locationService.PatchLocation(1, "warehouse_id", 2);
            Assert.IsNotNull(location);
            Assert.AreEqual("New Name", location.name);
            Assert.AreEqual("New Code", location.code);
            Assert.AreEqual(2, location.warehouse_id);
        }

        [TestMethod]
        public void PatchLocationService_Failed()
        {
            var locationService = new LocationService();
            var location = locationService.PatchLocation(0, "name", "New Name");
            Assert.IsNull(location);
        }
    }
}
