using Microsoft.VisualStudio.TestTools.UnitTesting;
using Services;
using Moq;
using Controllers;
using System.Data.Common;
using Microsoft.AspNetCore.Mvc;

namespace Tests
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
            //arrange
            var locations = new List<LocationCS>
            {
                new LocationCS { Id = 1, warehouse_id = 1, code = "B.2.1" },
                new LocationCS { Id = 2, warehouse_id = 1, code = "C.3.2" }
            };
            _mockLocationService.Setup(service => service.GetAllLocations()).Returns(locations);
            
            //Act
            var value = _locationController.GetAllLocations();
            
            //Assert
            var okResult = value.Result as OkObjectResult;
            var returnedItems = okResult.Value as IEnumerable<LocationCS>;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(2, returnedItems.Count());
        }

        [TestMethod]
        public void GetLocationByIdTest_Exists()
        {
            //arrange
            var locations = new List<LocationCS>
            {
                new LocationCS { Id = 1, warehouse_id = 1, code = "B.2.1" },
                new LocationCS { Id = 2, warehouse_id = 1, code = "C.3.2" }
            };
            _mockLocationService.Setup(service => service.GetLocationById(1)).Returns(locations[0]);
            
            //Act
            var value = _locationController.GetLocationById(1);
            
            //Assert
            var okResult = value.Result as OkObjectResult;
            var returnedItems = okResult.Value as LocationCS;
            Assert.IsNotNull(okResult);
            Assert.IsNotNull(okResult.Value);
            Assert.AreEqual(locations[0].code, returnedItems.code);
        }

        [TestMethod]
        public void GetLocationByIdTest_WrongId()
        {
            //arrange
            _mockLocationService.Setup(service => service.GetLocationById(1)).Returns((LocationCS)null);
            
            //Act
            var value = _locationController.GetLocationById(1);
            
            //Assert
            Assert.IsInstanceOfType(value.Result, typeof(NotFoundResult));
        }

        [TestMethod]
        public void CreateLocationTest_Success()
        {
            // Arrange
            var newLocation = new LocationCS { Id = 1, warehouse_id = 1, code = "B.2.1" };
            var createdLocation = new LocationCS { Id = 2, warehouse_id = 5, code = "C.3.2" };
            
            // Set up the mock service to return the created order
            _mockLocationService.Setup(service => service.CreateLocation(newLocation)).Returns(createdLocation);

            // Act
            var result = _locationController.CreateLocation(newLocation);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(CreatedAtActionResult));
            var createdResult = result.Result as CreatedAtActionResult;
            Assert.IsNotNull(createdResult);
            Assert.IsInstanceOfType(createdResult.Value, typeof(LocationCS));
            var returnedLocation = createdResult.Value as LocationCS;
            Assert.AreEqual("C.3.2", returnedLocation.code);
            Assert.AreEqual(5, returnedLocation.warehouse_id);
        }
    }
}

