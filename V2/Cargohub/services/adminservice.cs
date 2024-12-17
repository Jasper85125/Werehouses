using Newtonsoft.Json;

namespace ServicesV2;

public class AdminService : IAdminService
{
    public AdminService()
    {
    }
    public string AddData(IFormFile file)
    {
        
        var path = Path.Combine(Directory.GetCurrentDirectory(), "Data", file.FileName);

        if (Path.GetExtension(file.FileName) == ".json")
        {

            using (var stream = new FileStream(path, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            return file.FileName;
        }
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
        var saveFileName = Path.Combine(Directory.GetCurrentDirectory(), "Data", Path.ChangeExtension(file.FileName, ".json"));
        System.IO.File.WriteAllText(saveFileName, jsonContent);
        return saveFileName;
    }

}
