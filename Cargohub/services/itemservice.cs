using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Services;

public class ItemService : IItemService
{
    // Constructor
    public ItemService()
    {
        // Initialization code here
    }

    // Method to get all items
    public List<ItemCS> GetAllItems()
    {
        var path = "data/items.json";
        if (!File.Exists(path))
        {
            return new List<ItemCS>();
        }

        var jsonData = File.ReadAllText(path);
        var items = JsonConvert.DeserializeObject<List<ItemCS>>(jsonData);
        return items ?? new List<ItemCS>();
    }

    // Method to get an item by ID
    public ItemCS GetItemById(string uid)
    {
        var items = GetAllItems();
        var item = items.FirstOrDefault(i => i.uid == uid);
        return item;
    }

    // Method to add a new item
    public ItemCS CreateItem(ItemCS item)
    {
        var path = "data/items.json";
        List<ItemCS> items;

        if (File.Exists(path))
        {
            var jsonData = File.ReadAllText(path);
            items = JsonConvert.DeserializeObject<List<ItemCS>>(jsonData) ?? new List<ItemCS>();
        }
        else
        {
            items = new List<ItemCS>();
        }

        // Generate a new unique UID
        string newUid;
        if (items.Count > 0)
        {
            var maxUid = items.Max(i => i.uid);
            var numericPart = int.Parse(maxUid.Substring(1)); // Extract numeric part
            newUid = "P" + (numericPart + 1).ToString("D6"); // Increment and format back
        }
        else
        {
            newUid = "P000001"; // Starting UID
        }
        item.uid = newUid;

        items.Add(item);

        var updatedJsonData = JsonConvert.SerializeObject(items, Formatting.Indented);
        File.WriteAllText(path, updatedJsonData);

        return item;
    }
}
