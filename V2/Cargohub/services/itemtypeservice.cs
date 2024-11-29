using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ServicesV2;

public class ItemTypeService : IItemtypeService
{

    // Method to get all item types
    public List<ItemTypeCS> GetAllItemtypes()
    {
        var path = "data/item_types.json";
        if (!File.Exists(path))
        {
            return new List<ItemTypeCS>();
        }

        var jsonData = File.ReadAllText(path);
        var items = JsonConvert.DeserializeObject<List<ItemTypeCS>>(jsonData);
        return items ?? new List<ItemTypeCS>();
    }

    // Method to get an item type by ID
    public ItemTypeCS GetItemById(int id)
    {
        var items = GetAllItemtypes();
        var item = items.FirstOrDefault(i => i.Id == id);
        return item;
    }

    // Method to create a new item type
    public async Task<ItemTypeCS> CreateItemType(ItemTypeCS newItemType)
    {
        List<ItemTypeCS> items = GetAllItemtypes();

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
        await File.WriteAllTextAsync("data/item_types.json", jsonData);

        return newItemType;
    }

    // Method to update an item type
    public async Task<ItemTypeCS> UpdateItemType(int id, ItemTypeCS itemType)
    {
        List<ItemTypeCS> items = GetAllItemtypes();
        var existingItem = items.FirstOrDefault(i => i.Id == id);
        if (existingItem == null)
        {
            return null;
        }

        // Get the current date and time
        var currentDateTime = DateTime.Now;

        // Format the date and time to the desired format
        var formattedDateTime = currentDateTime.ToString("yyyy-MM-dd HH:mm:ss");

        existingItem.Name = itemType.Name;
        existingItem.description = itemType.description;
        existingItem.updated_at = DateTime.ParseExact(formattedDateTime, "yyyy-MM-dd HH:mm:ss", null);

        var jsonData = JsonConvert.SerializeObject(items, Formatting.Indented);
        await File.WriteAllTextAsync("data/item_types.json", jsonData);

        return existingItem;
    }

    public void DeleteItemType(int id)
    {
        var path = "data/item_types.json";
        List<ItemTypeCS> items = GetAllItemtypes();
        var item = items.FirstOrDefault(i => i.Id == id);
        if (item == null)
        {
            return;
        }

        items.Remove(item);

        var jsonData = JsonConvert.SerializeObject(items, Formatting.Indented);
        File.WriteAllText(path, jsonData);
    }
    public void DeleteItemTypes(List<int> ids){
        var item_types = GetAllItemtypes();
        foreach(int id in ids){
            var item_type = item_types.Find(_=>_.Id == id);
            if(item_type is not null){
                item_types.Remove(item_type);
            }
        }
        var path = "data/item_types.json";
        var json = JsonConvert.SerializeObject(item_types, Formatting.Indented);
        File.WriteAllText(path, json);
    }
}
