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
    public class ShipmentTest
    {
        private Mock<IShipmentService> _mockShipmentService;
        private ShipmentController _shipmentController;

        [TestInitialize]
        public void Setup()
        {
            _mockShipmentService = new Mock<IShipmentService>();
            _shipmentController = new ShipmentController(_mockShipmentService.Object);
        }

        [TestMethod]
        public void GetShipmentsTest_Exists()
        {
            //arrange
            var shipments = new List<ShipmentCS>
            {
                new ShipmentCS { Id = 1, order_id = 1, source_id = 24 },
                new ShipmentCS { Id = 2, order_id = 4, source_id = 10 },
            };
            _mockShipmentService.Setup(service => service.GetAllShipments()).Returns(shipments);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";  // Set the UserRole in HttpContext

            // Assign HttpContext to the controller
            _shipmentController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            //Act
            var value = _shipmentController.GetAllShipments(null, 1, 10);
            var okResult = value.Result as OkObjectResult;
            var returnedItems = okResult.Value as PaginationCS<ShipmentCS>;

            //Assert
            Assert.IsNotNull(okResult);
            Assert.AreEqual(2, returnedItems.Data.Count());
            
            httpContext.Items["UserRole"] = "Skipper";
            _shipmentController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            //act
            var result = _shipmentController.GetAllShipments(null, 1, 10);

            //assert
            var unauthorizedResult = result.Result as UnauthorizedResult;
            Assert.IsNotNull(unauthorizedResult);
            Assert.AreEqual(401, unauthorizedResult.StatusCode);
        }

        [TestMethod]
        public void GetShipmentByIdTest_Exists()
        {
            //arrange
            var shipments = new List<ShipmentCS>
            {
                new ShipmentCS { Id = 1, order_id = 1, source_id = 24 },
                new ShipmentCS { Id = 2, order_id = 4, source_id = 10 },
            };
            _mockShipmentService.Setup(service => service.GetShipmentById(1)).Returns(shipments[0]);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";  // Set the UserRole in HttpContext

            // Assign HttpContext to the controller
            _shipmentController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            //Act
            var value = _shipmentController.GetShipmentById(1);
            var okResult = value.Result as OkObjectResult;
            var returnedItems = okResult.Value as ShipmentCS;

            //Assert
            Assert.IsNotNull(okResult);
            Assert.IsNotNull(okResult.Value);
            Assert.AreEqual(shipments[0].source_id, returnedItems.source_id);

            httpContext.Items["UserRole"] = "Skipper";
            _shipmentController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            //act
            var result = _shipmentController.GetShipmentById(1);

            //assert
            var unauthorizedResult = result.Result as UnauthorizedResult;
            Assert.IsNotNull(unauthorizedResult);
            Assert.AreEqual(401, unauthorizedResult.StatusCode);
        }

        [TestMethod]
        public void GetShipmentByIdTest_WrongId()
        {
            //arrange
            _mockShipmentService.Setup(service => service.GetShipmentById(1)).Returns((ShipmentCS)null);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";  // Set the UserRole in HttpContext

            // Assign HttpContext to the controller
            _shipmentController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            //Act
            var value = _shipmentController.GetShipmentById(1);

            //Assert
            Assert.IsInstanceOfType(value.Result, typeof(NotFoundResult));
        }
        //GetItemsInShipment test
        [TestMethod]
        public void GetItemsInShipmentTest_Exists()
        {
            //arrange
            var items = new List<ItemIdAndAmount>
            {
                new ItemIdAndAmount { item_id = "P01", amount = 23 },
                new ItemIdAndAmount { item_id = "P02", amount = 12 },
            };
            _mockShipmentService.Setup(service => service.GetItemsInShipment(1)).Returns(items);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";  // Set the UserRole in HttpContext

            // Assign HttpContext to the controller
            _shipmentController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            //Act
            var value = _shipmentController.GetItemsInShipment(1);
            var okResult = value.Result as OkObjectResult;
            var returnedItems = okResult.Value as IEnumerable<ItemIdAndAmount>;

            //Assert
            Assert.IsNotNull(okResult);
            Assert.AreEqual(2, returnedItems.Count());

            httpContext.Items["UserRole"] = "Skipper";
            _shipmentController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            //act
            var result = _shipmentController.GetShipmentById(1);

            //assert
            var unauthorizedResult = result.Result as UnauthorizedResult;
            Assert.IsNotNull(unauthorizedResult);
            Assert.AreEqual(401, unauthorizedResult.StatusCode);
        }

        [TestMethod]
        public void CreateShipment_ReturnsCreatedResult_WithNewShipment()
        {
            // Arrange
            var shipment = new ShipmentCS { Id = 1, order_id = 1, source_id = 24 };
            _mockShipmentService.Setup(service => service.CreateShipment(shipment)).Returns(shipment);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";  // Set the UserRole in HttpContext

            // Assign HttpContext to the controller
            _shipmentController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var value = _shipmentController.CreateShipment(shipment);

            // Assert
            var createdResult = value.Result as CreatedAtActionResult;  // Use CreatedAtActionResult
            Assert.IsNotNull(createdResult);

            var returnedItems = createdResult.Value as ShipmentCS;
            Assert.IsNotNull(returnedItems);
            Assert.AreEqual(shipment.source_id, returnedItems.source_id);

            httpContext.Items["UserRole"] = "Skipper";
            _shipmentController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            //act
            var result = _shipmentController.CreateShipment(shipment);

            //assert
            var unauthorizedResult = result.Result as UnauthorizedResult;
            Assert.IsNotNull(unauthorizedResult);
            Assert.AreEqual(401, unauthorizedResult.StatusCode);
        }

        [TestMethod]
        public void CreateMultipleShipments_ReturnsCreatedResult_WithNewShipments()
        {
            // Arrange
            var shipments = new List<ShipmentCS>
            {
                new ShipmentCS { Id = 1, order_id = 1, source_id = 24, shipment_type = "I", shipment_status = "Transit", carrier_code = "PostNL",
                                 service_code = "ThreeDay", payment_type = "Card", transfer_mode = "Ground", total_package_count = 56,
                                 total_package_weight = 42.50, Items = new List<ItemIdAndAmount>{ new ItemIdAndAmount { item_id = "P007435", amount = 23 }}},
                new ShipmentCS { Id = 1, order_id = 1, source_id = 24, shipment_type = "I", shipment_status = "Transit", carrier_code = "PostNL",
                                 service_code = "ThreeDay", payment_type = "Card", transfer_mode = "Ground", total_package_count = 56,
                                 total_package_weight = 42.50,Items = new List<ItemIdAndAmount>{ new ItemIdAndAmount { item_id = "P007435", amount = 23 }}}
            };
            _mockShipmentService.Setup(service => service.CreateMultipleShipments(shipments)).Returns(shipments);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";  // Set the UserRole in HttpContext

            // Assign HttpContext to the controller
            _shipmentController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = _shipmentController.CreateMultipleShipments(shipments);
            var createdResult = result.Result as ObjectResult;
            var returnedItems = createdResult.Value as List<ShipmentCS>;
            var firstOrder = returnedItems[0];

            // Assert
            Assert.IsNotNull(createdResult);
            Assert.IsNotNull(returnedItems);
            Assert.AreEqual(shipments[0].source_id, firstOrder.source_id);
            Assert.AreEqual(shipments[0].order_id, firstOrder.order_id);

            httpContext.Items["UserRole"] = "Skipper";
            _shipmentController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            //act
            result = _shipmentController.CreateMultipleShipments(shipments);

            //assert
            var unauthorizedResult = result.Result as UnauthorizedResult;
            Assert.IsNotNull(unauthorizedResult);
            Assert.AreEqual(401, unauthorizedResult.StatusCode);
        }

        public async Task UpdateShipmentTest_Success()
        {
            // Arrange
            var updatedShipment = new ShipmentCS { Id = 1, order_id = 1, source_id = 24 };
            _mockShipmentService.Setup(service => service.UpdateShipment(1, updatedShipment)).Returns(updatedShipment);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";  // Set the UserRole in HttpContext

            // Assign HttpContext to the controller
            _shipmentController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = _shipmentController.UpdateShipment(1, updatedShipment);
            var okResult = result.Result as OkObjectResult;
            var returnedShipment = okResult.Value as ShipmentCS;

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            Assert.IsNotNull(okResult);
            Assert.IsInstanceOfType(okResult.Value, typeof(ShipmentCS));
            Assert.AreEqual(updatedShipment.source_id, returnedShipment.source_id);

            httpContext.Items["UserRole"] = "Skipper";
            _shipmentController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            //act
            result = _shipmentController.UpdateShipment(1, updatedShipment);

            //assert
            var unauthorizedResult = result.Result as UnauthorizedResult;
            Assert.IsNotNull(unauthorizedResult);
            Assert.AreEqual(401, unauthorizedResult.StatusCode);
        }

        [TestMethod]
        public void UpdateItemsInShipmentById_Succes()
        {
            //Arrange
            List<ItemIdAndAmount> newItemsAndAmounts = new List<ItemIdAndAmount>()
            {
                new ItemIdAndAmount(){ item_id= "P007435", amount= 100},
                new ItemIdAndAmount(){ item_id= "P009553", amount= 100},
                new ItemIdAndAmount(){ item_id= "P002084", amount= 100}
            };

            ShipmentCS testshipment = new ShipmentCS()
            {
                Id = 1,
                order_id = 1,
                source_id = 33,
                order_date = DateTime.Parse("2000-03-09"),
                request_date = DateTime.Parse("2000-03-11"),
                shipment_date = DateTime.Parse("2000-03-13"),
                shipment_type = "I",
                shipment_status = "Pending",
                Notes = "Zee vertrouwen klas rots heet lachen oneven begrijpen.",
                carrier_code = "DPD",
                carrier_description = "Dynamic Parcel Distribution",
                service_code = "Fastest",
                payment_type = "Manual",
                transfer_mode = "Ground",
                total_package_count = 31,
                total_package_weight = 594.42,
                created_at = DateTime.Parse("2000-03-10T11:11:14Z"),
                updated_at = DateTime.Parse("2000-03-11T13:11:14Z"),
                Items = newItemsAndAmounts
            };

            _mockShipmentService.Setup(service => service.UpdateItemsInShipment(1, newItemsAndAmounts)).Returns(testshipment);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";  // Set the UserRole in HttpContext

            // Assign HttpContext to the controller
            _shipmentController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            //Act
            var result = _shipmentController.UpdateItemsinShipment(1, newItemsAndAmounts);
            var okResult = result.Result as OkObjectResult;
            var value = okResult.Value as ShipmentCS;

            //Assert
            Assert.IsNotNull(okResult);
            Assert.AreEqual(okResult.StatusCode, 200);
            Assert.IsNotNull(value);
            Assert.AreEqual(newItemsAndAmounts.Count, value.Items.Count);
            Assert.AreEqual(value.Items[0].amount, 100);
            Assert.AreEqual(value.Items[1].amount, 100);
            Assert.AreEqual(value.Items[2].amount, 100);

            httpContext.Items["UserRole"] = "Skipper";
            _shipmentController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            //act
            result = _shipmentController.UpdateItemsinShipment(1, newItemsAndAmounts);

            //assert
            var unauthorizedResult = result.Result as UnauthorizedResult;
            Assert.IsNotNull(unauthorizedResult);
            Assert.AreEqual(401, unauthorizedResult.StatusCode);
        }

        [TestMethod]
        public async Task UpdateShipmentTest_Failed()
        {
            // Arrange
            var updatedShipment = new ShipmentCS { Id = 1, order_id = 1, source_id = 24 };
            _mockShipmentService.Setup(service => service.UpdateShipment(1, updatedShipment)).Returns((ShipmentCS)null);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";  // Set the UserRole in HttpContext

            // Assign HttpContext to the controller
            _shipmentController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = _shipmentController.UpdateShipment(1, updatedShipment);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));

            httpContext.Items["UserRole"] = "Skipper";
            _shipmentController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            //act
            result = _shipmentController.UpdateShipment(1, updatedShipment);

            //assert
            var unauthorizedResult = result.Result as UnauthorizedResult;
            Assert.IsNotNull(unauthorizedResult);
            Assert.AreEqual(401, unauthorizedResult.StatusCode);
        }

        [TestMethod]
        public void PatchShipmentTest_Succes()
        {
            //Arrange
            var patchedshipment = new ShipmentCS() { Id = 1, Notes = "EW" };
            _mockShipmentService.Setup(service => service.PatchShipment(1, "Notes", "EW")).Returns(patchedshipment);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";  // Set the UserRole in HttpContext

            // Assign HttpContext to the controller
            _shipmentController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            //Act
            var result = _shipmentController.PatchShipment(1, "Notes", "EW");
            var resultok = result.Result as OkObjectResult;
            var value = resultok.Value as ShipmentCS;

            //Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(resultok);
            Assert.IsNotNull(value);
            Assert.AreEqual(resultok.StatusCode, 200);
            Assert.AreEqual(value.Notes, patchedshipment.Notes);

            httpContext.Items["UserRole"] = "Skipper";
            _shipmentController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            //act
            result = _shipmentController.PatchShipment(1, "Notes", "EW");

            //assert
            var unauthorizedResult = result.Result as UnauthorizedResult;
            Assert.IsNotNull(unauthorizedResult);
            Assert.AreEqual(401, unauthorizedResult.StatusCode);
        }

        [TestMethod]
        public void DeleteShipmentTest_Success()
        {
            // Arrange
            var shipment = new ShipmentCS { Id = 1, order_id = 1, source_id = 24 };
            _mockShipmentService.Setup(service => service.GetShipmentById(1)).Returns(shipment);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";  // Set the UserRole in HttpContext

            // Assign HttpContext to the controller
            _shipmentController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = _shipmentController.DeleteShipment(1);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkResult));

            httpContext.Items["UserRole"] = "Skipper";
            _shipmentController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            //act
            result = _shipmentController.DeleteShipment(1);

            //assert
            var unauthorizedResult = result as UnauthorizedResult;
            Assert.IsNotNull(unauthorizedResult);
            Assert.AreEqual(401, unauthorizedResult.StatusCode);
        }

        [TestMethod]
        public void DeleteShipmentItemTest_Success()
        {
            //arrange
            var shipment = new ShipmentCS { Id = 1, order_id = 1, source_id = 24, Items = new List<ItemIdAndAmount> { new ItemIdAndAmount { item_id = "P01", amount = 23 } } };
            _mockShipmentService.Setup(service => service.GetShipmentById(1)).Returns(shipment);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";  // Set the UserRole in HttpContext

            // Assign HttpContext to the controller
            _shipmentController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            //act
            var result = _shipmentController.DeleteItemFromShipment(1, "P01");

            //assert
            Assert.IsInstanceOfType(result, typeof(OkResult));

            httpContext.Items["UserRole"] = "Skipper";
            _shipmentController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            //act
            result = _shipmentController.DeleteItemFromShipment(1, "P01");

            //assert
            var unauthorizedResult = result as UnauthorizedResult;
            Assert.IsNotNull(unauthorizedResult);
            Assert.AreEqual(401, unauthorizedResult.StatusCode);
        }

        [TestMethod]
        public void DeleteShipmentsTest_Succes()
        {
            //Arrange
            var shipmentsToDelete = new List<int>() { 1, 2, 3 };

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";  // Set the UserRole in HttpContext

            // Assign HttpContext to the controller
            _shipmentController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            //Act
            var result = _shipmentController.DeleteShipments(shipmentsToDelete);
            var resultok = result as OkObjectResult;

            //Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            Assert.AreEqual(resultok.StatusCode, 200);

            httpContext.Items["UserRole"] = "Skipper";
            _shipmentController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            //act
            result = _shipmentController.DeleteShipments(shipmentsToDelete);

            //assert
            var unauthorizedResult = result as UnauthorizedResult;
            Assert.IsNotNull(unauthorizedResult);
            Assert.AreEqual(401, unauthorizedResult.StatusCode);
        }
    }
}

