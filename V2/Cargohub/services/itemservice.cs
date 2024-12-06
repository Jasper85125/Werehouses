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
