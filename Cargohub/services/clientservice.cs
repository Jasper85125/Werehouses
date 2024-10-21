using Newtonsoft.Json;

namespace Services;

public class ClientService : IClientService
{
    public ClientService()
    { 

    }
    
    public List<ClientCS> GetAllClients()
    {
        var dataPath = "/data/clients.json";
        if (!File.Exists(dataPath))
        {
            return new List<ClientCS>();
        }
        var json = File.ReadAllText(dataPath);
        var clientsdata = JsonConvert.DeserializeObject<List<ClientCS>>(json);
        return clientsdata ?? new List<ClientCS>();
    }
}

