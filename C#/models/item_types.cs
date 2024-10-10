using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

public class ItemTypes : BaseCS
{
    private string dataPath;
    private List<Dictionary<string, object>> data;

    public ItemTypes(string rootPath, bool isDebug = false)
    {
        dataPath = Path.Combine(rootPath, "item_types.json");
        Load(isDebug);
    }

    public List<Dictionary<string, object>> GetItemTypes()
    {
        return data;
    }

    public Dictionary<string, object> GetItemType(string itemTypeId)
    {
        foreach (var item in data)
        {
            if (item["id"].ToString() == itemTypeId)
            {
                return item;
            }
        }
        return null;
    }

    public void AddItemType(Dictionary<string, object> itemType)
    {
        itemType["created_at"] = GetTimestamp();
        itemType["updated_at"] = GetTimestamp();
        data.Add(itemType);
    }

    public void UpdateItemType(string itemTypeId, Dictionary<string, object> itemType)
    {
        itemType["updated_at"] = GetTimestamp();
        for (int i = 0; i < data.Count; i++)
        {
            if (data[i]["id"].ToString() == itemTypeId)
            {
                data[i] = itemType;
                break;
            }
        }
    }

    public void RemoveItemType(string itemTypeId)
    {
        data.RemoveAll(item => item["id"].ToString() == itemTypeId);
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
        return DateTime.UtcNow.ToString("o");
    }
}
