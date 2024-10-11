using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

public class ClientCS
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Address { get; set; }
    public string City { get; set; }
    public string ZipCode { get; set; }
    public string Province { get; set; }
    public string Country { get; set; }
    public string ContactName { get; set; }
    public string ConactPhone { get; set; }
    public string ContactEmail { get; set; }
    public string CreatedAt { get; set; }
    public string UpdatedAt { get; set; }
}

public class ClientsCS : BaseCS
{
    private string dataPath;
    private List<ClientCS> data;

    public ClientsCS(string rootPath, bool isDebug = false)
    {
        dataPath = Path.Combine(rootPath, "clients.json");
        LoadCS(isDebug);
    }

    public List<ClientCS> GetClientsCS()
    {
        return data;
    }

    public ClientCS GetClientCS(int clientId)
    {
        return data.Find(x => x.Id == clientId);
    }

    public void AddClientCS(ClientCS client)
    {
        client.CreatedAt = GetTimestampCS();
        client.UpdatedAt = GetTimestampCS();
        data.Add(client);
    }

    public void UpdateClientCS(int clientId, ClientCS client)
    {
        client.UpdatedAt = GetTimestampCS();
        int index = data.FindIndex(x => x.Id == clientId);
        if (index != -1)
        {
            data[index] = client;
        }
    }

    public void RemoveClientCS(int clientId)
    {
        data.RemoveAll(x => x.Id == clientId);
    }

    private void LoadCS(bool isDebug)
    {
        if (isDebug)
        {
            data = new List<ClientCS>(); // Replace with actual debug data if needed
        }
        else
        {
            if (File.Exists(dataPath))
            {
                string json = File.ReadAllText(dataPath);
                data = JsonConvert.DeserializeObject<List<ClientCS>>(json);
            }
            else
            {
                data = new List<ClientCS>();
            }
        }
    }

    public void SaveCS()
    {
        string json = JsonConvert.SerializeObject(data, Formatting.Indented);
        File.WriteAllText(dataPath, json);
    }
}
