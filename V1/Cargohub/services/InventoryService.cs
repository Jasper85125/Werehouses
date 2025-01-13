using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ServicesV1;

public class InventoryService : IInventoryService
{
    // Constructor
    public InventoryService()
    {
        // Initialization code here
    }

    public List<InventoryCS> GetAllInventories()
    {
        var Path = "data/inventories.json";
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

    public InventoryCS CreateInventory(InventoryCS newInventory)
    {
        var path = "data/inventories.json";

        List<InventoryCS> inventories = GetAllInventories();


        newInventory.Id = inventories.Count > 0 ? inventories.Max(i => i.Id) + 1 : 1;
        inventories.Add(newInventory);


        var jsonData = JsonConvert.SerializeObject(inventories, Formatting.Indented);
        File.WriteAllText(path, jsonData);
        return newInventory;
        
    }
    public void DeleteInventory(int id)
    {
        var path = "data/inventories.json";
        List<InventoryCS> inventories = GetAllInventories();
        InventoryCS inventory = inventories.FirstOrDefault(inv => inv.Id == id);
        if (inventory == null)
        {
            return;
        }
        inventories.Remove(inventory);
        var jsonData = JsonConvert.SerializeObject(inventories, Formatting.Indented);
        File.WriteAllText(path, jsonData);
    }
    public InventoryCS UpdateInventoryById(int id, InventoryCS updatedinventory){
        var currentDateTime = DateTime.Now;
        var formattedDateTime = currentDateTime.ToString("yyyy-MM-dd HH:mm:ss");
        var inventories = GetAllInventories();
        var toUpdate = inventories.Find(_ => _.Id == id);
        if(toUpdate is null)
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
        var path = "data/inventories.json";
        var json = JsonConvert.SerializeObject(inventories);
        File.WriteAllText(path, json);
        return toUpdate;
    }
}
