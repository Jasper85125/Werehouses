using Microsoft.AspNetCore.Mvc;
using ServicesV2;

namespace ControllersV2;

[ApiController]
[Route("api/v2/clients")]
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
        List<string> listOfAllowedRoles = new List<string>() { "Admin", "Warehouse Manager", "Inventory Manager", "Floor Manager", "Sales", "Analyst", "Logistics" };
        var userRole = HttpContext.Items["UserRole"]?.ToString();

        if (userRole == null || !listOfAllowedRoles.Contains(userRole))
        {
            return Unauthorized();
        }

        var clients = _clientservice.GetAllClients();
        if (clients is null)
        {
            return NotFound();
        }
        return Ok(clients);
    }

    // GET: /clients/{id}
    [HttpGet("{id}")]
    public ActionResult<ClientCS> GetClientById([FromRoute] int Id)
    {
        List<string> listOfAllowedRoles = new List<string>() { "Admin", "Warehouse Manager", "Inventory Manager", "Floor Manager", "Sales", "Analyst", "Logistics" };
        var userRole = HttpContext.Items["UserRole"]?.ToString();

        if (userRole == null || !listOfAllowedRoles.Contains(userRole))
        {
            return Unauthorized();
        }

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
        List<string> listOfAllowedRoles = new List<string>() { "Admin", "Warehouse Manager", "Sales", "Logistics" };
        var userRole = HttpContext.Items["UserRole"]?.ToString();

        if (userRole == null || !listOfAllowedRoles.Contains(userRole))
        {
            return Unauthorized();
        }

        if (newClient is null)
        {
            return BadRequest("Client data is null");
        }

        var createdClient = _clientservice.CreateClient(newClient);
        return CreatedAtAction(nameof(GetClientById), new { id = createdClient.Id }, createdClient);
    }

    // POST: /clients/multiple
    [HttpPost("multiple")]
    public ActionResult<ClientCS> CreateMultipleClients([FromBody] List<ClientCS> newClient)
    {
        List<string> listOfAllowedRoles = new List<string>() { "Admin", "Warehouse Manager", "Sales", "Logistics" };
        var userRole = HttpContext.Items["UserRole"]?.ToString();

        if (userRole == null || !listOfAllowedRoles.Contains(userRole))
        {
            return Unauthorized();
        }

        if (newClient is null)
        {
            return BadRequest("Client data is null");
        }

        var createdClient = _clientservice.CreateMultipleClients(newClient);
        return StatusCode(StatusCodes.Status201Created, createdClient);
    }

    // PUT: /clients/{id}
    [HttpPut("{id}")]
    public ActionResult<ClientCS> UpdateClient([FromRoute] int id, [FromBody] ClientCS client)
    {
        List<string> listOfAllowedRoles = new List<string>() { "Admin", "Warehouse Manager", "Sales", "Logistics" };
        var userRole = HttpContext.Items["UserRole"]?.ToString();

        if (userRole == null || !listOfAllowedRoles.Contains(userRole))
        {
            return Unauthorized();
        }

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
    public ActionResult DeleteClient([FromRoute] int id)
    {
        List<string> listOfAllowedRoles = new List<string>() { "Admin" };
        var userRole = HttpContext.Items["UserRole"]?.ToString();

        if (userRole == null || !listOfAllowedRoles.Contains(userRole))
        {
            return Unauthorized();
        }

        var existingClient = _clientservice.GetClientById(id);
        if (existingClient is null)
        {
            return NotFound();
        }
        _clientservice.DeleteClient(id);
        return Ok();
    }

    [HttpDelete("batch")]
    public ActionResult DeleteClients([FromBody] List<int> ids)
    {
        List<string> listOfAllowedRoles = new List<string>() { "Admin" };
        var userRole = HttpContext.Items["UserRole"]?.ToString();

        if (userRole == null || !listOfAllowedRoles.Contains(userRole))
        {
            return Unauthorized();
        }
        
        if (ids is null)
        {
            return BadRequest("error in request");
        }
        _clientservice.DeleteClients(ids);
        return Ok("multiple clients deleted");
    }

    //PATCH: /clients/{id}
    [HttpPatch("{id}")]
    public ActionResult<ClientCS> PatchClient([FromRoute] int id, [FromQuery] string property, [FromBody] object newvalue)
    {
        List<string> listOfAllowedRoles = new List<string>() { "Admin", "Warehouse Manager", "Sales", "Logistics" };
        var userRole = HttpContext.Items["UserRole"]?.ToString();

        if (userRole == null || !listOfAllowedRoles.Contains(userRole))
        {
            return Unauthorized();
        }

        var client = _clientservice.GetClientById(id);
        if (client is null)
        {
            return NotFound();
        }

        var updatedClient = _clientservice.PatchClient(id, property, newvalue);
        if (updatedClient is null)
        {
            return BadRequest("Failed to patch client.");
        }

        return Ok(updatedClient);
    }
}
