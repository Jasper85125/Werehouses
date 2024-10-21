using Newtonsoft.Json;

namespace Services;

public class ClientService : IClientService
{
    private string dataPath = "/data/clients.json";
    public ClientService()
    { 

    }
    
    public List<ClientCS> GetAllClients()
    {
        if (!File.Exists(dataPath))
        {
            return new List<ClientCS>();
        }
        var json = File.ReadAllText(dataPath);
        var clientsdata = JsonConvert.DeserializeObject<List<ClientCS>>(json);
        return clientsdata ?? new List<ClientCS>();
    }
    public ClientCS GetClientById(int id){
        var clients = GetAllClients();
        var result = clients.FirstOrDefault(_ => _.Id == id);
        return result;
        // if (!File.Exists(dataPath))
        // {
        //     return null;
        // }
        // var json = File.ReadAllText(dataPath);
        // var clientsdata = JsonConvert.DeserializeObject<List<ClientCS>>(json);
        // foreach(var client in clientsdata){
        //     if(client.Id == id)
        //     {
        //         return client;
        //     }
        // }
        // return null;
    }

}

