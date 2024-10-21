using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;
using Microsoft.AspNetCore.Mvc;
using Services;
using Controllers;

namespace clients.Test
{
    [TestClass]
    public class ClientTest
    {
        private Mock<IClientService> _clientservice;
        private ClientController _clientcontroller;

        [TestInitialize]
        public void Setup()
        {
            _clientservice = new Mock<IClientService>();
            _clientcontroller = new ClientController(_clientservice.Object);
        }

        [TestMethod]
        public void GetAllClients_Test_returns_true()
        {
            //arrange
            var listofclients = new List<ClientCS>()
            {
                new ClientCS{ Address="street", City="city", ConactPhone="number", ContactEmail="email", ContactName="name", Country="Japan", CreatedAt=default, Id=1, Name="name", Province="province", UpdatedAt=default, ZipCode="zip"},
                new ClientCS{ Address="street2", City="city2", ConactPhone="number2", ContactEmail="email2", ContactName="name2", Country="Japan2", CreatedAt=default, Id=2, Name="name2", Province="province2", UpdatedAt=default, ZipCode="zip2"},
            };
            _clientservice.Setup(_ => _.GetAllClients()).Returns(listofclients);

            //act
            var result = _clientcontroller.GetAllClients();
            
            //assert
            var okResult = result.Result as OkObjectResult;
            var returnedItems = okResult.Value as IEnumerable<ClientCS>;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(2, returnedItems.Count());
        }
    }

}
