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
    public ActionResult<IEnumerable<ClientCS>> GetAllClients()
    {
        var clients = _clientservice.GetAllClients();
        return Ok(clients);
    }
}
