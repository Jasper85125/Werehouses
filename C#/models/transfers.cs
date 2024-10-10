using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

public class Transfers : Base
{
    private string dataPath;
    private List<Dictionary<string, object>> data;

    public Transfers(string rootPath, bool isDebug = false)
    {
        dataPath = Path.Combine(rootPath, "transfers.json");
        Load(isDebug);
    }

    public List<Dictionary<string, object>> GetTransfers()
    {
        return data;
    }

    public Dictionary<string, object> GetTransfer(string transferId)
    {
        foreach (var transfer in data)
        {
            if (transfer["id"].ToString() == transferId)
            {
                return transfer;
            }
        }
        return null;
    }

    public List<object> GetItemsInTransfer(string transferId)
    {
        foreach (var transfer in data)
        {
            if (transfer["id"].ToString() == transferId)
            {
                return (List<object>)transfer["items"];
            }
        }
        return null;
    }

    public void AddTransfer(Dictionary<string, object> transfer)
    {
        transfer["transfer_status"] = "Scheduled";
        transfer["created_at"] = GetTimestamp();
        transfer["updated_at"] = GetTimestamp();
        data.Add(transfer);
    }

    public void UpdateTransfer(string transferId, Dictionary<string, object> transfer)
    {
        transfer["updated_at"] = GetTimestamp();
        for (int i = 0; i < data.Count; i++)
        {
            if (data[i]["id"].ToString() == transferId)
            {
                data[i] = transfer;
                break;
            }
        }
    }

    public void RemoveTransfer(string transferId)
    {
        data.RemoveAll(x => x["id"].ToString() == transferId);
    }

    private void Load(bool isDebug)
    {
        if (isDebug)
        {
            data = new List<Dictionary<string, object>>();
        }
        else
        {
            using (StreamReader r = new StreamReader(dataPath))
            {
                string json = r.ReadToEnd();
                data = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(json);
            }
        }
    }

    public void Save()
    {
        using (StreamWriter w = new StreamWriter(dataPath))
        {
            string json = JsonConvert.SerializeObject(data, Formatting.Indented);
            w.Write(json);
        }
    }

    private string GetTimestamp()
    {
        return DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
    }
}
