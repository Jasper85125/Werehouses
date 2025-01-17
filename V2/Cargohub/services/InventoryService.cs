using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ServicesV2;

public class InventoryService : IInventoryService
{
    private string Path = "../../data/inventories.json";
    // Constructor
    public InventoryService()
    {
        // Initialization code here
    }

    public List<InventoryCS> GetAllInventories()
    {
        if (!File.Exists(Path))
        {
            return new List<InventoryCS>();
        }
        var jsonData = File.ReadAllText(Path);
        List<InventoryCS> inventories = JsonConvert.DeserializeObject<List<InventoryCS>>(jsonData);
        return inventories ?? new List<InventoryCS>();
    }

    public InventoryCS GetInventoryById(int id)
    {
        List<InventoryCS> inventories = GetAllInventories();
        InventoryCS inventory = inventories.FirstOrDefault(inv => inv.Id == id);
        return inventory;
    }

    public InventoryCS GetInventoriesForItem(string itemId)
    {
        List<InventoryCS> inventories = GetAllInventories();
        foreach (InventoryCS inventory in inventories)
        {
            if (inventory.item_id == itemId)
            {
                return inventory;
            }
        }
        return null;

    }

    public List<InventoryCS> GetInventoriesByLocationId(List<int> location){{
        List<InventoryCS> inventories = GetAllInventories();
        return inventories.Where(inventory => inventory.Locations.Any(locationId => location.Contains(locationId))).ToList();
    }
    }

    public InventoryCS CreateInventory(InventoryCS newInventory)
    {
        List<InventoryCS> inventories = GetAllInventories();
        var currentDateTime = DateTime.Now;
        var formattedDateTime = currentDateTime.ToString("yyyy-MM-dd HH:mm:ss");

        newInventory.Id = inventories.Count > 0 ? inventories.Max(i => i.Id) + 1 : 1;
        newInventory.created_at = DateTime.ParseExact(formattedDateTime, "yyyy-MM-dd HH:mm:ss", null);
        newInventory.updated_at = DateTime.ParseExact(formattedDateTime, "yyyy-MM-dd HH:mm:ss", null);
        inventories.Add(newInventory);

        var jsonData = JsonConvert.SerializeObject(inventories, Formatting.Indented);
        File.WriteAllText(Path, jsonData);
        return newInventory;
    }

    public List<InventoryCS> CreateMultipleInventories(List<InventoryCS> newInventories)
    {
        List<InventoryCS> addedInventory = new List<InventoryCS>();
        foreach (InventoryCS inventory in newInventories)
        {
            InventoryCS addInventorty = CreateInventory(inventory);
            addedInventory.Add(addInventorty);
        }
        return addedInventory;
    }

    public void DeleteInventory(int id)
    {
        List<InventoryCS> inventories = GetAllInventories();
        InventoryCS inventory = inventories.FirstOrDefault(inv => inv.Id == id);
        if (inventory == null)
        {
            return;
        }
        inventories.Remove(inventory);
        var jsonData = JsonConvert.SerializeObject(inventories, Formatting.Indented);
        File.WriteAllText(Path, jsonData);
    }
    public InventoryCS UpdateInventoryById(int id, InventoryCS updatedinventory)
    {
        var currentDateTime = DateTime.Now;
        var formattedDateTime = currentDateTime.ToString("yyyy-MM-dd HH:mm:ss");
        var inventories = GetAllInventories();
        var toUpdate = inventories.Find(_ => _.Id == id);
        if (toUpdate is null)
        {
            return null;
        }
        toUpdate.description = updatedinventory.description;
        toUpdate.item_reference = updatedinventory.item_reference;
        toUpdate.Locations = updatedinventory.Locations;
        toUpdate.total_on_hand = updatedinventory.total_on_hand;
        toUpdate.total_expected = updatedinventory.total_expected;
        toUpdate.total_ordered = updatedinventory.total_ordered;
        toUpdate.total_allocated = updatedinventory.total_allocated;
        toUpdate.total_available = updatedinventory.total_available;
        toUpdate.updated_at = DateTime.ParseExact(formattedDateTime, "yyyy-MM-dd HH:mm:ss", null);
        var json = JsonConvert.SerializeObject(inventories);
        File.WriteAllText(Path, json);
        return toUpdate;
    }

    public void DeleteInventories(List<int> ids)
    {
        var inventories = GetAllInventories();
        foreach (int id in ids)
        {
            inventories.Remove(inventories.Find(_ => _.Id == id));
        }
        var json = JsonConvert.SerializeObject(inventories, Formatting.Indented);
        File.WriteAllText(Path, json);
    }

    public InventoryCS PatchInventory(int id, string property, object newvalue)
    {
        var currentDateTime = DateTime.Now;
        var formattedDateTime = currentDateTime.ToString("yyyy-MM-dd HH:mm:ss");
        var inventories = GetAllInventories();
        var inventory = inventories.FirstOrDefault(_=>_.Id == id);

        if (inventory is null)
        {
            return null;
        }
        switch(property){
            case "description":
                inventory.description = newvalue.ToString();
                break;
            case "item_reference":
                inventory.item_reference = newvalue.ToString();
                break;
            case "Locations":
                var patchvalue = JsonConvert.DeserializeObject<List<int>>(newvalue.ToString()).ToList();
                inventory.Locations = patchvalue;
                break;
            case "total_on_hand":
                inventory.total_on_hand = Convert.ToInt32(newvalue.ToString());
                break;
            case "total_expected":
                inventory.total_expected = Convert.ToInt32(newvalue.ToString());
                break;
            case "total_ordered":
                inventory.total_ordered = Convert.ToInt32(newvalue.ToString());
                break;
            case "total_allocated":
                inventory.total_allocated = Convert.ToInt32(newvalue.ToString());
                break;
            case "total_available":
                inventory.total_available = Convert.ToInt32(newvalue.ToString());
                break;
        }
        
        inventory.updated_at = DateTime.ParseExact(formattedDateTime, "yyyy-MM-dd HH:mm:ss", null);

        var json = JsonConvert.SerializeObject(inventories, Formatting.Indented);
        File.WriteAllText(Path, json);

        return inventory;
    }
}
