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
    public Task<ItemGroupCS> CreateItemGroup(ItemGroupCS Itemgroup)
    {
        // Implementation code here
        return Task.FromResult(Itemgroup);
    }
}
