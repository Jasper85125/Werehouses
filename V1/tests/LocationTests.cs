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
            
            // Act
            var value = _locationController.GetAllLocations();
            
            // Assert
            var okResult = value.Result as OkObjectResult;
            var returnedItems = okResult.Value as IEnumerable<LocationCS>;
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
            
            // Act
            var value = _locationController.GetLocationById(1);
            
            // Assert
            var okResult = value.Result as OkObjectResult;
            var returnedItems = okResult.Value as LocationCS;
            Assert.IsNotNull(okResult);
            Assert.IsNotNull(okResult.Value);
            Assert.AreEqual(locations[0].code, returnedItems.code);
        }

        [TestMethod]
        public void GetLocationByIdTest_WrongId()
        {
            // Arrange
            _mockLocationService.Setup(service => service.GetLocationById(1)).Returns((LocationCS)null);
            
            // Act
            var value = _locationController.GetLocationById(1);
            
            // Assert
            Assert.IsInstanceOfType(value.Result, typeof(NotFoundResult));
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
            
            // Act
            var value = _locationController.GetLocationsByWarehouseId(1);
            
            // Assert
            var okResult = value.Result as OkObjectResult;
            var returnedItems = okResult.Value as IEnumerable<LocationCS>;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(2, returnedItems.Count());
        }

        [TestMethod]
        public void CreateLocationTest_Success()
        {
            // Arrange
            var createdLocation = new LocationCS { Id = 2, warehouse_id = 5, code = "C.3.2" };
            _mockLocationService.Setup(service => service.CreateLocation(createdLocation)).Returns(createdLocation);

            // Act
            var result = _locationController.CreateLocation(createdLocation);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(CreatedAtActionResult));
            var createdResult = result.Result as CreatedAtActionResult;
            Assert.IsNotNull(createdResult);
            Assert.IsInstanceOfType(createdResult.Value, typeof(LocationCS));
            var returnedLocation = createdResult.Value as LocationCS;
            Assert.AreEqual("C.3.2", returnedLocation.code);
            Assert.AreEqual(5, returnedLocation.warehouse_id);
        }

        [TestMethod]
        public void UpdatedLocationTest_Success()
        {
            // Arrange
            var updatedLocation = new LocationCS { Id = 1, warehouse_id = 5, code = "C.3.2"};
            _mockLocationService.Setup(service => service.UpdateLocation(updatedLocation, 1)).Returns(updatedLocation);

            // Act
            var result = _locationController.UpdateLocation(1, updatedLocation);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var createdResult = result.Result as OkObjectResult;
            Assert.IsNotNull(createdResult);
            Assert.IsInstanceOfType(createdResult.Value, typeof(LocationCS));
            var returnedLocation = createdResult.Value as LocationCS;
            Assert.AreEqual("C.3.2", returnedLocation.code);
            Assert.AreEqual(5, returnedLocation.warehouse_id);
        }

        [TestMethod]
        public void UpdatedLocationTest_Failed()
        {
            // Arrange
            var updatedLocation = new LocationCS { Id = 1, warehouse_id = 5, code = "C.3.2"};
            _mockLocationService.Setup(service => service.UpdateLocation(updatedLocation, 0)).Returns((LocationCS)null);

            // Act
            var result = _locationController.UpdateLocation(0, updatedLocation);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(BadRequestObjectResult));
            var createdResult = result.Result as BadRequestObjectResult;
            var returnedLocation = createdResult.Value as LocationCS;
            Assert.IsNull(returnedLocation);
        }
        [TestMethod]
        public void DeleteLocationTest_Exists()
        {
            // Arrange
            var location = new LocationCS { Id = 1, warehouse_id = 1, code = "B.2.1" };
            _mockLocationService.Setup(service => service.GetLocationById(1)).Returns(location);
            
            // Act
            var result = _locationController.DeleteLocation(1);
            
            // Assert
            Assert.IsInstanceOfType(result, typeof(OkResult));
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
        public void DeleteLocationService_Test()
        {
            var locationService = new LocationService();
            var location = locationService.GetLocationById(1);
            Assert.IsNotNull(location);

            locationService.DeleteLocation(1);
            location = locationService.GetLocationById(1);
            Assert.IsNull(location);
        }
    }
}

