using Microsoft.AspNetCore.Mvc;
namespace clients.Controllers
{
    [Route("clients")]
    [ApiController]
    public class ClientController : ControllerBase{
        private readonly Iclientservice _clientservice;
        public ClientController(Iclientservice clientservice){
            _clientservice = clientservice;
        }
        public IActionResult GetAllClients()
        {
            var clients = _clientservice.GetAllClients();
            return Ok(clients);
        }
    }
}