using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

public class TransferCS
{
    public int Id { get; set;}
    public string? Reference { get; set; }
    public int TransferFrom { get; set; }
    public int TransferTo { get; set; }
    public string? TransferStatus { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<ItemCS> Items { get; set; }
}
public class TransfersCS : BaseCS
{
    private string dataPath;
    private List<Dictionary<string, object>> data;

    public TransfersCS(string rootPath, bool isDebug = false)
    {
        dataPath = Path.Combine(rootPath, "transfers.json");
        LoadCS(isDebug);
    }

    public List<Dictionary<string, object>> GetTransfersCS()
    {
        return data;
    }

    public Dictionary<string, object> GetTransferCS(string transferId)
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

    public List<object> GetItemsInTransferCS(string transferId)
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

    public void AddTransferCS(Dictionary<string, object> transfer)
    {
        transfer["transfer_status"] = "Scheduled";
        transfer["created_at"] = GetTimestampCS();
        transfer["updated_at"] = GetTimestampCS();
        data.Add(transfer);
    }

    public void UpdateTransferCS(string transferId, Dictionary<string, object> transfer)
    {
        transfer["updated_at"] = GetTimestampCS();
        for (int i = 0; i < data.Count; i++)
        {
            if (data[i]["id"].ToString() == transferId)
            {
                data[i] = transfer;
                break;
            }
        }
    }

    public void RemoveTransferCS(string transferId)
    {
        data.RemoveAll(x => x["id"].ToString() == transferId);
    }

    private void LoadCS(bool isDebug)
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
}
