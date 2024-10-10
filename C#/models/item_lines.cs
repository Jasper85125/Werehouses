using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

public class ItemLine
{
    public int Id { get; set; }
    public string CreatedAt { get; set; }
    public string UpdatedAt { get; set; }
    // Add other properties as needed
}

public class ItemLines : BaseCS
{
    private string dataPath;
    private List<ItemLine> data;

    public ItemLines(string rootPath, bool isDebug = false)
    {
        dataPath = Path.Combine(rootPath, "item_lines.json");
        Load(isDebug);
    }

    public List<ItemLine> GetItemLines()
    {
        return data;
    }

    public ItemLine GetItemLine(int itemLineId)
    {
        return data.Find(x => x.Id == itemLineId);
    }

    public void AddItemLine(ItemLine itemLine)
    {
        itemLine.CreatedAt = GetTimestamp();
        itemLine.UpdatedAt = GetTimestamp();
        data.Add(itemLine);
    }

    public void UpdateItemLine(int itemLineId, ItemLine itemLine)
    {
        itemLine.UpdatedAt = GetTimestamp();
        var index = data.FindIndex(x => x.Id == itemLineId);
        if (index != -1)
        {
            data[index] = itemLine;
        }
    }

    public void RemoveItemLine(int itemLineId)
    {
        data.RemoveAll(x => x.Id == itemLineId);
    }

    private void Load(bool isDebug)
    {
        if (isDebug)
        {
            data = new List<ItemLine>(); // Replace with actual debug data if needed
        }
        else
        {
            using (StreamReader r = new StreamReader(dataPath))
            {
                string json = r.ReadToEnd();
                data = JsonConvert.DeserializeObject<List<ItemLine>>(json);
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
