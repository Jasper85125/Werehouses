using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using item.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace clients.Test{
    public class ClientTest{
        
        [TestMethod]
        public void GetAllClients_Test()
        {
            var Mockclientdb = new Mock<ClientService>();
            var result = Mockclientdb.GetAllClients();
            // Assert.IsNotEmpty(result);
        }
    }

}
