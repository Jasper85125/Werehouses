using System;
using System.Collections.Generic;
using Newtonsoft.Json;


namespace Services;

public class ItemGroupService : IitemGroupService
{
    // Constructor
    public ItemGroupService()
    {
        // Initialization code here
    }

    // Method to get all Itemgroups
    public List<ItemGroupCS> GetAllItemGroups()
    {
        var path = "data/item_groups.json";
        if (!File.Exists(path))
        {
            return new List<ItemGroupCS>();
        }

        var jsonData = File.ReadAllText(path);
        var Itemgroups = JsonConvert.DeserializeObject<List<ItemGroupCS>>(jsonData);
        return Itemgroups ?? new List<ItemGroupCS>();
    }

    // Method to get an Itemgroup by ID
    public ItemGroupCS GetItemById(int id)
    {
        var Itemgroups = GetAllItemGroups();
        var Itemgroup = Itemgroups.FirstOrDefault(i => i.Id == id);
        return Itemgroup;
    }

    // Method to add a new Itemgroup
    public async Task<ItemGroupCS> CreateItemGroup(ItemGroupCS newItemType)
    {
        List<ItemGroupCS> items = GetAllItemGroups();

        // Auto-increment ID
        if (items.Any())
        {
            newItemType.Id = items.Max(i => i.Id) + 1;
        }
        else
        {
            newItemType.Id = 1;
        }

        items.Add(newItemType);

        var jsonData = JsonConvert.SerializeObject(items, Formatting.Indented);
        await File.WriteAllTextAsync("data/item_groups.json", jsonData);

        return newItemType;
    }
}
