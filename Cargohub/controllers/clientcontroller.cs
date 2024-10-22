using Microsoft.AspNetCore.Mvc;
using Services;

namespace Controllers;

[Route("clients")]
[ApiController]
public class ClientController : ControllerBase
{
    private readonly IClientService _clientservice;
    
    public ClientController(IClientService clientservice)
    {
        _clientservice = clientservice;
    }
    [HttpPost("create")]
    public ActionResult<ClientCS> CreateClient([FromBody] ClientCS newClient)
    {
        var newclient = _clientservice.CreateClient(newClient);
        return Ok(newClient);
    }
    [HttpGet("{clientId}")]
    public ActionResult<ClientCS> GetClientById(int clientId)
    {
        var client = _clientservice.GetClientById(clientId);
        return Ok(client);
    }
    [HttpGet()]
    public ActionResult<IEnumerable<ClientCS>> GetAllClients()
    {
        var clients = _clientservice.GetAllClients();
        return Ok(clients);
    }

}
