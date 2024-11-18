using System;
using System.Collections.Generic;
using Microsoft.VisualBasic;
using Newtonsoft.Json;


namespace Services;

public class ItemGroupService : ItemService, IitemGroupService
{
    // Constructor
    ItemService itemService;
    public ItemGroupService()
    {
        // Initialization code here
        itemService = new ItemService();
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
    public List<ItemCS> ItemsFromItemGroupId(int groupid){
        var items = itemService.GetAllItems();
        var find = items.FindAll(_ => _.item_group == groupid);
        if(find is null){
            return null;
        }
        return find;
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

    // Method to update an existing Itemgroup
    public async Task<ItemGroupCS> UpdateItemGroup(int id, ItemGroupCS itemLine)
    {
        List<ItemGroupCS> items = GetAllItemGroups();
        var existingItem = items.FirstOrDefault(i => i.Id == id);
        if (existingItem == null)
        {
            return null;
        }

        // Get the current date and time
        var currentDateTime = DateTime.Now;

        // Format the date and time to the desired format
        var formattedDateTime = currentDateTime.ToString("yyyy-MM-dd HH:mm:ss");

        existingItem.Name = itemLine.Name;
        existingItem.Description = itemLine.Description;
        existingItem.updated_at = DateTime.ParseExact(formattedDateTime, "yyyy-MM-dd HH:mm:ss", null);

        var jsonData = JsonConvert.SerializeObject(items, Formatting.Indented);
        await File.WriteAllTextAsync("data/item_lines.json", jsonData);

        return existingItem;
    }
    
    // Method to delete an Itemgroup
    public void DeleteItemGroup(int id)
    {
        var path = "data/item_groups.json";
        List<ItemGroupCS> items = GetAllItemGroups();
        var item = items.FirstOrDefault(i => i.Id == id);
        if (item == null)
        {
            return;
        }

        items.Remove(item);

        var jsonData = JsonConvert.SerializeObject(items, Formatting.Indented);
        File.WriteAllTextAsync(path, jsonData);
    }
    public void DeleteItemGroups(List<int> ids){
        var item_groups = GetAllItemGroups();
        foreach(int id in ids){
            var item_group = item_groups.Find(_=>_.Id == id);
            if(item_group is not null){
                item_groups.Remove(item_group);
            }
        }
        var path = "data/item_groups.json";
        var json = JsonConvert.SerializeObject(item_groups, Formatting.Indented);
        File.WriteAllText(path, json);
    }
}
