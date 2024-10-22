using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Services;

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

}
