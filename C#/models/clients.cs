using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

public class Client
{
    public int Id { get; set; }
    public string CreatedAt { get; set; }
    public string UpdatedAt { get; set; }
    // Add other client properties here
}

public class Clients : BaseCS
{
    private string dataPath;
    private List<Client> data;

    public Clients(string rootPath, bool isDebug = false)
    {
        dataPath = Path.Combine(rootPath, "clients.json");
        Load(isDebug);
    }

    public List<Client> GetClients()
    {
        return data;
    }

    public Client GetClient(int clientId)
    {
        return data.Find(x => x.Id == clientId);
    }

    public void AddClient(Client client)
    {
        client.CreatedAt = GetTimestamp();
        client.UpdatedAt = GetTimestamp();
        data.Add(client);
    }

    public void UpdateClient(int clientId, Client client)
    {
        client.UpdatedAt = GetTimestamp();
        int index = data.FindIndex(x => x.Id == clientId);
        if (index != -1)
        {
            data[index] = client;
        }
    }

    public void RemoveClient(int clientId)
    {
        data.RemoveAll(x => x.Id == clientId);
    }

    private void Load(bool isDebug)
    {
        if (isDebug)
        {
            data = new List<Client>(); // Replace with actual debug data if needed
        }
        else
        {
            if (File.Exists(dataPath))
            {
                string json = File.ReadAllText(dataPath);
                data = JsonConvert.DeserializeObject<List<Client>>(json);
            }
            else
            {
                data = new List<Client>();
            }
        }
    }

    public void Save()
    {
        string json = JsonConvert.SerializeObject(data, Formatting.Indented);
        File.WriteAllText(dataPath, json);
    }

    private string GetTimestamp()
    {
        return DateTime.UtcNow.ToString("o");
    }
}
