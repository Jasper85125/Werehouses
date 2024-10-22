using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Services;

public class ItemLineService : IItemLineService
{
    // Constructor
    public ItemLineService()
    {
        // Initialization code here
    }

    // Method to get all items
    public List<ItemLineCS> GetAllItemlines()
    {
        var path = "data/item_lines.json";
        if (!File.Exists(path))
        {
            return new List<ItemLineCS>();
        }

        var jsonData = File.ReadAllText(path);
        var items = JsonConvert.DeserializeObject<List<ItemLineCS>>(jsonData);
        return items ?? new List<ItemLineCS>();
    }

    // Method to get an item by ID
    public ItemLineCS GetItemLineById(int id)
    {
        var items = GetAllItemlines();
        var item = items.FirstOrDefault(i => i.Id == id);
        return item;
    }

    // Method to add a new item
    public async Task<ItemLineCS> AddItemLine(ItemLineCS newItemLine)
    {
        List<ItemLineCS> items = GetAllItemlines();

        // Auto-increment ID
        if (items.Any())
        {
            newItemLine.Id = items.Max(i => i.Id) + 1;
        }
        else
        {
            newItemLine.Id = 1;
        }

        items.Add(newItemLine);

        var jsonData = JsonConvert.SerializeObject(items, Formatting.Indented);
        await File.WriteAllTextAsync("data/item_lines.json", jsonData);

        return newItemLine;
    }
}
