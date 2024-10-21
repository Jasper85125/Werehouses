using Microsoft.VisualStudio.TestTools.UnitTesting;
using inventory.Services;

namespace InventoryTest
{
    [TestClass]
    public class InventoryTest
    {
        [TestMethod]
        public void GetInventoryTest()
        {
            IInventoryService inventoryService = new InventoryService();
            var value = inventoryService.GetAllInventories();
            Assert.IsNotNull(value);
        }

        [TestMethod]
        public void GetInventoryByIdTest()
        {
            IInventoryService inventoryService = new InventoryService();
            var value = inventoryService.GetInventoryById();
            Assert.IsNotNull(value);
        }
    }
}

