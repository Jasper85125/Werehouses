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

            httpContext.Items["UserRole"] = "Operative";
            _clientController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            //act
            result = _clientController.GetAllClients();

            //assert
            var unauthorizedResult = result.Result as UnauthorizedResult;
            Assert.IsNotNull(unauthorizedResult);
            Assert.AreEqual(401, unauthorizedResult.StatusCode);
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

            httpContext.Items["UserRole"] = "Operative";  // Set the UserRole in HttpContext

            // Assign HttpContext to the controller
            _clientController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            //act
            result = _clientController.GetClientById(1);

            //assert
            var unauthorizedResult = result.Result as UnauthorizedResult;
            Assert.IsNotNull(unauthorizedResult);
            Assert.AreEqual(401, unauthorizedResult.StatusCode);
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

            httpContext.Items["UserRole"] = "Operative";  // Set the UserRole in HttpContext

            // Assign HttpContext to the controller
            _clientController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
            result = _clientController.CreateClient(client);

            var unauthorizedResult = result.Result as UnauthorizedResult;
            Assert.IsNotNull(unauthorizedResult);
            Assert.AreEqual(401, unauthorizedResult.StatusCode);
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

            httpContext.Items["UserRole"] = "Operative";  // Set the UserRole in HttpContext

            // Assign HttpContext to the controller
            _clientController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
            result = _clientController.CreateMultipleClients(clients);

            var unauthorizedResult = result.Result as UnauthorizedResult;
            Assert.IsNotNull(unauthorizedResult);
            Assert.AreEqual(401, unauthorizedResult.StatusCode);
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

            httpContext.Items["UserRole"] = "Operative";  // Set the UserRole in HttpContext

            // Assign HttpContext to the controller
            _clientController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
            result = _clientController.UpdateClient(1, updatedClient);

            var unauthorizedResult = result.Result as UnauthorizedResult;
            Assert.IsNotNull(unauthorizedResult);
            Assert.AreEqual(401, unauthorizedResult.StatusCode);
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

            httpContext.Items["UserRole"] = "Operative";  // Set the UserRole in HttpContext

            // Assign HttpContext to the controller
            _clientController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
            result = _clientController.DeleteClient(1);

            var unauthorizedResult = result as UnauthorizedResult;
            Assert.IsNotNull(unauthorizedResult);
            Assert.AreEqual(401, unauthorizedResult.StatusCode);
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

            httpContext.Items["UserRole"] = "Operative";  // Set the UserRole in HttpContext

            // Assign HttpContext to the controller
            _clientController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
            var resultUn = _clientController.DeleteClients(clientsToDelete);

            var unauthorizedResult = resultUn as UnauthorizedResult;
            Assert.AreEqual(401, unauthorizedResult.StatusCode);
        }

        [TestMethod]
        public void PatchClientTest_Success()
        {
            // Arrange
            var existingClient = new ClientCS { Id = 1, Address = "old street", City = "old city", contact_phone = "old number", contact_email = "old email", contact_name = "old name", Country = "old country", Name = "old name", Province = "old province", zip_code = "old zip" };
            var patchClient = new ClientCS { Address = "new street",City = "old city", contact_phone = "old number", contact_email = "old email", contact_name = "old name", Country = "old country", Name = "old name", Province = "old province", zip_code = "old zip" };

            _mockClientService.Setup(service => service.GetClientById(1)).Returns(existingClient);
            _mockClientService.Setup(service => service.PatchClient(1, "address", "new street")).Returns(patchClient);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";  // Set the UserRole in HttpContext

            // Assign HttpContext to the controller
            _clientController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = _clientController.PatchClient(1, "address", "new street");
            var resultOk = result.Result as OkObjectResult;
            var returnedClient = resultOk.Value as ClientCS;

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            Assert.IsNotNull(resultOk);
            Assert.IsNotNull(returnedClient);
            Assert.AreEqual(patchClient.Address, returnedClient.Address);
            Assert.AreEqual(patchClient.City, returnedClient.City);

            httpContext.Items["UserRole"] = "Operative";  // Set the UserRole in HttpContext

            // Assign HttpContext to the controller
            _clientController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
            result = _clientController.PatchClient(1, "address", "new street");
            
            var unauthorizedResult = result.Result as UnauthorizedResult;
            Assert.IsNotNull(unauthorizedResult);
            Assert.AreEqual(401, unauthorizedResult.StatusCode);
        }

        [TestMethod]
        public void PatchClientTest_NotFound()
        {
            // Arrange
            var patchClient = new ClientCS { Address = "new street", City = "new city", contact_phone = "new number", contact_email = "new email", contact_name = "new name", Country = "new country", Name = "new name", Province = "new province", zip_code = "new zip" };

            _mockClientService.Setup(service => service.GetClientById(1)).Returns((ClientCS)null);

            var httpContext = new DefaultHttpContext();
            httpContext.Items["UserRole"] = "Admin";  // Set the UserRole in HttpContext

            // Assign HttpContext to the controller
            _clientController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = _clientController.PatchClient(1, "address", "new street");

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));
        }
    }
}
