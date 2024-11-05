using System;
using System.Collections.Generic;
using Microsoft.VisualBasic;
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
        var inventories = GetAllInventories();
        var inventoryToUpdate = inventories.Find(_ => _.Id == id);
        var currentDateTime = DateTime.Now;
        var formattedDateTime = currentDateTime.ToString("yyyy-MM-dd HH:mm:ss");
        if(inventoryToUpdate is null){
            return null;
        }
        inventoryToUpdate.description = updatedinventory.description;
        inventoryToUpdate.updated_at = DateTime.ParseExact(formattedDateTime, "yyyy-MM-dd HH:mm:ss", null);
        inventoryToUpdate.item_reference = updatedinventory.item_reference;
        inventoryToUpdate.Locations = updatedinventory.Locations;
        inventoryToUpdate.total_on_hand = updatedinventory.total_on_hand;
        inventoryToUpdate.total_expected = updatedinventory.total_expected;
        inventoryToUpdate.total_ordered = updatedinventory.total_ordered;
        inventoryToUpdate.total_allocated = updatedinventory.total_allocated;
        inventoryToUpdate.total_available = updatedinventory.total_available;
        var path = "data/inventories.json";
        var json = JsonConvert.SerializeObject(inventoryToUpdate, Formatting.Indented);
        File.WriteAllText(path, json);
        return inventoryToUpdate;
    }
}