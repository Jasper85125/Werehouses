using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;
using Microsoft.AspNetCore.Mvc;
using clients.Services;
using client.Controllers;

namespace clients.Test{
    [TestClass]
    public class ClientTest{
        private Mock<ClientService> _clientservice;
        private ClientController _clientcontroller;
        [TestInitialize]
        public void Setup(){
            _clientservice = new Mock<ClientService>();
            _clientcontroller = new ClientController(_clientservice.Object);
        }
        public void GetAllClients_Test_returns_true(){
            var listofclients = new List<ClientCS>(){
                new ClientCS{ Address="", City="", ConactPhone="", ContactEmail="", ContactName="", Country="", CreatedAt=new DateTime(), Id=new int(), Name="", Province="", UpdatedAt=new DateTime(), ZipCode=""},
                new ClientCS{ Address="", City="", ConactPhone="", ContactEmail="", ContactName="", Country="", CreatedAt=new DateTime(), Id=new int(), Name="", Province="", UpdatedAt=new DateTime(), ZipCode=""},
            };
            _clientservice.Setup(_ => _.GetAllClients()).Returns(listofclients);
            var result = _clientcontroller.GetAllClients();
            var result_count = result as List<ClientCS>;
            Assert.AreEqual(2, result_count.Count);
        }
    }

}