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
    public ItemLineCS AddItemLine(ItemLineCS newItemLine)
    {
        List<ItemLineCS> items = GetAllItemlines();
        var currentDateTime = DateTime.Now;
        var formattedDateTime = currentDateTime.ToString("yyyy-MM-dd HH:mm:ss");

        // Auto-increment ID
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
        File.WriteAllText("data/item_lines.json", jsonData);

        return newItemLine;
    }

    public List<ItemLineCS> CreateMultipleItemLines(List<ItemLineCS>newItemLines)
    {
        List<ItemLineCS> addedItemLines = new List<ItemLineCS>();
        foreach(ItemLineCS itemLine in newItemLines)
        {
            ItemLineCS addItemLine = AddItemLine(itemLine);
            addedItemLines.Add(addItemLine);
        }
        return addedItemLines;
    }

    // Method to update an item
    public ItemLineCS UpdateItemLine(int id, ItemLineCS itemLine)
    {
        List<ItemLineCS> items = GetAllItemlines();
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
        File.WriteAllText("data/item_lines.json", jsonData);

        return existingItem;
    }

    public void DeleteItemLine(int id)
    {
        var _path  = "data/item_lines.json";
        List<ItemLineCS> items = GetAllItemlines();
        var item = items.FirstOrDefault(i => i.Id == id);
        if (item == null)
        {
            return;
        }

        items.Remove(item);

        var jsonData = JsonConvert.SerializeObject(items, Formatting.Indented);
        File.WriteAllText(_path, jsonData);
    }

    public List<ItemCS> GetItemsByItemLineId(int itemlineId)
    {
        var itemsPath = "data/items.json";
        if (!File.Exists(itemsPath))
        {
            return new List<ItemCS>();
        }

        var jsonData = File.ReadAllText(itemsPath);
        List<ItemCS> items = JsonConvert.DeserializeObject<List<ItemCS>>(jsonData);

        return items?.Where(item => item.item_line == itemlineId).ToList() ?? new List<ItemCS>();
    }
    public void DeleteItemLines(List<int> ids){
        var item_lines = GetAllItemlines();
        foreach(int id in ids){
            var item_line = item_lines.Find(_=>_.Id == id);
            if(item_line is not null){
                item_lines.Remove(item_line);
            }
        }
        var path = "data/item_lines.json";
        var json = JsonConvert.SerializeObject(item_lines, Formatting.Indented);
        File.WriteAllText(path, json);
    }
}
