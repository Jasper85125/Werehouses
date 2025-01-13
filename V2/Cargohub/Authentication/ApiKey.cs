using Newtonsoft.Json;

namespace ApiKeyAuthentication.Authentication;
public class ApiKeyModel
{
    public string Key {get; set;}
    public string Role {get; set;}
    public int WarehouseID {get; set;}
}

public class ApiKeyStorage
{
    private static string _path = "../../data/apikeys.json";
    public static List<ApiKeyModel> GetApiKeys()
    {
        if (!File.Exists(_path))
        {
            return new List<ApiKeyModel>();
        }
        var jsonData = File.ReadAllText(_path);
        List<ApiKeyModel> apikeys = JsonConvert.DeserializeObject<List<ApiKeyModel>>(jsonData);
        return apikeys ?? new List<ApiKeyModel>();
    }

    public static void UpdateApiKey(List<ApiKeyModel> apikeys)
    {
        var jsonData = JsonConvert.SerializeObject(apikeys, Formatting.Indented);
        File.WriteAllText(_path, jsonData);
    }
}