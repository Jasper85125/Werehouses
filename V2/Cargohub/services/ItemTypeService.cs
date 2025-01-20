using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ServicesV2;

public class ItemTypeService : IItemtypeService
{
    private string path = "../../data/item_types.json";

    public List<ItemTypeCS> GetAllItemtypes()
    {
        if (!File.Exists(path))
        {
            return new List<ItemTypeCS>();
        }

        var jsonData = File.ReadAllText(path);
        var items = JsonConvert.DeserializeObject<List<ItemTypeCS>>(jsonData);
        return items ?? new List<ItemTypeCS>();
    }

    public ItemTypeCS GetItemById(int id)
    {
        var items = GetAllItemtypes();
        var item = items.FirstOrDefault(i => i.Id == id);
        return item;
    }

    public ItemTypeCS CreateItemType(ItemTypeCS newItemType)
    {
        List<ItemTypeCS> items = GetAllItemtypes();
        var currentDateTime = DateTime.Now;
        var formattedDateTime = currentDateTime.ToString("yyyy-MM-dd HH:mm:ss");

        if (items.Any())
        {
            newItemType.Id = items.Max(i => i.Id) + 1;
        }
        else
        {
            newItemType.Id = 1;
        }

        newItemType.created_at = DateTime.ParseExact(formattedDateTime, "yyyy-MM-dd HH:mm:ss", null);
        newItemType.updated_at = DateTime.ParseExact(formattedDateTime, "yyyy-MM-dd HH:mm:ss", null);
        items.Add(newItemType);

        var jsonData = JsonConvert.SerializeObject(items, Formatting.Indented);
        File.WriteAllText(path, jsonData);

        return newItemType;
    }

    public List<ItemTypeCS> CreateMultipleItemTypes(List<ItemTypeCS>newItemTypes)
    {
        List<ItemTypeCS> addedItemTypes = new List<ItemTypeCS>();
        foreach(ItemTypeCS itemType in newItemTypes)
        {
            ItemTypeCS addItemType = CreateItemType(itemType);
            addedItemTypes.Add(addItemType);
        }
        return addedItemTypes;
    }

    public ItemTypeCS UpdateItemType(int id, ItemTypeCS itemType)
    {
        List<ItemTypeCS> items = GetAllItemtypes();
        var existingItem = items.FirstOrDefault(i => i.Id == id);
        if (existingItem == null)
        {
            return null;
        }

        var currentDateTime = DateTime.Now;

        var formattedDateTime = currentDateTime.ToString("yyyy-MM-dd HH:mm:ss");

        existingItem.Name = itemType.Name;
        existingItem.description = itemType.description;
        existingItem.updated_at = DateTime.ParseExact(formattedDateTime, "yyyy-MM-dd HH:mm:ss", null);

        var jsonData = JsonConvert.SerializeObject(items, Formatting.Indented);
        File.WriteAllText(path, jsonData); 

        return existingItem;
    }

    public ItemTypeCS PatchItemType(int id, string property, object newvalue){
        var formattednow = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        var itemtypes = GetAllItemtypes();
        var itemtype = itemtypes.Find(_=>_.Id == id);
        if(itemtype is null){
            return null;
        }
        switch (property){
            case"Name":
            itemtype.Name = newvalue.ToString();
            break;
            case"description":
            itemtype.description = newvalue.ToString();
            break;
        }
        itemtype.updated_at = DateTime.ParseExact(formattednow, "yyyy-MM-dd HH:mm:ss", null);
        var json = JsonConvert.SerializeObject(itemtypes, Formatting.Indented);
        File.WriteAllText(path, json);
        return itemtype;
    }
    public void DeleteItemType(int id)
    {
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
        var json = JsonConvert.SerializeObject(item_types, Formatting.Indented);
        File.WriteAllText(path, json);
    }
}
