using Newtonsoft.Json;

namespace ServicesV2;

public class ClientService : IClientService
{
    private string _path = "../../data/clients.json";
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
        var currentDateTime = DateTime.Now;
        var formattedDateTime = currentDateTime.ToString("yyyy-MM-dd HH:mm:ss");

        newClient.Id = clients.Count > 0 ? clients.Max(c => c.Id) + 1 : 1;
        newClient.created_at = DateTime.ParseExact(formattedDateTime, "yyyy-MM-dd HH:mm:ss", null);
        newClient.updated_at = DateTime.ParseExact(formattedDateTime, "yyyy-MM-dd HH:mm:ss", null);
        clients.Add(newClient);

        var jsonData = JsonConvert.SerializeObject(clients, Formatting.Indented);
        File.WriteAllText(_path, jsonData);
        return newClient;
    }

    public List<ClientCS> CreateMultipleClients(List<ClientCS> newClients)
    {
        List<ClientCS> addedClient = new List<ClientCS>();
        foreach (ClientCS client in newClients)
        {
            ClientCS addClient = CreateClient(client);
            addedClient.Add(addClient);
        }
        return addedClient;
    }

    public ClientCS UpdateClient(int id, ClientCS updateClient)
    {
        var currentDateTime = DateTime.Now;

        var formattedDateTime = currentDateTime.ToString("yyyy-MM-dd HH:mm:ss");
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
            clientToUpdate.updated_at = DateTime.ParseExact(formattedDateTime, "yyyy-MM-dd HH:mm:ss", null);

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
    public void DeleteClients(List<int> ids)
    {
        var clients = GetAllClients();
        foreach (int id in ids)
        {
            var client = clients.Find(_ => _.Id == id);
            if (client is not null)
            {
                clients.Remove(client);
            }
        }
        var json = JsonConvert.SerializeObject(clients, Formatting.Indented);
        File.WriteAllText(_path, json);
    }

    public ClientCS PatchClient(int id, string property, object newValue)
    {
        var allClients = GetAllClients();
        var clientToUpdate = allClients.Find(_ => _.Id == id);

        if (clientToUpdate is not null)
        {
            var currentDateTime = DateTime.Now;

            var formattedDateTime = currentDateTime.ToString("yyyy-MM-dd HH:mm:ss");
            switch(property)
            {
                case "name":
                    clientToUpdate.Name = newValue.ToString();
                    break;
                case "address":
                    clientToUpdate.Address = newValue.ToString();
                    break;
                case "city":
                    clientToUpdate.City = newValue.ToString();
                    break;
                case "zip_code":
                    clientToUpdate.zip_code = newValue.ToString();
                    break;
                case "province":
                    clientToUpdate.Province = newValue.ToString();
                    break;
                case "country":
                    clientToUpdate.Country = newValue.ToString();
                    break;
                case "contact_name":
                    clientToUpdate.contact_name = newValue.ToString();
                    break;
                case "contact_phone":
                    clientToUpdate.contact_phone = newValue.ToString();
                    break;
                case "contact_email":
                    clientToUpdate.contact_email = newValue.ToString();
                    break;
            }
            clientToUpdate.updated_at = DateTime.ParseExact(formattedDateTime, "yyyy-MM-dd HH:mm:ss", null);

            var jsonData = JsonConvert.SerializeObject(allClients, Formatting.Indented);
            File.WriteAllText(_path, jsonData);
            return clientToUpdate;
        }
        return null;
    }
}

