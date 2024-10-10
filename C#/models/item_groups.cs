using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

public class ItemGroup
{
    public int Id { get; set; }
    public string Name { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class ItemGroups : BaseCS
{
    private string dataPath;
    private List<ItemGroup> data;

    public ItemGroups(string rootPath, bool isDebug = false)
    {
        dataPath = Path.Combine(rootPath, "item_lines.json");
        Load(isDebug);
    }

    public List<ItemGroup> GetItemGroups()
    {
        return data;
    }

    public ItemGroup GetItemGroup(int itemGroupId)
    {
        return data.Find(x => x.Id == itemGroupId);
    }

    public void AddItemGroup(ItemGroup itemGroup)
    {
        itemGroup.CreatedAt = GetTimestamp();
        itemGroup.UpdatedAt = GetTimestamp();
        data.Add(itemGroup);
    }

    public void UpdateItemGroup(int itemGroupId, ItemGroup itemGroup)
    {
        itemGroup.UpdatedAt = GetTimestamp();
        var index = data.FindIndex(x => x.Id == itemGroupId);
        if (index != -1)
        {
            data[index] = itemGroup;
        }
    }

    public void RemoveItemGroup(int itemGroupId)
    {
        data.RemoveAll(x => x.Id == itemGroupId);
    }

    private void Load(bool isDebug)
    {
        if (isDebug)
        {
            data = new List<ItemGroup>();
        }
        else
        {
            using (StreamReader r = new StreamReader(dataPath))
            {
                string json = r.ReadToEnd();
                data = JsonConvert.DeserializeObject<List<ItemGroup>>(json);
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
