using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ServicesV2;

public class ItemService : IItemService
{
    // Constructor
    private string path = "../../data/items.json";
    public ItemService()
    {
        // Initialization code here
    }

    // Method to get all items
    public List<ItemCS> GetAllItems()
    {
        if (!File.Exists(path))
        {
            return new List<ItemCS>();
        }

        var jsonData = File.ReadAllText(path);
        var items = JsonConvert.DeserializeObject<List<ItemCS>>(jsonData);
        return items ?? new List<ItemCS>();
    }

    // Method to get an item by ID
    public ItemCS GetItemById(string uid)
    {
        var items = GetAllItems();
        var item = items.FirstOrDefault(i => i.uid == uid);
        return item;
    }

    public IEnumerable<ItemCS> GetAllItemsInItemType(int itemTypeId)
    {
        List<ItemCS> itemTypeItems = new List<ItemCS>();
        var items = GetAllItems();
        foreach (ItemCS item in items)
        {
            if (item.item_type == itemTypeId)
            {
                itemTypeItems.Add(item);
            }
        }
        return itemTypeItems;
    }

    public void GenerateReport(List<string> uids)
    {
        var items = GetAllItems();
        List<string> reportStrings = new List<string>();

        foreach (var uid in uids)
        {
            var item = items.FirstOrDefault(i => i.uid == uid);
            if (item != null)
            {
                string itemReport = $"The item {item.uid} is a {item.short_description}. There have been {item.unit_order_quantity} orders. That is in total {item.unit_purchase_quantity}. The item was created at {item.created_at} and the last time it was updated was {item.updated_at}.";
                reportStrings.Add(itemReport);
            }
        }

        var directory = "reports";
        var _path = Path.Combine(directory, "report.txt");

        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
        File.WriteAllLines(_path, reportStrings);

    }


    public ItemCS CreateItem(ItemCS item)
    {
        List<ItemCS> items;
        var currentDateTime = DateTime.Now;
        var formattedDateTime = currentDateTime.ToString("yyyy-MM-dd HH:mm:ss");

        if (File.Exists(path))
        {
            var jsonData = File.ReadAllText(path);
            items = JsonConvert.DeserializeObject<List<ItemCS>>(jsonData) ?? new List<ItemCS>();
        }
        else
        {
            items = new List<ItemCS>();
        }

        string newUid;
        if (items.Count > 0)
        {
            var maxUid = items.Max(i => i.uid);
            var numericPart = int.Parse(maxUid.Substring(1));
            newUid = "P" + (numericPart + 1).ToString("D6");
        }
        else
        {
            newUid = "P000001";
        }
        item.uid = newUid;
        item.created_at = DateTime.ParseExact(formattedDateTime, "yyyy-MM-dd HH:mm:ss", null);
        item.updated_at = DateTime.ParseExact(formattedDateTime, "yyyy-MM-dd HH:mm:ss", null);
        items.Add(item);

        var updatedJsonData = JsonConvert.SerializeObject(items, Formatting.Indented);
        File.WriteAllText(path, updatedJsonData);

        return item;
    }

    public List<ItemCS> CreateMultipleItems(List<ItemCS> newItems)
    {
        List<ItemCS> addedItem = new List<ItemCS>();
        foreach (ItemCS item in newItems)
        {
            ItemCS addItem = CreateItem(item);
            addedItem.Add(addItem);
        }
        return addedItem;
    }

    public ItemCS UpdateItem(string uid, ItemCS item)
    {
        var items = GetAllItems();
        var existingItem = items.FirstOrDefault(i => i.uid == uid);
        if (existingItem == null)
        {
            return null;
        }
        var currentDateTime = DateTime.Now;

        var formattedDateTime = currentDateTime.ToString("yyyy-MM-dd HH:mm:ss");

        existingItem.code = item.code;
        existingItem.description = item.description;
        existingItem.short_description = item.short_description;
        existingItem.upc_code = item.upc_code;
        existingItem.model_number = item.model_number;
        existingItem.commodity_code = item.commodity_code;
        existingItem.item_line = item.item_line;
        existingItem.item_group = item.item_group;
        existingItem.item_type = item.item_type;
        existingItem.unit_purchase_quantity = item.unit_purchase_quantity;
        existingItem.unit_order_quantity = item.unit_order_quantity;
        existingItem.pack_order_quantity = item.pack_order_quantity;
        existingItem.supplier_id = item.supplier_id;
        existingItem.supplier_code = item.supplier_code;
        existingItem.supplier_part_number = item.supplier_part_number;
        existingItem.updated_at = DateTime.ParseExact(formattedDateTime, "yyyy-MM-dd HH:mm:ss", null);

        var updatedJsonData = JsonConvert.SerializeObject(items, Formatting.Indented);
        File.WriteAllText(path, updatedJsonData);

        return existingItem;
    }
    public ItemCS PatchItem(string uid, string property, object newvalue)
    {
        var formattednow = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        var items = GetAllItems();
        var item = items.Find(_ => _.uid == uid);
        if (item is null)
        {
            return null;
        }
        switch (property)
        {
            case "code":
                item.code = newvalue.ToString();
                break;
            case "description":
                item.description = newvalue.ToString();
                break;
            case "short_description":
                item.short_description = newvalue.ToString();
                break;
            case "upc_code":
                item.upc_code = newvalue.ToString();
                break;
            case "model_number":
                item.model_number = newvalue.ToString();
                break;
            case "commodity_code":
                item.commodity_code = newvalue.ToString();
                break;
            case "item_line":
                item.item_line = (int)newvalue;
                break;
            case "item_group":
                item.item_group = (int)newvalue;
                break;
            case "item_type":
                item.item_type = (int)newvalue;
                break;
            case "unit_purchase_quantity":
                item.unit_purchase_quantity = (int)newvalue;
                break;
            case "unit_order_quantity":
                item.unit_order_quantity = (int)newvalue;
                break;
            case "pack_order_quantity":
                item.pack_order_quantity = (int)newvalue;
                break;
            case "supplier_id":
                item.supplier_id = (int)newvalue;
                break;
            case "supplier_code":
                item.supplier_code = newvalue.ToString();
                break;
            case "supplier_part_number":
                item.supplier_part_number = newvalue.ToString();
                break;
        }
        item.updated_at = DateTime.ParseExact(formattednow, "yyyy-MM-dd HH:mm:ss", null);
        var json = JsonConvert.SerializeObject(items, Formatting.Indented);
        File.WriteAllText(path, json);
        return item;
    }
    public void DeleteItem(string uid)
    {
        var items = GetAllItems();
        var item = items.FirstOrDefault(i => i.uid == uid);
        if (item == null)
        {
            return;
        }

        items.Remove(item);

        var updatedJsonData = JsonConvert.SerializeObject(items, Formatting.Indented);
        File.WriteAllText(path, updatedJsonData);
    }

    public void DeleteItems(List<string> uids)
    {
        var items = GetAllItems();
        foreach (var uid in uids)
        {
            var item = items.FirstOrDefault(i => i.uid == uid);
            if (item != null)
            {
                items.Remove(item);
            }
        }

        var updatedJsonData = JsonConvert.SerializeObject(items, Formatting.Indented);
        File.WriteAllText(path, updatedJsonData);
    }
}
