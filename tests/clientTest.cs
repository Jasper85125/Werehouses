using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;
using Microsoft.AspNetCore.Mvc;
using clients.Services;
using clients.Controllers;

namespace clients.Test
{
    [TestClass]
    public class ClientTest
    {
        private Mock<ClientService> _clientservice;
        private ClientController _clientcontroller;

        [TestInitialize]
        public void Setup()
        {
            _clientservice = new Mock<ClientService>();
            _clientcontroller = new ClientController(_clientservice.Object);
        }

        [TestMethod]
        public void GetAllClients_Test_returns_true()
        {
            //arrange
            var listofclients = new List<ClientCS>()
            {
                new ClientCS{ Address="", City="", ConactPhone="", ContactEmail="", ContactName="", Country="", CreatedAt=new DateTime(), Id=new int(), Name="", Province="", UpdatedAt=new DateTime(), ZipCode=""},
                new ClientCS{ Address="", City="", ConactPhone="", ContactEmail="", ContactName="", Country="", CreatedAt=new DateTime(), Id=new int(), Name="", Province="", UpdatedAt=new DateTime(), ZipCode=""},
            };

            //Act
            _clientservice.Setup(_ => _.GetAllClients()).Returns(listofclients);

            //Assert
            var result = _clientcontroller.GetAllClients();
            var result_count = result as List<ClientCS>;
            Assert.AreEqual(2, result_count.Count);
        }
    }

}
