using Microsoft.AspNetCore.Mvc;
using ServicesV2;

namespace ControllersV2;

[ApiController]
[Route("api/v2/clients")]
public class ClientController : ControllerBase
{
    private readonly IClientService _clientservice;
    public ActionLogService _actionlogservice;
    public ClientController(IClientService clientservice)
    {
        _clientservice = clientservice;
        _actionlogservice = new ActionLogService();
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

    [HttpGet("latest-actions")]
    public ActionResult<IEnumerable<object>> GetClientsWithLatestActions()
    {
        var userRole = HttpContext.Items["UserRole"]?.ToString();
        List<string> allowedRoles = new List<string>() { "Admin", "Analyst", "Logistics" };

        if (userRole == null || !allowedRoles.Contains(userRole))
        {
            return Unauthorized();
        }

        var suppliers = _clientservice.GetAllClients();
        var actions = _actionlogservice.GetLatestActionsForSuppliers();

        var result = suppliers.Select(supplier => new
        {
            Client = supplier,
            LatestAction = actions.FirstOrDefault(action => action.model == "supplier")
        });

        return Ok(result);
    }
    
    [HttpGet("latest-actions/{amount}")]
    public ActionResult<IEnumerable<object>> GetClientsWithLatestActions([FromRoute] int amount)
    {
        var userRole = HttpContext.Items["UserRole"]?.ToString();
        List<string> allowedRoles = new List<string>() { "Admin", "Analyst", "Logistics" };

        if (userRole == null || !allowedRoles.Contains(userRole))
        {
            return Unauthorized();
        }

        var suppliers = _clientservice.GetAllClients();
        var actions = _actionlogservice.GetLatestActionsForSuppliers();

        var result = suppliers.Select(supplier => new
        {
            Client = supplier,
            LatestAction = actions.FirstOrDefault(action => action.model == "supplier")
        });
        var listed = result.ToList();
        while(listed.Count() > amount){
            listed.Remove(listed.Last());
        }

        return Ok(listed);
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
        
        var actionlogs = _actionlogservice.GetAllActionLogs();
        ActionLogCS actionLog = new ActionLogCS();
        actionLog.performed_by = userRole;
        actionLog.id = actionlogs.Count()  + 1;
        actionLog.model = "client";
        actionLog.action = "client created";
        actionLog.timestamp = DateTime.ParseExact(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "yyyy-MM-dd HH:mm:ss", null);
        actionlogs.Add(actionLog);
        _actionlogservice.SaveActionLogs(actionlogs);

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

        var actionlogs = _actionlogservice.GetAllActionLogs();
        ActionLogCS actionLog = new ActionLogCS();
        actionLog.performed_by = userRole;
        actionLog.id = actionlogs.Count()  + 1;
        actionLog.model = "client";
        actionLog.action = "multiple clients created";
        actionLog.timestamp = DateTime.ParseExact(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "yyyy-MM-dd HH:mm:ss", null);
        actionlogs.Add(actionLog);
        _actionlogservice.SaveActionLogs(actionlogs);

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
        
        var actionlogs = _actionlogservice.GetAllActionLogs();
        ActionLogCS actionLog = new ActionLogCS();
        actionLog.performed_by = userRole;
        actionLog.id = actionlogs.Count()  + 1;
        actionLog.model = "client";
        actionLog.action = "client updated";
        actionLog.timestamp = DateTime.ParseExact(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "yyyy-MM-dd HH:mm:ss", null);
        actionlogs.Add(actionLog);
        _actionlogservice.SaveActionLogs(actionlogs);

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
        
        var actionlogs = _actionlogservice.GetAllActionLogs();
        ActionLogCS actionLog = new ActionLogCS();
        actionLog.performed_by = userRole;
        actionLog.id = actionlogs.Count()  + 1;
        actionLog.model = "client";
        actionLog.action = "client deleted";
        actionLog.timestamp = DateTime.ParseExact(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "yyyy-MM-dd HH:mm:ss", null);
        actionlogs.Add(actionLog);
        _actionlogservice.SaveActionLogs(actionlogs);

        return Ok();
    }

    [HttpDelete("batch")]
    public ActionResult DeleteClients ([FromBody] List<int> ids){
        List<string> listOfAllowedRoles = new List<string>() { "Admin" };
        var userRole = HttpContext.Items["UserRole"]?.ToString();
        if (userRole == null || !listOfAllowedRoles.Contains(userRole))
        {
            return Unauthorized();
        }
        if(ids is null){
            return BadRequest("error in request");
        }

        _clientservice.DeleteClients(ids);
        
        var actionlogs = _actionlogservice.GetAllActionLogs();
        ActionLogCS actionLog = new ActionLogCS();
        actionLog.performed_by = userRole;
        actionLog.id = actionlogs.Count()  + 1;
        actionLog.model = "client";
        actionLog.action = "multiple clients deleted";
        actionLog.timestamp = DateTime.ParseExact(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "yyyy-MM-dd HH:mm:ss", null);
        actionlogs.Add(actionLog);
        _actionlogservice.SaveActionLogs(actionlogs);
        
        return Ok("multiple clients deleted");
    }

    //PATCH: /clients/{id}
    [HttpPatch("{id}/{property}")]
    public ActionResult<ClientCS> PatchClient([FromRoute] int id, [FromRoute] string property, [FromBody] object newvalue)
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

        var result = _clientservice.PatchClient(id, property, newvalue);
        if (result is null)
        {
            return BadRequest("Failed to patch client.");
        }

        var actionlogs = _actionlogservice.GetAllActionLogs();
        ActionLogCS actionLog = new ActionLogCS();
        actionLog.performed_by = userRole;
        actionLog.id = actionlogs.Count()  + 1;
        actionLog.model = "client";
        actionLog.action = "inventory patched";
        actionLog.timestamp = DateTime.ParseExact(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "yyyy-MM-dd HH:mm:ss", null);
        actionlogs.Add(actionLog);
        _actionlogservice.SaveActionLogs(actionlogs);
        return Ok(result);
    }
}
