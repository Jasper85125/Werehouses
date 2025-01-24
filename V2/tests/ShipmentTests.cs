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
    public class ShipmentTest
    {
        private Mock<IShipmentService> _mockShipmentService;
        private ShipmentController _shipmentController;

        [TestInitialize]
        public void Setup()
        {
            _mockShipmentService = new Mock<IShipmentService>();
            _shipmentController = new ShipmentController(_mockShipmentService.Object);

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "../../data/shipments.json");
            var shipment = new ShipmentCS
            {
                Id = 1,
                order_id = 1,
                source_id = 33,
                order_date = DateTime.Parse("2000-03-09T00:00:00"),
                request_date = DateTime.Parse("2000-03-11T00:00:00"),
                shipment_date = DateTime.Parse("2000-03-13T00:00:00"),
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
                created_at = DateTime.Now,
                updated_at = DateTime.Now,
                Items = new List<ItemIdAndAmount>
                {
                    new ItemIdAndAmount { item_id = "P01", amount = 23 }
                }
            };

            var shipmentList = new List<ShipmentCS> { shipment };
            var json = JsonConvert.SerializeObject(shipmentList, Formatting.Indented);

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
            httpContext.Items["UserRole"] = "Admin";

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
            httpContext.Items["UserRole"] = "Admin";

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
            httpContext.Items["UserRole"] = "Admin";

            _shipmentController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            //Act
            var value = _shipmentController.GetShipmentById(1);

            //Assert
            Assert.IsInstanceOfType(value.Result, typeof(NotFoundResult));
        }

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
            httpContext.Items["UserRole"] = "Admin";

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
            httpContext.Items["UserRole"] = "Admin";

            _shipmentController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var value = _shipmentController.CreateShipment(shipment);

            // Assert
            var createdResult = value.Result as CreatedAtActionResult;
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
            httpContext.Items["UserRole"] = "Admin";

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
            httpContext.Items["UserRole"] = "Admin";

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
            httpContext.Items["UserRole"] = "Admin";

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
            httpContext.Items["UserRole"] = "Admin";

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
            httpContext.Items["UserRole"] = "Admin";

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
            httpContext.Items["UserRole"] = "Admin";

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
            httpContext.Items["UserRole"] = "Admin";

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
            httpContext.Items["UserRole"] = "Admin";

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

        [TestMethod]
        public void GetAllShipmentsService_Test()
        {
            var shipmentService = new ShipmentService();
            var shipments = shipmentService.GetAllShipments();
            Assert.IsNotNull(shipments);
            Assert.AreEqual(1, shipments.Count);
        }

        [TestMethod]
        public void GetShipmentByIdService_Test()
        {
            var shipmentService = new ShipmentService();
            var shipment = shipmentService.GetShipmentById(1);
            Assert.IsNotNull(shipment);
            Assert.AreEqual(1, shipment.Id);
        }

        [TestMethod]
        public void CreateShipmentService_Test()
        {
            var shipment = new ShipmentCS
            {
                Id = 2,
                order_id = 1,
                source_id = 24,
                shipment_type = "I",
                shipment_status = "Pending",
                carrier_code = "DPD",
                carrier_description = "Dynamic Parcel Distribution",
                service_code = "Fastest",
                payment_type = "Manual",
                transfer_mode = "Ground",
                total_package_count = 31,
                total_package_weight = 594.42,
                created_at = DateTime.Now,
                updated_at = DateTime.Now,
                Items = new List<ItemIdAndAmount>
            {
                new ItemIdAndAmount { item_id = "P01", amount = 23 }
            }
            };
            var shipmentService = new ShipmentService();
            var createdShipment = shipmentService.CreateShipment(shipment);
            Assert.IsNotNull(createdShipment);
            Assert.AreEqual(2, createdShipment.Id);

            var shipmentsUpdated = shipmentService.GetAllShipments();
            Assert.AreEqual(2, shipmentsUpdated.Count);
        }

        [TestMethod]
        public void CreateMultipleShipmentsService_Test()
        {
            var shipments = new List<ShipmentCS>
            {
            new ShipmentCS
            {
                Id = 2,
                order_id = 1,
                source_id = 24,
                shipment_type = "I",
                shipment_status = "Pending",
                carrier_code = "DPD",
                carrier_description = "Dynamic Parcel Distribution",
                service_code = "Fastest",
                payment_type = "Manual",
                transfer_mode = "Ground",
                total_package_count = 31,
                total_package_weight = 594.42,
                created_at = DateTime.Now,
                updated_at = DateTime.Now,
                Items = new List<ItemIdAndAmount>
                {
                new ItemIdAndAmount { item_id = "P01", amount = 23 }
                }
            },
            new ShipmentCS
            {
                Id = 3,
                order_id = 2,
                source_id = 25,
                shipment_type = "I",
                shipment_status = "Pending",
                carrier_code = "UPS",
                carrier_description = "United Parcel Service",
                service_code = "Standard",
                payment_type = "Credit",
                transfer_mode = "Air",
                total_package_count = 15,
                total_package_weight = 300.00,
                created_at = DateTime.Now,
                updated_at = DateTime.Now,
                Items = new List<ItemIdAndAmount>
                {
                new ItemIdAndAmount { item_id = "P02", amount = 10 }
                }
            }
            };
            var shipmentService = new ShipmentService();
            var createdShipments = shipmentService.CreateMultipleShipments(shipments);
            Assert.IsNotNull(createdShipments);

            var shipmentsUpdated = shipmentService.GetAllShipments();
            Assert.AreEqual(3, shipmentsUpdated.Count);
        }

        [TestMethod]
        public void UpdateShipmentService_Test()
        {
            var shipment = new ShipmentCS
            {
                Id = 1,
                order_id = 1,
                source_id = 24,
                shipment_type = "I",
                shipment_status = "Pending",
                carrier_code = "DPD",
                carrier_description = "Dynamic Parcel Distribution",
                service_code = "Fastest",
                payment_type = "Manual",
                transfer_mode = "Ground",
                total_package_count = 31,
                total_package_weight = 594.42,
                created_at = DateTime.Now,
                updated_at = DateTime.Now,
                Items = new List<ItemIdAndAmount>
            {
                new ItemIdAndAmount { item_id = "P01", amount = 23 }
            }
            };
            var shipmentService = new ShipmentService();
            var updatedShipment = shipmentService.UpdateShipment(1, shipment);
            Assert.IsNotNull(updatedShipment);
            Assert.AreEqual(1, updatedShipment.Id);
        }

        [TestMethod]
        public void UpdateShipmentService_Test_Failed()
        {
            var shipment = new ShipmentCS
            {
                Id = 3,
                order_id = 1,
                source_id = 24,
                shipment_type = "I",
                shipment_status = "Pending",
                carrier_code = "DPD",
                carrier_description = "Dynamic Parcel Distribution",
                service_code = "Fastest",
                payment_type = "Manual",
                transfer_mode = "Ground",
                total_package_count = 31,
                total_package_weight = 594.42,
                created_at = DateTime.Now,
                updated_at = DateTime.Now,
                Items = new List<ItemIdAndAmount>
            {
                new ItemIdAndAmount { item_id = "P01", amount = 23 }
            }
            };
            var shipmentService = new ShipmentService();
            var updatedShipment = shipmentService.UpdateShipment(3, shipment);
            Assert.IsNull(updatedShipment);
        }

        [TestMethod]
        public void DeleteShipmentService_Test()
        {
            var shipmentService = new ShipmentService();
            shipmentService.DeleteShipment(1);
            var shipmentsUpdated = shipmentService.GetAllShipments();
            Assert.AreEqual(0, shipmentsUpdated.Count);
        }

        [TestMethod]
        public void DeleteShipmentService_Test_Failed()
        {
            var shipmentService = new ShipmentService();
            shipmentService.DeleteShipment(3);
            var shipmentsUpdated = shipmentService.GetAllShipments();
            Assert.AreEqual(1, shipmentsUpdated.Count);
        }

        [TestMethod]
        public void DeleteMultipleShipmentsService_Test()
        {
            var shipment = new ShipmentCS
            {
                Id = 2,
                order_id = 1,
                source_id = 24,
                shipment_type = "I",
                shipment_status = "Pending",
                carrier_code = "DPD",
                carrier_description = "Dynamic Parcel Distribution",
                service_code = "Fastest",
                payment_type = "Manual",
                transfer_mode = "Ground",
                total_package_count = 31,
                total_package_weight = 594.42,
                created_at = DateTime.Now,
                updated_at = DateTime.Now,
                Items = new List<ItemIdAndAmount>
            {
                new ItemIdAndAmount { item_id = "P01", amount = 23 }
            }
            };
            var shipmentService = new ShipmentService();
            var createdShipment = shipmentService.CreateShipment(shipment);
            Assert.IsNotNull(createdShipment);
            Assert.AreEqual(2, createdShipment.Id);

            var shipmentsUpdated = shipmentService.GetAllShipments();
            Assert.AreEqual(2, shipmentsUpdated.Count);
            List<int> shipmentsToDelete = new List<int> { 1, 2 };
            shipmentService.DeleteShipments(shipmentsToDelete);
            var shipmentsAfterDelete = shipmentService.GetAllShipments();
            Assert.AreEqual(0, shipmentsAfterDelete.Count);
        }

        [TestMethod]
        public void PatchShipmentService_Test()
        {
            var shipmentService = new ShipmentService();
            var shipment = shipmentService.PatchShipment(1, "shipment_status", "Delivered");
            shipment = shipmentService.PatchShipment(1, "carrier_code", "UPS");
            shipment = shipmentService.PatchShipment(1, "service_code", "NextDay");
            shipment = shipmentService.PatchShipment(1, "payment_type", "Credit");
            shipment = shipmentService.PatchShipment(1, "transfer_mode", "Air");
            shipment = shipmentService.PatchShipment(1, "Notes", "Updated Notes");
            Assert.IsNotNull(shipment);
            Assert.AreEqual("Delivered", shipment.shipment_status);
            Assert.AreEqual("UPS", shipment.carrier_code);
            Assert.AreEqual("NextDay", shipment.service_code);
            Assert.AreEqual("Credit", shipment.payment_type);
            Assert.AreEqual("Air", shipment.transfer_mode);
            Assert.AreEqual("Updated Notes", shipment.Notes);
        }

        [TestMethod]
        public void PatchShipmentService_Test_Failed()
        {
            var shipmentService = new ShipmentService();
            var shipment = shipmentService.PatchShipment(3, "Notes", "Updated Notes");
            Assert.IsNull(shipment);
        }
    }
}