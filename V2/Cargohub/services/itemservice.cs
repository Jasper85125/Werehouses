using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ServicesV2;

public class ItemService : IItemService
{
    // Constructor
    public ItemService()
    {
        // Initialization code here
    }

    // Method to get all items
    public List<ItemCS> GetAllItems()
    {
        var path = "data/items.json";
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
        List<ItemCS> itemTypeItems = new List<ItemCS> ();
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

    // public void GenerateReport(List<string> uids)
    // {
    //     var items = GetAllItems();
    //     var reportItems = new List<object>(); // Using a list of anonymous objects
    //     foreach (var uid in uids)
    //     {
    //         var item = items.FirstOrDefault(i => i.uid == uid);
    //         if (item != null)
    //         {
    //             reportItems.Add(new
    //             {
    //                 item.uid,
    //                 item.short_description,
    //                 item.unit_purchase_quantity,
    //                 item.unit_order_quantity,
    //                 item.created_at,
    //                 item.updated_at
    //             });
    //         }
    //     }

    //     var directory = "reports";
    //     var path = Path.Combine(directory, "report.json");

    //     // Ensure the directory exists
    //     if (!Directory.Exists(directory))
    //     {
    //         Directory.CreateDirectory(directory);
    //     }
    //     var updatedJsonData = JsonConvert.SerializeObject(reportItems, Formatting.Indented);
    //     File.WriteAllText(path, updatedJsonData);
    // }


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
        var path = Path.Combine(directory, "report.txt");

        // Ensure the directory exists
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }        
        File.WriteAllLines(path, reportStrings); 

    }


    // public void GenerateReport(List<string> uids)
    // {
    //     var items = GetAllItems();
    //     List<ItemCS> reportItems = new List<ItemCS>();
    //     foreach (var uid in uids)
    //     {
    //         var item = items.FirstOrDefault(i => i.uid == uid);
    //         if (item != null)
    //         {
    //             reportItems.Add(item);
    //         }
    //     }

    //     var path = "data/report.json";
    //     var updatedJsonData = JsonConvert.SerializeObject(reportItems, Formatting.Indented);
    //     File.WriteAllText(path, updatedJsonData);
    // }

    // Method to add a new item
    public ItemCS CreateItem(ItemCS item)
    {
        var path = "data/items.json";
        List<ItemCS> items;

        if (File.Exists(path))
        {
            var jsonData = File.ReadAllText(path);
            items = JsonConvert.DeserializeObject<List<ItemCS>>(jsonData) ?? new List<ItemCS>();
        }
        else
        {
            items = new List<ItemCS>();
        }

        // Generate a new unique UID
        string newUid;
        if (items.Count > 0)
        {
            var maxUid = items.Max(i => i.uid);
            var numericPart = int.Parse(maxUid.Substring(1)); // Extract numeric part
            newUid = "P" + (numericPart + 1).ToString("D6"); // Increment and format back
        }
        else
        {
            newUid = "P000001"; // Starting UID
        }
        item.uid = newUid;

        items.Add(item);

        var updatedJsonData = JsonConvert.SerializeObject(items, Formatting.Indented);
        File.WriteAllText(path, updatedJsonData);

        return item;
    }

    public List<ItemCS> CreateMultipleItems(List<ItemCS>newItems)
    {
        List<ItemCS> addedItem = new List<ItemCS>();
        foreach(ItemCS item in newItems)
        {
            ItemCS addItem = CreateItem(item);
            addedItem.Add(addItem);
        }
        return addedItem;
    }

    // Method to update an existing item
    public ItemCS UpdateItem(string uid, ItemCS item)
    {
        var items = GetAllItems();
        var existingItem = items.FirstOrDefault(i => i.uid == uid);
        if (existingItem == null)
        {
            return null;
        }
        // Get the current date and time
        var currentDateTime = DateTime.Now;

        // Format the date and time to the desired format
        var formattedDateTime = currentDateTime.ToString("yyyy-MM-dd HH:mm:ss");

        // Update the properties of the existing item
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

        var path = "data/items.json";
        var updatedJsonData = JsonConvert.SerializeObject(items, Formatting.Indented);
        File.WriteAllText(path, updatedJsonData);

        return existingItem;
    }
    public ItemCS PatchItem(string uid, string property, object newvalue){
        var formattednow = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        var items = GetAllItems();
        var item = items.Find(_=>_.uid == uid);
        if(item is null){
            return null;
        }
        switch(property){
            case"code":
            item.code = newvalue.ToString();
            break;
            case"description":
            item.description = newvalue.ToString();
            break;
            case"short_description":
            item.short_description = newvalue.ToString();
            break;
            case"upc_code":
            item.upc_code = newvalue.ToString();
            break;
            case"model_number":
            item.model_number = newvalue.ToString();
            break;
            case"commodity_code":
            item.commodity_code = newvalue.ToString();
            break;
            case"item_line":
            item.item_line = (int)newvalue;
            break;
            case"item_group":
            item.item_group = (int) newvalue;
            break;
            case"item_type":
            item.item_type = (int)newvalue;
            break;
            case"unit_purchase_quantity":
            item.unit_purchase_quantity = (int)newvalue;
            break;
            case"unit_order_quantity":
            item.unit_order_quantity = (int)newvalue;
            break;
            case"pack_order_quantity":
            item.pack_order_quantity = (int)newvalue;
            break;
            case"supplier_id":
            item.supplier_id = (int)newvalue;
            break;
            case"supplier_code":
            item.supplier_code = newvalue.ToString();
            break;
            case"supplier_part_number":
            item.supplier_part_number = newvalue.ToString();
            break;
        }
        item.updated_at = DateTime.ParseExact(formattednow, "yyyy-MM-dd HH:mm:ss", null);
        var json = JsonConvert.SerializeObject(items, Formatting.Indented);
        var path = "data/items.json";
        File.WriteAllText(path, json);
        return item;
    }
    // Method to delete an item
    public void DeleteItem(string uid)
    {
        var items = GetAllItems();
        var item = items.FirstOrDefault(i => i.uid == uid);
        if (item == null)
        {
            return;
        }

        items.Remove(item);

        var path = "data/items.json";
        var updatedJsonData = JsonConvert.SerializeObject(items, Formatting.Indented);
        File.WriteAllText(path, updatedJsonData);
    }

    // Method to delete multiple items
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

        var path = "data/items.json";
        var updatedJsonData = JsonConvert.SerializeObject(items, Formatting.Indented);
        File.WriteAllText(path, updatedJsonData);
    }
}
