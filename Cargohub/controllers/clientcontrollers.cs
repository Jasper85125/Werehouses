using Microsoft.AspNetCore.Mvc;
using Services;

namespace Controllers;

[Route("clients")]
[ApiController]
public class ClientController : ControllerBase
{
<<<<<<< HEAD
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
        public IActionResult GetClientById(int id){
            var client = _clientservice.GetClientById(id);
            return Ok(client);
        }
=======
    private readonly IClientService _clientservice;
    public ClientController(IClientService clientservice)
    {
        _clientservice = clientservice;
>>>>>>> fdefea12b06a68d4eb09b653045d919bc6884c83
    }
    public ActionResult<IEnumerable<ClientCS>> GetAllClients()
    {
        var clients = _clientservice.GetAllClients();
        return Ok(clients);
    }
}
