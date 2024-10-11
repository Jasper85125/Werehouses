using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

public class ItemLineCS
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set;}
    public string CreatedAt { get; set; }
    public string UpdatedAt { get; set; }
}

public class ItemLinesCS : BaseCS
{
    private string dataPath;
    private List<ItemLineCS> data;

    public ItemLinesCS(string rootPath, bool isDebug = false)
    {
        dataPath = Path.Combine(rootPath, "item_lines.json");
        LoadCS(isDebug);
    }

    public List<ItemLineCS> GetItemLinesCS()
    {
        return data;
    }

    public ItemLineCS GetItemLineCS(int itemLineId)
    {
        return data.Find(x => x.Id == itemLineId);
    }

    public void AddItemLineCS(ItemLineCS itemLine)
    {
        itemLine.CreatedAt = GetTimestampCS();
        itemLine.UpdatedAt = GetTimestampCS();
        data.Add(itemLine);
    }

    public void UpdateItemLineCS(int itemLineId, ItemLineCS itemLine)
    {
        itemLine.UpdatedAt = GetTimestampCS();
        var index = data.FindIndex(x => x.Id == itemLineId);
        if (index != -1)
        {
            data[index] = itemLine;
        }
    }

    public void RemoveItemLineCS(int itemLineId)
    {
        data.RemoveAll(x => x.Id == itemLineId);
    }

    private void LoadCS(bool isDebug)
    {
        if (isDebug)
        {
            data = new List<ItemLineCS>(); // Replace with actual debug data if needed
        }
        else
        {
            using (StreamReader r = new StreamReader(dataPath))
            {
                string json = r.ReadToEnd();
                data = JsonConvert.DeserializeObject<List<ItemLineCS>>(json);
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
