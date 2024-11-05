using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Services;

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

    public InventoryCS UpdateInventoryById(int id, InventoryCS updatedinventory){
        InventoryCS inventoryToUpdate = GetInventoryById(id);
        if(inventoryToUpdate is null){
            return null;
        }
        inventoryToUpdate.description = updatedinventory.description;
        inventoryToUpdate.updated_at = DateTime.Now;
        inventoryToUpdate.item_id = updatedinventory.item_id;
        inventoryToUpdate.item_reference = updatedinventory.item_reference;
        inventoryToUpdate.Locations = updatedinventory.Locations;
        // inventoryToUpdate. = updatedinventory;
        // inventoryToUpdate = updatedinventory;
        // inventoryToUpdate = updatedinventory;
        return inventoryToUpdate;
    }
}