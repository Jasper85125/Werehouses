using System;
using System.Collections.Generic;
using Microsoft.VisualBasic;
using Newtonsoft.Json;

namespace ServicesV2;

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
    public List<ItemCS> ItemsFromItemGroupId(int groupid)
    {
        var items = itemService.GetAllItems();
        var find = items.FindAll(_ => _.item_group == groupid);
        if (find is null)
        {
            return null;
        }
        return find;
    }

    public ItemGroupCS CreateItemGroup(ItemGroupCS newItemGroup)
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
        File.WriteAllText(Path, jsonData);

        return newItemGroup;
    }

    public List<ItemGroupCS> CreateMultipleItemGroups(List<ItemGroupCS>newItemGroup)
    {
        List<ItemGroupCS> addedItemGroups = new List<ItemGroupCS>();
        foreach(ItemGroupCS itemGroup in newItemGroup)
        {
            ItemGroupCS addItemGroup = CreateItemGroup(itemGroup);
            addedItemGroups.Add(addItemGroup);
        }
        return addedItemGroups;
    }

    public ItemGroupCS UpdateItemGroup(int id, ItemGroupCS itemLine)
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
        File.WriteAllText(Path, jsonData);

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
        File.WriteAllText(Path, jsonData);
    }

    public ItemGroupCS PatchItemGroup(int Id, string property, object newvalue)
    {
        List<ItemGroupCS> items = GetAllItemGroups();
        var existingItem = items.Find(i => i.Id == Id);
        if (existingItem == null)
        {
            return null;
        }

        var currentDateTime = DateTime.Now;

        var formattedDateTime = currentDateTime.ToString("yyyy-MM-dd HH:mm:ss");
        switch(property){
            case "Name":
                existingItem.Name = newvalue.ToString();
                break;
            case "Description":
                existingItem.Description = newvalue.ToString();
                break;
            default:
                return null;
        }

        existingItem.updated_at = DateTime.ParseExact(formattedDateTime, "yyyy-MM-dd HH:mm:ss", null);

        var jsonData = JsonConvert.SerializeObject(items, Formatting.Indented);
        File.WriteAllText(Path, jsonData);

        return existingItem;
    }

    public void DeleteItemGroups(List<int> ids)
    {
        var item_groups = GetAllItemGroups();
        foreach (int id in ids)
        {
            var item_group = item_groups.Find(_ => _.Id == id);
            if (item_group is not null)
            {
                item_groups.Remove(item_group);
            }
        }
        var json = JsonConvert.SerializeObject(item_groups, Formatting.Indented);
        File.WriteAllText(Path, json);
    }
}
