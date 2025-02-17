using Microsoft.AspNetCore.Mvc;
using ServicesV2;
using System.Linq;

namespace ControllersV2;


[ApiExplorerSettings(IgnoreApi = true)]
[ApiController]
[Route("api/v2/admin")]
public class AdminController : ControllerBase
{
    private readonly IAdminService _adminservice;
    ApiKeyStorage _apikeystorage;
    public AdminController(IAdminService adminservice)
    {
        _adminservice = adminservice;
        _apikeystorage = new ApiKeyStorage();
    }

    // POST: 
    [HttpPost("AddData")]
    public IActionResult AddData([FromForm] IFormFile file)
    {
        // Allowed roles
        List<string> listOfAllowedRoles = new List<string>() { "Admin" };
        var userRole = HttpContext.Items["UserRole"]?.ToString();

        // Authorization check
        if (userRole == null || !listOfAllowedRoles.Contains(userRole))
        {
            return Unauthorized("You are not authorized to upload data.");
        }

        // Check if the file is provided
        if (file == null || file.Length == 0)
        {
            return BadRequest(new { error = "The file field is required and cannot be empty." });
        }

        if (Path.GetExtension(file.FileName) != ".json" && Path.GetExtension(file.FileName) != ".csv")
        {
            return BadRequest(new { error = "The file must be either a .json or .csv file." });
        }

        try
        {
            var filename = _adminservice.AddData(file);

            return Ok(new { message = "File uploaded, converted, and saved successfully as JSON.", filename });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An error occurred while processing the file.", details = ex.Message });
        }
    }

    // GET: /GetData/{filename}.
    [HttpGet("GetData/{filename}")]
    public IActionResult GetData(string filename)
    {
        // Allowed roles
        List<string> listOfAllowedRoles = new List<string>() { "Admin" };
        var userRole = HttpContext.Items["UserRole"]?.ToString();

        // Authorization check
        if (userRole == null || !listOfAllowedRoles.Contains(userRole))
        {
            return Unauthorized("You are not authorized to download data.");
        }

        // Construct file path
        var path = Path.Combine(Directory.GetCurrentDirectory(), "Data", filename);

        // Check if file exists
        if (!System.IO.File.Exists(path))
        {
            return NotFound(new { error = "The requested file does not exist." });
        }

        var fileType = "application/octet-stream";
        return PhysicalFile(path, fileType, filename);
    }

    // GET: /GenerateReport.
    [HttpGet("GenerateReport")]
    public IActionResult GenerateReport()
    {
        // Allowed roles
        List<string> listOfAllowedRoles = new List<string>() { "Admin" };
        var userRole = HttpContext.Items["UserRole"]?.ToString();

        // Authorization check
        if (userRole == null || !listOfAllowedRoles.Contains(userRole))
        {
            return Unauthorized("You are not authorized to generate reports.");
        }

        try
        {
            var reportContent = _adminservice.GenerateReport();
            var reportFileName = $"WarehouseReport_{DateTime.Now:yyyyMMddHHmmss}.txt";
            var reportPath = Path.Combine(Directory.GetCurrentDirectory(), "Reports", reportFileName);

            // Ensure the Reports directory exists
            Directory.CreateDirectory(Path.GetDirectoryName(reportPath));
            System.IO.File.WriteAllText(reportPath, reportContent);

            return Ok(new { message = "Report generated successfully.", reportFileName });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An error occurred while generating the report.", details = ex.Message });
        }
    }

    // GET: /GetAPIKeys.
    [HttpPost("AddAPIKeys")]
    public IActionResult AddAPIKeys([FromBody]ApiKeyModel ApiKey)
    {
        // Allowed roles
        List<string> listOfAllowedRoles = new List<string>() { "Admin" };
        var userRole = HttpContext.Items["UserRole"]?.ToString();

        // Authorization check
        if (userRole == null || !listOfAllowedRoles.Contains(userRole))
        {
            return Unauthorized("You are not authorized to add API keys.");
        }

        try
        {
            var newKey = _adminservice.AddAPIKeys(ApiKey);
            if (newKey == null)
            {
                return BadRequest(new { error = "API Key addition failed" });
            }
            return Ok(newKey);
        }
        catch (Exception ex)
        {
            return StatusCode(400, new { error = "An error occurred while adding the API keys.", details = ex.Message });
        }
    }


    [HttpPut("UpdateAPIKeys")]
    public IActionResult UpdateAPIKeys([FromQuery]string ApiKey,[FromBody]ApiKeyModel NewApiKey)
    {
        // Allowed roles
        List<string> listOfAllowedRoles = new List<string>() { "Admin" };
        var userRole = HttpContext.Items["UserRole"]?.ToString();

        // Authorization check
        if (userRole == null || !listOfAllowedRoles.Contains(userRole))
        {
            return Unauthorized("You are not authorized to update API keys.");
        }

        try
        {
            var newKey = _adminservice.UpdateAPIKeys(ApiKey, NewApiKey);
            if (newKey == null)
            {
                return BadRequest(new { error = "API Key update failed" });
            }
            return Ok(newKey);
        }
        catch (Exception ex)
        {
            return StatusCode(400, new { error = "An error occurred while updating the API keys.", details = ex.Message });
        }
    }


    [HttpDelete("DeleteAPIKeys")]
    public IActionResult DeleteAPIKeys([FromQuery]string ApiKey)
    {
        // Allowed roles
        List<string> listOfAllowedRoles = new List<string>() { "Admin" };
        var userRole = HttpContext.Items["UserRole"]?.ToString();

        // Authorization check
        if (userRole == null || !listOfAllowedRoles.Contains(userRole))
        {
            return Unauthorized("You are not authorized to delete API keys.");
        }

        try
        {
            var deletedKey = _adminservice.DeleteAPIKeys(ApiKey);
            if (deletedKey == null)
            {
                return BadRequest(new { error = "API Key deletion failed" });
            }
            return Ok(new { message = "API Key deleted successfully", deletedKey });
        }
        catch (Exception ex)
        {
            return StatusCode(400, new { error = "An error occurred while deleting the API keys.", details = ex.Message });
        }
    }


}
