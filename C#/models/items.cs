using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

public class Item
{
    public string Uid { get; set; }
    public string ItemLine { get; set; }
    public string ItemGroup { get; set; }
    public string ItemType { get; set; }
    public string SupplierId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class Items
{
    private string dataPath;
    private List<Item> data;
    private static List<Item> ITEMS = new List<Item>();

    public Items(string rootPath, bool isDebug = false)
    {
        dataPath = Path.Combine(rootPath, "items.json");
        Load(isDebug);
    }

    public List<Item> GetItems()
    {
        return data;
    }

    public Item GetItem(string itemId)
    {
        return data.Find(x => x.Uid == itemId);
    }

    public List<Item> GetItemsForItemLine(string itemLineId)
    {
        return data.FindAll(x => x.ItemLine == itemLineId);
    }

    public List<Item> GetItemsForItemGroup(string itemGroupId)
    {
        return data.FindAll(x => x.ItemGroup == itemGroupId);
    }

    public List<Item> GetItemsForItemType(string itemTypeId)
    {
        return data.FindAll(x => x.ItemType == itemTypeId);
    }

    public List<Item> GetItemsForSupplier(string supplierId)
    {
        return data.FindAll(x => x.SupplierId == supplierId);
    }

    public void AddItem(Item item)
    {
        item.CreatedAt = GetTimestamp();
        item.UpdatedAt = GetTimestamp();
        data.Add(item);
    }

    public void UpdateItem(string itemId, Item item)
    {
        item.UpdatedAt = GetTimestamp();
        int index = data.FindIndex(x => x.Uid == itemId);
        if (index != -1)
        {
            data[index] = item;
        }
    }

    public void RemoveItem(string itemId)
    {
        data.RemoveAll(x => x.Uid == itemId);
    }

    private void Load(bool isDebug)
    {
        if (isDebug)
        {
            data = ITEMS;
        }
        else
        {
            using (StreamReader r = new StreamReader(dataPath))
            {
                string json = r.ReadToEnd();
                data = JsonConvert.DeserializeObject<List<Item>>(json);
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
