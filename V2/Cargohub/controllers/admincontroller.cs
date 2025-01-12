using Microsoft.AspNetCore.Mvc;
using ServicesV2;
using System.Linq;

namespace ControllersV2;

[ApiController]
[Route("api/v2/admin")]
public class AdminController : ControllerBase
{
    private readonly IAdminService _adminservice;
    public AdminController(IAdminService adminservice)
    {
        _adminservice = adminservice;
    }

    // POST: /AddData. this is a function that is used to add data to the data folder. it can recieve either be a .json or .csv file. but when the function is going to save the file, it will save it as a .json file.
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

    // a function that returns a requested file from the data folder
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

        // Return file directly with correct content disposition
        var fileType = "application/octet-stream"; // Generic binary file type for downloads
        return PhysicalFile(path, fileType, filename);
    }

    // GET: /GenerateReport. Generates a summary report based on JSON data.
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
}
