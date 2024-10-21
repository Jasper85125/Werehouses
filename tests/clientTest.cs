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
                new ClientCS{ Address="street", City="city", contact_phone="number", contact_email="email", contact_name="name", Country="Japan", created_at=default, Id=1, Name="name", Province="province", updated_at=default, zip_code="zip"},
                new ClientCS{ Address="street2", City="city2", contact_phone="number2", contact_email="email2", contact_name="name2", Country="Japan2", created_at=default, Id=2, Name="name2", Province="province2", updated_at=default, zip_code="zip2"},
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

        [TestMethod]
        public void GetClientById_Test_returns_true(){
            //arrange
            var client = new ClientCS(){Id=1, Address="", City="", ConactPhone="", ContactEmail="", ContactName="", Country="", CreatedAt=default, UpdatedAt=default, Name="", Province="", ZipCode=""};
            _clientservice.Setup(_ => _.GetClientById(client.Id)).Returns(client);

            //act
            // var result = _clientservice.Setup(_ => _.GetClientById(client.Id)).Returns(client);
            var result = _clientcontroller.GetClientById(1);

            //assert
            var resultok = result.Result as OkObjectResult;
            Assert.IsNotNull(resultok);
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
        }
    }

}
