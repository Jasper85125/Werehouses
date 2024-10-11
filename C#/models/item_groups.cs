using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

public class ItemGroupCS
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class ItemGroupsCS : BaseCS
{
    private string dataPath;
    private List<ItemGroupCS> data;

    public ItemGroupsCS(string rootPath, bool isDebug = false)
    {
        dataPath = Path.Combine(rootPath, "item_lines.json");
        LoadCS(isDebug);
    }

    public List<ItemGroupCS> GetItemGroupsCS()
    {
        return data;
    }

    public ItemGroupCS GetItemGroupCS(int itemGroupId)
    {
        return data.Find(x => x.Id == itemGroupId);
    }

    public void AddItemGroupCS(ItemGroupCS itemGroup)
    {
        itemGroup.CreatedAt = GetTimestamp();
        itemGroup.UpdatedAt = GetTimestamp();
        data.Add(itemGroup);
    }

    public void UpdateItemGroupCS(int itemGroupId, ItemGroupCS itemGroup)
    {
        itemGroup.UpdatedAt = GetTimestamp();
        var index = data.FindIndex(x => x.Id == itemGroupId);
        if (index != -1)
        {
            data[index] = itemGroup;
        }
    }

    public void RemoveItemGroupCS(int itemGroupId)
    {
        data.RemoveAll(x => x.Id == itemGroupId);
    }

    private void LoadCS(bool isDebug)
    {
        if (isDebug)
        {
            data = new List<ItemGroupCS>();
        }
        else
        {
            using (StreamReader r = new StreamReader(dataPath))
            {
                string json = r.ReadToEnd();
                data = JsonConvert.DeserializeObject<List<ItemGroupCS>>(json);
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

    private DateTime GetTimestamp()
    {
        return DateTime.Now;
    }
}
