using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Newtonsoft.Json;

namespace ServicesV2;

public class ClientService : IClientService
{
    private string _path = "data/clients.json";
    private readonly Iactionlogservice _actionlogservice;
    public ClientService(ActionLogService actionLogService)
    {
        _actionlogservice = actionLogService;
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

        // Format the date and time to the desired format
        var formattedDateTime = currentDateTime.ToString("yyyy-MM-dd HH:mm:ss");
        var allClients = GetAllClients();
        var clientToPatch = allClients.Single(client => client.Id == id);

        if (clientToPatch is not null)
        {
            clientToPatch.Name = updateClient.Name;
            clientToPatch.Address = updateClient.Address;
            clientToPatch.City = updateClient.City;
            clientToPatch.zip_code = updateClient.zip_code;
            clientToPatch.Province = updateClient.Province;
            clientToPatch.Country = updateClient.Country;
            clientToPatch.contact_name = updateClient.contact_name;
            clientToPatch.contact_phone = updateClient.contact_phone;
            clientToPatch.contact_email = updateClient.contact_email;
            clientToPatch.updated_at = DateTime.ParseExact(formattedDateTime, "yyyy-MM-dd HH:mm:ss", null);

            var jsonData = JsonConvert.SerializeObject(allClients, Formatting.Indented);
            File.WriteAllText(_path, jsonData);
            return clientToPatch;
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
        File.WriteAllText("data/clients.json", json);
    }

    public ClientCS PatchClient(int id, string property, object newvalue, string userRole)
    {
        var actionlogs = _actionlogservice.GetLatestActionsForClients();
        var actionlog = actionlogs.Find(_=>_.id == id) ?? new ActionLogCS();
        actionlog.id = actionlogs.Count() + 1;
        actionlog.performed_by = userRole;
        actionlog.model = "supplier";

        var clients = GetAllClients();
        var clientToPatch = clients.Find(_=>_.Id == id);
        
        if (clientToPatch is not null)
        {
            // Get the current date and time
            var currentDateTime = DateTime.Now;

            // Format the date and time to the desired format
            var formattedDateTime = currentDateTime.ToString("yyyy-MM-dd HH:mm:ss");
            switch (property)
            {
                case"Name":
                    clientToPatch.Name = newvalue.ToString();
                    actionlog.action = "changed Name";
                break;
                case"Address":
                    clientToPatch.Address = newvalue.ToString();
                    actionlog.action = "changed Address";
                break;
                case"City":
                    clientToPatch.City = newvalue.ToString();
                    actionlog.action = "changed City";
                break;
                case"zip_code":
                    clientToPatch.zip_code = newvalue.ToString();
                    actionlog.action = "changed zip code";
                break;
                case"Province":
                    clientToPatch.Province = newvalue.ToString();
                    actionlog.action = "changed province";
                break;
                case"Country":
                    clientToPatch.Country = newvalue.ToString();
                    actionlog.action = "changed Country";
                break;
                case"contact_name":
                    clientToPatch.contact_name = newvalue.ToString();
                    actionlog.action = "changed contact name";
                break;
                case"contact_phone":
                    clientToPatch.contact_phone = newvalue.ToString();
                    actionlog.action = "changed contact phone";
                break;
                case"contact_email":
                    actionlog.action = "changed contact email";
                    clientToPatch.contact_email = newvalue.ToString();
                break;
                default:
                break;
            }

            clientToPatch.updated_at = DateTime.ParseExact(formattedDateTime, "yyyy-MM-dd HH:mm:ss", null);
            actionlog.timestamp = DateTime.ParseExact(formattedDateTime, "yyyy-MM-dd HH:mm:ss", null);

            var jsonData = JsonConvert.SerializeObject(clients, Formatting.Indented);
            File.WriteAllText(_path, jsonData);

            _actionlogservice.SaveActionLogs(actionlogs);

            return clientToPatch;
        }
        return null;
    }
}

