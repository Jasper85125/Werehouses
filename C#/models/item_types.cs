using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

public class ItemTypeCS
{
    public int Id { get; set;}
    public string Name { get; set; }
    public string Description { get; set;}
    public string CreatedAt { get; set; }
    public string UpdatedAt { get; set; }
}

public class ItemTypesCS : BaseCS
{
    private string dataPath;
    private List<Dictionary<string, object>> data;

    public ItemTypesCS(string rootPath, bool isDebug = false)
    {
        dataPath = Path.Combine(rootPath, "item_types.json");
        LoadCS(isDebug);
    }

    public List<Dictionary<string, object>> GetItemTypesCS()
    {
        return data;
    }

    public Dictionary<string, object> GetItemTypeCS(string itemTypeId)
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

    public void AddItemTypeCS(Dictionary<string, object> itemType)
    {
        itemType["created_at"] = GetTimestampCS();
        itemType["updated_at"] = GetTimestampCS();
        data.Add(itemType);
    }

    public void UpdateItemTypeCS(string itemTypeId, Dictionary<string, object> itemType)
    {
        itemType["updated_at"] = GetTimestampCS();
        for (int i = 0; i < data.Count; i++)
        {
            if (data[i]["id"].ToString() == itemTypeId)
            {
                data[i] = itemType;
                break;
            }
        }
    }

    public void RemoveItemTypeCS(string itemTypeId)
    {
        data.RemoveAll(item => item["id"].ToString() == itemTypeId);
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

    public void SaveCS()
    {
        using (StreamWriter w = new StreamWriter(dataPath))
        {
            string json = JsonConvert.SerializeObject(data, Formatting.Indented);
            w.Write(json);
        }
    }
}
