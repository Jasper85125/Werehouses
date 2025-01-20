using System;
using System.Collections.Generic;
using Newtonsoft.Json;


namespace ServicesV1;

public class ItemGroupService : ItemService, IitemGroupService
{
    private string Path = "../../data/item_groups.json";
    ItemService itemService;
    public ItemGroupService()
    {
        itemService = new ItemService();
    }

    public List<ItemGroupCS> GetAllItemGroups()
    {
        if (!File.Exists(Path))
        {
            return new List<ItemGroupCS>();
        }

        var jsonData = File.ReadAllText(Path);
        var Itemgroups = JsonConvert.DeserializeObject<List<ItemGroupCS>>(jsonData);
        return Itemgroups ?? new List<ItemGroupCS>();
    }

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

    public async Task<ItemGroupCS> CreateItemGroup(ItemGroupCS newItemGroup)
    {
        List<ItemGroupCS> items = GetAllItemGroups();
        var currentDateTime = DateTime.Now;
        var formattedDateTime = currentDateTime.ToString("yyyy-MM-dd HH:mm:ss");

        if (items.Any())
        {
            newItemGroup.Id = items.Max(i => i.Id) + 1;
        }
        else
        {
            newItemGroup.Id = 1;
        }
        newItemGroup.created_at = DateTime.ParseExact(formattedDateTime, "yyyy-MM-dd HH:mm:ss", null);
        newItemGroup.updated_at = DateTime.ParseExact(formattedDateTime, "yyyy-MM-dd HH:mm:ss", null);
        items.Add(newItemGroup);

        var jsonData = JsonConvert.SerializeObject(items, Formatting.Indented);
        await File.WriteAllTextAsync(Path, jsonData);

        return newItemGroup;
    }

    public async Task<ItemGroupCS> UpdateItemGroup(int id, ItemGroupCS itemLine)
    {
        List<ItemGroupCS> items = GetAllItemGroups();
        var existingItem = items.FirstOrDefault(i => i.Id == id);
        if (existingItem == null)
        {
            return null;
        }

        var currentDateTime = DateTime.Now;

        var formattedDateTime = currentDateTime.ToString("yyyy-MM-dd HH:mm:ss");

        existingItem.Name = itemLine.Name;
        existingItem.Description = itemLine.Description;
        existingItem.updated_at = DateTime.ParseExact(formattedDateTime, "yyyy-MM-dd HH:mm:ss", null);

        var jsonData = JsonConvert.SerializeObject(items, Formatting.Indented);
        await File.WriteAllTextAsync("../../data/item_groups.json", jsonData);

        return existingItem;
    }
    
    public void DeleteItemGroup(int id)
    {
        List<ItemGroupCS> items = GetAllItemGroups();
        var item = items.FirstOrDefault(i => i.Id == id);
        if (item == null)
        {
            return;
        }

        items.Remove(item);

        var jsonData = JsonConvert.SerializeObject(items, Formatting.Indented);
        File.WriteAllTextAsync(Path, jsonData);
    }
}
