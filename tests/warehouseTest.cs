using Microsoft.VisualStudio.TestTools.UnitTesting;
using warehouse.Services;

namespace WarehouseTest
{
    [TestClass]
    public class WarehouseTest
    {
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

