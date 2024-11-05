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
    
}