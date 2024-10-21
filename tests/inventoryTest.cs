using Microsoft.VisualStudio.TestTools.UnitTesting;
using inventory.Services;
using Moq;

namespace Tests
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
            var value = inventoryService.GetInventoryById(1);
            Assert.IsNotNull(value);
        }
    }
}

