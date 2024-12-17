using Microsoft.AspNetCore.Mvc;
using ServicesV2;

namespace ControllersV2;

[ApiController]
[Route("api/v2/admin")]
public class AdminController : ControllerBase
{
    // private readonly IAdminService _adminservice;
    // public AdminController(IAdminService adminservice)
    // {
    //     _adminservice = adminservice;
    // }
    public AdminController()
    {
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

        try
        {
            // Read CSV content
            using var reader = new StreamReader(file.OpenReadStream());
            var csvContent = reader.ReadToEnd();

            // Parse CSV into JSON format
            var lines = csvContent.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            var headers = lines[0].Split(',');

            var clients = new List<Dictionary<string, string>>();

            for (int i = 1; i < lines.Length; i++)
            {
                var values = lines[i].Split(',');
                var client = new Dictionary<string, string>();

                for (int j = 0; j < headers.Length; j++)
                {
                    client[headers[j].Trim()] = values[j].Trim();
                }

                clients.Add(client);
            }

            var jsonContent = System.Text.Json.JsonSerializer.Serialize(clients, new System.Text.Json.JsonSerializerOptions
            {
                WriteIndented = true
            });

            // Save as JSON file
            var saveFileName = Path.ChangeExtension(file.FileName, ".json");
            var path = Path.Combine(Directory.GetCurrentDirectory(), "Data", saveFileName);

            System.IO.File.WriteAllText(path, jsonContent);

            return Ok(new { message = "File uploaded, converted, and saved successfully as JSON.", fileName = saveFileName });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An error occurred while processing the file.", details = ex.Message });
        }
    }

}