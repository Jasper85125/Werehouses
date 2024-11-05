using Newtonsoft.Json;

namespace Services;

public class ClientService : IClientService
{
    private string _path = "data/clients.json";
    public ClientService()
    {

    }

    public List<ClientCS> GetAllClients()
    {
        if (!File.Exists(_path))
        {
            return new List<ClientCS>();
        }
        var json = File.ReadAllText(_path);
        var clientsdata = JsonConvert.DeserializeObject<List<ClientCS>>(json);
        return clientsdata ?? new List<ClientCS>();
    }

    public ClientCS GetClientById(int id)
    {
        List<ClientCS> clients = GetAllClients();
        ClientCS client = clients.FirstOrDefault(cli => cli.Id == id);
        return client;
    }

    public ClientCS CreateClient(ClientCS newClient)
    {
        List<ClientCS> clients = GetAllClients();

        newClient.Id = clients.Count > 0 ? clients.Max(c => c.Id) + 1 : 1;
        clients.Add(newClient);

        var jsonData = JsonConvert.SerializeObject(clients, Formatting.Indented);
        File.WriteAllText(_path, jsonData);
        return newClient;
    }

    public ClientCS UpdateClient(int id, ClientCS updateClient)
    {
        var allClients = GetAllClients();
        var clientToUpdate = allClients.Single(client => client.Id == id);

        if (clientToUpdate is not null)
        {
            clientToUpdate.Name = updateClient.Name;
            clientToUpdate.Address = updateClient.Address;
            clientToUpdate.City = updateClient.City;
            clientToUpdate.zip_code = updateClient.zip_code;
            clientToUpdate.Province = updateClient.Province;
            clientToUpdate.Country = updateClient.Country;
            clientToUpdate.contact_name = updateClient.contact_name;
            clientToUpdate.contact_phone = updateClient.contact_phone;
            clientToUpdate.contact_email = updateClient.contact_email;
            clientToUpdate.updated_at = DateTime.UtcNow;

            var jsonData = JsonConvert.SerializeObject(allClients, Formatting.Indented);
            File.WriteAllText(_path, jsonData);
            return clientToUpdate;
        }
        return null;
    }

    public void DeleteClient(int id)
    {
        var clients = GetAllClients();
        var client = clients.FirstOrDefault(c => c.Id == id);
        if (client == null)
        {
            return;
        }
        clients.Remove(client);
        var jsonData = JsonConvert.SerializeObject(clients, Formatting.Indented);
        File.WriteAllText(_path, jsonData);
        
    }
}

