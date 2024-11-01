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
            var client = new ClientCS(){Id=1, Address="", City="", contact_phone="", contact_email="", contact_name="", Country="", created_at=default, updated_at=default, Name="", Province="", zip_code=""};
            _clientservice.Setup(_ => _.GetClientById(client.Id)).Returns(client);

            //act
            // var result = _clientservice.Setup(_ => _.GetClientById(client.Id)).Returns(client);
            var result = _clientcontroller.GetClientById(1);

            //assert
            var resultok = result.Result as OkObjectResult;
            Assert.IsNotNull(resultok);
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
        }

        [TestMethod]
        public void CreateClient_ReturnsCreatedResult_WithNewClient()
        {
            // Arrange
            var client = new ClientCS { Id = 1, Address = "Straat 1" };
    
            _mockClientService.Setup(service => service.CreateWarehouse(client)).Returns(client);
            
            // Act
            var result = _clientController.CreateClient(client);
            
            // Assert
            var createdResult = result.Result as CreatedAtActionResult;  // Use CreatedAtActionResult here
            Assert.IsNotNull(createdResult);
            
            var returnedItems = createdResult.Value as clientCS;
            Assert.IsNotNull(returnedItems);
            Assert.AreEqual(client.Address, returnedItems.Address);
        }

        [TestMethod]
        public void UpdatedClientTest_Success()
        {
            // Arrange
            var updatedClient = new ClientCS { Id= 1, Code= "X", Name= "cargo hub", Address= "bruv", Zip= "4002 AZ", City= "hub", Province= "Utrecht",
                                                    Country= "GER", Contact= new Dictionary<string, string>{ {"name", "Fem Keijzer"}, {"phone", "(078) 0013363"}, {"email", "blamore@example.net"}}
                                                   };

             _mockClientService.Setup(service => service.UpdateClient(1, updatedClient)).Returns(updatedClient);

            // Act
            var result = _clientcontroller.UpdateClient(1, updatedClient);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var createdResult = result.Result as OkObjectResult;
            Assert.IsNotNull(createdResult);
            Assert.IsInstanceOfType(createdResult.Value, typeof(ClientCS));
            var returnedClient = createdResult.Value as ClientCS;
            Assert.AreEqual(updatedClient.Code, returnedClient.Code);
            Assert.AreEqual(updatedClient.Address, returnedClient.Address);
        }

        [TestMethod]
        public void UpdatedClientTest_Failed()
        {
            // Arrange
            var updatedClient = new ClientCS { Id= 1, Code= "X", Name= "cargo hub", Address= "bruv", Zip= "4002 AZ", City= "hub", Province= "Utrecht",
                                                    Country= "GER", Contact= new Dictionary<string, string>{ {"name", "Fem Keijzer"}, {"phone", "(078) 0013363"}, {"email", "blamore@example.net"}}
                                                   };

             _mockClientService.Setup(service => service.UpdateClient(0, updatedClient)).Returns((ClientCS)null);

            // Act
            var result = _clientcontroller.UpdateClient(0, updatedClient);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(NotFoundObjectResult));
            var createdResult = result.Result as NotFoundObjectResult;
            var returnedClient = createdResult.Value as ClientCS;
            Assert.IsNull(returnedClient);
        }
    }

}
