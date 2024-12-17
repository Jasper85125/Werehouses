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
        // Log incoming form data for debugging
    var form = HttpContext.Request.Form;
    foreach (var key in form.Keys)
    {
        Console.WriteLine($"Key: {key}, Value: {form[key]}");
    }

    // Check for file binding issues
    if (file == null)
    {
        return BadRequest(new { error = "File is not being recognized. Ensure the key is 'file'." });
    }

    return Ok("File successfully received.");
        // // Allowed roles
        // List<string> listOfAllowedRoles = new List<string>() { "Admin" };
        // var userRole = HttpContext.Items["UserRole"]?.ToString();

        // // Authorization check
        // if (userRole == null || !listOfAllowedRoles.Contains(userRole))
        // {
        //     return Unauthorized("You are not authorized to upload data.");
        // }

        // // Check if the file is provided
        // if (file == null || file.Length == 0)
        // {
        //     return BadRequest(new { error = "The file field is required and cannot be empty." });
        // }

        // try
        // {
        //     // Ensure it's saved as .json
        //     var saveFileName = Path.ChangeExtension(file.FileName, ".json");
        //     var path = Path.Combine(Directory.GetCurrentDirectory(), "Data", saveFileName);

        //     // Save the file
        //     using (var stream = new FileStream(path, FileMode.Create))
        //     {
        //         file.CopyTo(stream);
        //     }

        //     return Ok(new { message = "File uploaded and saved successfully as JSON.", fileName = saveFileName });
        // }
        // catch (Exception ex)
        // {
        //     return StatusCode(500, new { error = "An error occurred while saving the file.", details = ex.Message });
        // }
    }

}