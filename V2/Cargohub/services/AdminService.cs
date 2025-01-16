using Newtonsoft.Json;

namespace ServicesV2;

public class AdminService : IAdminService
{
    
    public AdminService()
    {
    }

    public string AddData(IFormFile file)
    {
        var path = Path.Combine(Directory.GetCurrentDirectory(),"..","..", "data", file.FileName);

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
        var saveFileName = Path.Combine(Directory.GetCurrentDirectory(),"..","..", "Data", Path.ChangeExtension(file.FileName, ".json"));
        System.IO.File.WriteAllText(saveFileName, jsonContent);
        return saveFileName;
    }

    public string GenerateReport()
    {
        var dataDirectory = Path.Combine(Directory.GetCurrentDirectory(),"..","..", "data");

        if (!Directory.Exists(dataDirectory))
        {
            throw new Exception("Data directory does not exist.");
        }

        var jsonFiles = Directory.GetFiles(dataDirectory, "*.json");

        int totalClients = 0;
        int totalProducts = 0;
        int totalOrders = 0;
        int ordersDelivered = 0;
        int shipmentsPending = 0;

        foreach (var file in jsonFiles)
        {
            var content = File.ReadAllText(file);

            if (file.Contains("clients"))
            {
                var clients = JsonConvert.DeserializeObject<List<dynamic>>(content);
                totalClients += clients?.Count ?? 0;
            }
            else if (file.Contains("inventories"))
            {
                var inventories = JsonConvert.DeserializeObject<List<dynamic>>(content);
                totalProducts += inventories?.Count ?? 0;
            }
            else if (file.Contains("orders"))
            {
                var orders = JsonConvert.DeserializeObject<List<dynamic>>(content);
                totalOrders += orders?.Count ?? 0;
                ordersDelivered += orders?.Count(o => o.order_status == "Delivered") ?? 0;
            }
            else if (file.Contains("shipments"))
            {
                var shipments = JsonConvert.DeserializeObject<List<dynamic>>(content);
                shipmentsPending += shipments?.Count(s => s.shipment_status == "Pending") ?? 0;
            }
        }

        var report = $"Warehouse Report - {DateTime.Now:yyyy-MM-dd}\n\n" +
                     $"Total Clients: {totalClients}\n" +
                     $"Total Products: {totalProducts}\n" +
                     $"Total Orders: {totalOrders}\n" +
                     $"Orders Delivered: {ordersDelivered}\n" +
                     $"Shipments Pending: {shipmentsPending}\n\n";

        return report;
    }
}
