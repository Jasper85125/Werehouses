using Microsoft.VisualStudio.TestTools.UnitTesting;
using warehouse.Services;
using Moq;
using warehouses.Controllers;
using System.Data.Common;

namespace Tests
{
    [TestClass]
    public class WarehouseTest
    {
        private Mock<IWarehouseService> _mockWarehouseService;
        private WarehouseController _warehouseController;
        private IWarehouseService _warehouseService;

        [TestInitialize]
        public void Setup()
        {
            _mockWarehouseService = new Mock<IWarehouseService>();
            _warehouseController = new WarehouseController(_mockWarehouseService.Object);
        }

        [TestMethod]
        public void GetWarehousesTest()
        {
            //arrange
            List<WarehouseCS> warehouses = new List<WarehouseCS>
            {
                new WarehouseCS {Id = 1, Address = "Straat 1"},
                new WarehouseCS {Id = 2, Address = "Warenhuisstraat 201"}
            };
            _mockWarehouseService.Setup(service => service.GetAllWarehouses()).Returns(warehouses);

            //act
            var value = _warehouseController.GetAllWarehouses();

            //assert
            Assert.IsNotNull(value);
        }

        [TestMethod]
        public void GetWarehouseByIdTest()
        {
            var value = _warehouseController.GetWarehouseById(1);
            Assert.IsNotNull(value);
        }
    }
}

