using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;
using Microsoft.AspNetCore.Mvc;
using ServicesV1;
using ControllersV1;

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
            var client = new ClientCS {Address="street", City="city", contact_phone="number", contact_email="email", contact_name="name", Country="Japan", created_at=default, Id=1, Name="name", Province="province", updated_at=default, zip_code="zip"};
    
            _clientservice.Setup(service => service.CreateClient(client)).Returns(client);
            
            // Act
            var result = _clientcontroller.CreateClient(client);
            
            // Assert
            var createdResult = result.Result as CreatedAtActionResult;  // Use CreatedAtActionResult here
            Assert.IsNotNull(createdResult);
            
            var returnedClients = createdResult.Value as ClientCS;
            Assert.IsNotNull(returnedClients);
            Assert.AreEqual(client.Address, returnedClients.Address);
            Assert.AreEqual(client.City, returnedClients.City);
        }

        [TestMethod]
        public void UpdatedClientTest_Success()
        {
            // Arrange
            var updatedClient = new ClientCS {Address="street", City="city", contact_phone="number", contact_email="email", contact_name="name", Country="Japan", created_at=default, Id=1, Name="name", Province="province", updated_at=default, zip_code="zip"};

             _clientservice.Setup(service => service.UpdateClient(1, updatedClient)).Returns(updatedClient);

            // Act
            var result = _clientcontroller.UpdateClient(1, updatedClient);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var createdResult = result.Result as OkObjectResult;
            Assert.IsNotNull(createdResult);
            Assert.IsInstanceOfType(createdResult.Value, typeof(ClientCS));
            var returnedClient = createdResult.Value as ClientCS;
            Assert.AreEqual(updatedClient.City, returnedClient.City);
            Assert.AreEqual(updatedClient.Address, returnedClient.Address);
        }

        [TestMethod]
        public void UpdatedClientTest_Failed()
        {
            // Arrange
            var updatedClient = new ClientCS {Address="street", City="city", contact_phone="number", contact_email="email", contact_name="name", Country="Japan", created_at=default, Id=1, Name="name", Province="province", updated_at=default, zip_code="zip"};

             _clientservice.Setup(service => service.UpdateClient(0, updatedClient)).Returns((ClientCS)null);

            // Act
            var result = _clientcontroller.UpdateClient(0, updatedClient);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(NotFoundObjectResult));
            var createdResult = result.Result as NotFoundObjectResult;
            var returnedClient = createdResult.Value as ClientCS;
            Assert.IsNull(returnedClient);
        }

        [TestMethod]
        public void DeleteClientTest_Success()
        {
            
            // Arrange
            var existingClient = new ClientCS {Address="street", City="city", contact_phone="number", contact_email="email", contact_name="name", Country="Japan", created_at=default, Id=1, Name="name", Province="province", updated_at=default, zip_code="zip"};
            _clientservice.Setup(service => service.GetClientById(1)).Returns(existingClient);
            // Act
            var result = _clientcontroller.DeleteClient(1);
            // Assert
            Assert.IsInstanceOfType(result, typeof(OkResult));
        }
    }

}
