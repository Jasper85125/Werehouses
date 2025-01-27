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

            //Act
            var value = _shipmentController.GetAllShipments();

            //Assert
            var okResult = value.Result as OkObjectResult;
            var returnedItems = okResult.Value as IEnumerable<ShipmentCS>;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(2, returnedItems.Count());
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

            //Act
            var value = _shipmentController.GetShipmentById(1);

            //Assert
            var okResult = value.Result as OkObjectResult;
            var returnedItems = okResult.Value as ShipmentCS;
            Assert.IsNotNull(okResult);
            Assert.IsNotNull(okResult.Value);
            Assert.AreEqual(shipments[0].source_id, returnedItems.source_id);
        }

        [TestMethod]
        public void GetShipmentByIdTest_WrongId()
        {
            //arrange
            _mockShipmentService.Setup(service => service.GetShipmentById(1)).Returns((ShipmentCS)null);

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

            //Act
            var value = _shipmentController.GetItemsInShipment(1);

            //Assert
            var okResult = value.Result as OkObjectResult;
            var returnedItems = okResult.Value as IEnumerable<ItemIdAndAmount>;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(2, returnedItems.Count());
        }

        [TestMethod]
        public void CreateShipment_ReturnsCreatedResult_WithNewShipment()
        {
            // Arrange
            var shipment = new ShipmentCS { Id = 1, order_id = 1, source_id = 24 };
            _mockShipmentService.Setup(service => service.CreateShipment(shipment)).Returns(shipment);

            // Act
            var value = _shipmentController.CreateShipment(shipment);

            // Assert
            var createdResult = value.Result as CreatedAtActionResult;
            Assert.IsNotNull(createdResult);

            var returnedItems = createdResult.Value as ShipmentCS;
            Assert.IsNotNull(returnedItems);
            Assert.AreEqual(shipment.source_id, returnedItems.source_id);
        }

        public async Task UpdateShipmentTest_Success()
        {
            // Arrange
            var updatedShipment = new ShipmentCS { Id = 1, order_id = 1, source_id = 24 };
            _mockShipmentService.Setup(service => service.UpdateShipment(1, updatedShipment)).Returns(updatedShipment);

            // Act
            var result = _shipmentController.UpdateShipment(1, updatedShipment);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.IsInstanceOfType(okResult.Value, typeof(ShipmentCS));
            var returnedShipment = okResult.Value as ShipmentCS;
            Assert.AreEqual(updatedShipment.source_id, returnedShipment.source_id);
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

            ShipmentCS testshipment = new ShipmentCS() { Id = 1, order_id = 1, source_id = 33, order_date = DateTime.Parse("2000-03-09"), 
            request_date = DateTime.Parse("2000-03-11"), shipment_date = DateTime.Parse("2000-03-13"), shipment_type="I", shipment_status="Pending",
            Notes="Zee vertrouwen klas rots heet lachen oneven begrijpen.", carrier_code="DPD",
            carrier_description="Dynamic Parcel Distribution", service_code="Fastest", payment_type="Manual",
            transfer_mode="Ground", total_package_count=31, total_package_weight=594.42, created_at=DateTime.Parse("2000-03-10T11:11:14Z"),
            updated_at=DateTime.Parse("2000-03-11T13:11:14Z"), Items=newItemsAndAmounts};

            _mockShipmentService.Setup(service => service.UpdateItemsInShipment(1, newItemsAndAmounts)).Returns(testshipment);

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
        }

        [TestMethod]
        public async Task UpdateShipmentTest_Failed()
        {
            // Arrange
            var updatedShipment = new ShipmentCS { Id = 1, order_id = 1, source_id = 24 };
            _mockShipmentService.Setup(service => service.UpdateShipment(1, updatedShipment)).Returns((ShipmentCS)null);

            // Act
            var result = _shipmentController.UpdateShipment(1, updatedShipment);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));
        }

        [TestMethod]
        public void DeleteShipmentTest_Success()
        {
            // Arrange
            var shipment = new ShipmentCS { Id = 1, order_id = 1, source_id = 24 };
            _mockShipmentService.Setup(service => service.GetShipmentById(1)).Returns(shipment);

            // Act
            var result = _shipmentController.DeleteShipment(1);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkResult));
        }
        [TestMethod]
        public void DeleteShipmentItemTest_Success()
        {
            //arrange
            var shipment = new ShipmentCS { Id = 1, order_id = 1, source_id = 24, Items=new List<ItemIdAndAmount> { new ItemIdAndAmount { item_id = "P01", amount = 23 }}};
            _mockShipmentService.Setup(service => service.GetShipmentById(1)).Returns(shipment);

            //act
            var result = _shipmentController.DeleteItemFromShipment(1, "P01");
            
            //assert
            Assert.IsInstanceOfType(result, typeof(OkResult));
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
        public void GetItemsInShipmentService_Test()
        {
            var shipmentService = new ShipmentService();
            var items = shipmentService.GetItemsInShipment(1);
            Assert.IsNotNull(items);
            Assert.AreEqual(1, items.Count);
        }

        [TestMethod]
        public void UpdateItemsInShipmentService_Test()
        {
            var shipmentService = new ShipmentService();
            List<ItemIdAndAmount> newItemsAndAmounts = new List<ItemIdAndAmount>()
            {
                new ItemIdAndAmount(){ item_id= "P007435", amount= 100},
                new ItemIdAndAmount(){ item_id= "P009553", amount= 100},
                new ItemIdAndAmount(){ item_id= "P002084", amount= 100}
            };
            var shipment = shipmentService.UpdateItemsInShipment(1, newItemsAndAmounts);
            Assert.IsNotNull(shipment);
            Assert.AreEqual(3, shipment.Items.Count);
        }
        

        [TestMethod]
        public void GetShipmentById_ServiceTest_Exists()
        {
            // Arrange
            var shipments = new List<ShipmentCS>
            {
                new ShipmentCS { Id = 1, order_id = 1, source_id = 24 },
                new ShipmentCS { Id = 2, order_id = 4, source_id = 10 },
            };
            var shipmentService = new ShipmentService();
            var shipment = shipmentService.GetShipmentById(1);

            // Act
            var result = shipments.FirstOrDefault(s => s.Id == 1);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Id);
        }

        [TestMethod]
        public void GetShipmentById_ServiceTest_NotFound()
        {
            // Arrange
            var shipments = new List<ShipmentCS>
            {
                new ShipmentCS { Id = 1, order_id = 1, source_id = 24 },
                new ShipmentCS { Id = 2, order_id = 4, source_id = 10 },
            };
            var shipmentService = new ShipmentService();
            var shipment = shipmentService.GetShipmentById(3);

            // Act
            var result = shipments.FirstOrDefault(s => s.Id == 3);

            // Assert
            Assert.IsNull(result);
        }
    }
}