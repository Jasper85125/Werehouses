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
            IWarehouseService warehouseService = new WarehouseService();
            
            //Act
            var value = warehouseService.GetAllWarehouses();
            
            //Assert
            Assert.IsNotNull(value);
        }

        [TestMethod]
        public void GetWarehouseById()
        {
            //arrange
            int id = 1;
            IWarehouseService warehouseService = new WarehouseService();
            
            //Act
            var value = warehouseService.GetWarehouseById(id);
            
            //Assert
            Assert.IsNotNull(value);
        }
    }
}

