using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using ServicesV1;

public class ItemLineService : IItemLineService
{
    private string path = "../../data/item_lines.json";
    public ItemLineService()
    {
    }

    public List<ItemLineCS> GetAllItemlines()
    {
        if (!File.Exists(path))
        {
            return new List<ItemLineCS>();
        }

        var jsonData = File.ReadAllText(path);
        var items = JsonConvert.DeserializeObject<List<ItemLineCS>>(jsonData);
        return items ?? new List<ItemLineCS>();
    }

    public ItemLineCS GetItemLineById(int id)
    {
        var items = GetAllItemlines();
        var item = items.FirstOrDefault(i => i.Id == id);
        return item;
    }

    public ItemLineCS AddItemLine(ItemLineCS newItemLine)
    {
        List<ItemLineCS> items = GetAllItemlines();
        var currentDateTime = DateTime.Now;
        var formattedDateTime = currentDateTime.ToString("yyyy-MM-dd HH:mm:ss");

        if (items.Any())
        {
            newItemLine.Id = items.Max(i => i.Id) + 1;
        }
        else
        {
            newItemLine.Id = 1;
        }
        newItemLine.created_at = DateTime.ParseExact(formattedDateTime, "yyyy-MM-dd HH:mm:ss", null);
        newItemLine.updated_at = DateTime.ParseExact(formattedDateTime, "yyyy-MM-dd HH:mm:ss", null);
        items.Add(newItemLine);

        var jsonData = JsonConvert.SerializeObject(items, Formatting.Indented);
        File.WriteAllText(path, jsonData);

        return newItemLine;
    }

    public ItemLineCS UpdateItemLine(int id, ItemLineCS itemLine)
    {
        List<ItemLineCS> items = GetAllItemlines();
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
        File.WriteAllText(path, jsonData);

        return existingItem;
    }

    public void DeleteItemLine(int id)
    {
        List<ItemLineCS> items = GetAllItemlines();
        var item = items.FirstOrDefault(i => i.Id == id);
        if (item == null)
        {
            return;
        }

        items.Remove(item);

        var jsonData = JsonConvert.SerializeObject(items, Formatting.Indented);
        File.WriteAllText(path, jsonData);
    }

    public List<ItemCS> GetItemsByItemLineId(int itemlineId)
    {
        var itemsPath = "../../data/items.json";
        if (!File.Exists(itemsPath))
        {
            return new List<ItemCS>();
        }

        var jsonData = File.ReadAllText(itemsPath);
        List<ItemCS> items = JsonConvert.DeserializeObject<List<ItemCS>>(jsonData);

        return items?.Where(item => item.item_line == itemlineId).ToList() ?? new List<ItemCS>();
    }
}
