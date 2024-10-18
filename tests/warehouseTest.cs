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
            IWarehouseService warehouseService = new WarehouseService();
            var value = warehouseService.GetAllWarehouses();
            Assert.IsNotNull(value);
        }
    }
}

