using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;
using Microsoft.AspNetCore.Mvc;
using ServicesV2;
using ControllersV2;
using Microsoft.AspNetCore.Http;

namespace clients.TestsV2
{
    [TestClass]
    public class ClientTest
    {
        private Mock<IClientService> _mockClientService;
        private ClientController _clientController;

        [TestInitialize]
        public void Setup()
        {
            _mockClientService = new Mock<IClientService>();
            _clientController = new ClientController(_mockClientService.Object);
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
            _mockClientService.Setup(_ => _.GetAllClients()).Returns(listofclients);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";  // Set the UserRole in HttpContext

            // Assign HttpContext to the controller
            _clientController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            //act
            var result = _clientController.GetAllClients();

            //assert
            var okResult = result.Result as OkObjectResult;
            var returnedItems = okResult.Value as IEnumerable<ClientCS>;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(2, returnedItems.Count());
        }

        [TestMethod]
        public void GetClientById_Test_returns_true()
        {
            //arrange
            var client = new ClientCS() { Id = 1, Address = "", City = "", contact_phone = "", contact_email = "", contact_name = "", Country = "", created_at = default, updated_at = default, Name = "", Province = "", zip_code = "" };
            _mockClientService.Setup(_ => _.GetClientById(client.Id)).Returns(client);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";  // Set the UserRole in HttpContext

            // Assign HttpContext to the controller
            _clientController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            //act
            var result = _clientController.GetClientById(1);

            //assert
            var resultok = result.Result as OkObjectResult;
            Assert.IsNotNull(resultok);
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
        }

        [TestMethod]
        public void CreateClient_ReturnsCreatedResult_WithNewClient()
        {
            // Arrange
            var client = new ClientCS { Address = "street", City = "city", contact_phone = "number", contact_email = "email", contact_name = "name", Country = "Japan", created_at = default, Id = 1, Name = "name", Province = "province", updated_at = default, zip_code = "zip" };

            _mockClientService.Setup(service => service.CreateClient(client)).Returns(client);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";  // Set the UserRole in HttpContext

            // Assign HttpContext to the controller
            _clientController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = _clientController.CreateClient(client);

            // Assert
            var createdResult = result.Result as CreatedAtActionResult;  // Use CreatedAtActionResult here
            Assert.IsNotNull(createdResult);

            var returnedClients = createdResult.Value as ClientCS;
            Assert.IsNotNull(returnedClients);
            Assert.AreEqual(client.Address, returnedClients.Address);
            Assert.AreEqual(client.City, returnedClients.City);
        }

        [TestMethod]
        public void CreateMultipleClient_ReturnsCreatedResult_WithNewClient()
        {
            // Arrange
            var clients = new List<ClientCS>
            {
                new ClientCS { Address="street1", City="city1", contact_phone="number1", contact_email="email1", contact_name="name1",
                               Country="Japan1", Name="name1", Province="province1", zip_code="zip1"},
                new ClientCS { Address="street2", City="city2", contact_phone="number2", contact_email="email2", contact_name="name2",
                               Country="Japan2", Name="name2", Province="province2", zip_code="zip2"}
            };
            _mockClientService.Setup(service => service.CreateMultipleClients(clients)).Returns(clients);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";  // Set the UserRole in HttpContext

            // Assign HttpContext to the controller
            _clientController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = _clientController.CreateMultipleClients(clients);
            var createdResult = result.Result as ObjectResult;
            var returnedItems = createdResult.Value as List<ClientCS>;
            var firstClient = returnedItems[0];

            // Assert
            Assert.IsNotNull(createdResult);
            Assert.IsNotNull(returnedItems);
            Assert.AreEqual(clients[0].Address, firstClient.Address);
            Assert.AreEqual(clients[0].contact_phone, firstClient.contact_phone);
            Assert.AreEqual(clients[0].contact_name, firstClient.contact_name);
        }

        [TestMethod]
        public void UpdatedClientTest_Success()
        {
            // Arrange
            var updatedClient = new ClientCS { Address = "street", City = "city", contact_phone = "number", contact_email = "email", contact_name = "name", Country = "Japan", created_at = default, Id = 1, Name = "name", Province = "province", updated_at = default, zip_code = "zip" };

            _mockClientService.Setup(service => service.UpdateClient(1, updatedClient)).Returns(updatedClient);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";  // Set the UserRole in HttpContext

            // Assign HttpContext to the controller
            _clientController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = _clientController.UpdateClient(1, updatedClient);

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
            var updatedClient = new ClientCS { Address = "street", City = "city", contact_phone = "number", contact_email = "email", contact_name = "name", Country = "Japan", created_at = default, Id = 1, Name = "name", Province = "province", updated_at = default, zip_code = "zip" };

            _mockClientService.Setup(service => service.UpdateClient(0, updatedClient)).Returns((ClientCS)null);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";  // Set the UserRole in HttpContext

            // Assign HttpContext to the controller
            _clientController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = _clientController.UpdateClient(0, updatedClient);

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
            var existingClient = new ClientCS { Address = "street", City = "city", contact_phone = "number", contact_email = "email", contact_name = "name", Country = "Japan", created_at = default, Id = 1, Name = "name", Province = "province", updated_at = default, zip_code = "zip" };
            _mockClientService.Setup(service => service.GetClientById(1)).Returns(existingClient);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";  // Set the UserRole in HttpContext

            // Assign HttpContext to the controller
            _clientController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = _clientController.DeleteClient(1);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkResult));
        }
        [TestMethod]
        public void DeleteClientsTest_Succes()
        {
            //Arrange
            var clientsToDelete = new List<int>() { 1, 2, 3 };

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";  // Set the UserRole in HttpContext

            // Assign HttpContext to the controller
            _clientController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            //Act
            var result = _clientController.DeleteClients(clientsToDelete);
            var resultOK = result as OkObjectResult;

            //Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            Assert.AreEqual(resultOK.StatusCode, 200);
        }
    }

}
