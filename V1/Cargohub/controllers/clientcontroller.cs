using Microsoft.AspNetCore.Mvc;
using Services;

namespace ControllersV1;

[ApiController]
[Route("api/v1/clients")]
public class ClientController : ControllerBase
{
    private readonly IClientService _clientservice;
    public ClientController(IClientService clientservice)
    {
        _clientservice = clientservice;
    }

    // GET: /clients
    [HttpGet()]
    public ActionResult<IEnumerable<ClientCS>> GetAllClients()
    {
        var clients = _clientservice.GetAllClients();
        if (clients is null)
        {
            return NotFound();
        }
        return Ok(clients);
    }

    // GET: /clients/{id}
    [HttpGet("{id}")]
    public ActionResult<ClientCS> GetClientById([FromRoute]int Id)
    {
        var client = _clientservice.GetClientById(Id);
        if (client is null)
        {
            return NotFound();
        }
        return Ok(client);
    }
    
    // POST: /clients
    [HttpPost()]
    public ActionResult<ClientCS> CreateClient([FromBody] ClientCS newClient)
    {
        if (newClient is null)
        {
            return BadRequest("Client data is null");
        }

        var createdClient = _clientservice.CreateClient(newClient);
        return CreatedAtAction(nameof(GetClientById), new { id = createdClient.Id }, createdClient);
    }

    // PUT: /clients/{id}
    [HttpPut("{id}")]
    public ActionResult<ClientCS> UpdateClient([FromRoute]int id, [FromBody] ClientCS client)
    {
        if (client is null)
        {
            return BadRequest("Client data is null.");
        }

        var updatedClient = _clientservice.UpdateClient(id, client);
        if (updatedClient is null)
        {
            return NotFound("No Client found with the given id");
        }
        return Ok(updatedClient);

    }
    [HttpDelete("{id}")]
    public ActionResult DeleteClient([FromRoute]int id)
    {
        var existingClient = _clientservice.GetClientById(id);
        if (existingClient is null)
        {
            return NotFound();
        }
        _clientservice.DeleteClient(id);
        return Ok();
    }
}
