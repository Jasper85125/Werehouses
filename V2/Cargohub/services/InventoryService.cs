using System;
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



    public InventoryCS PatchInventory(int id, InventoryCS patchInventory)
    {
        var currentDateTime = DateTime.Now;
        var formattedDateTime = currentDateTime.ToString("yyyy-MM-dd HH:mm:ss");
        var inventories = GetAllInventories();
        var toPatch = inventories.Find(_ => _.Id == id);
        if (toPatch is null)
        {
            return null;
        }

        if (patchInventory.description != null)
        {
            toPatch.description = patchInventory.description;
        }
        if (patchInventory.item_reference != null)
        {
            toPatch.item_reference = patchInventory.item_reference;
        }
        if (patchInventory.Locations != null)
        {
            toPatch.Locations = patchInventory.Locations;
        }
        if (patchInventory.total_on_hand != 0)
        {
            toPatch.total_on_hand = patchInventory.total_on_hand;
        }
        if (patchInventory.total_expected != 0)
        {
            toPatch.total_expected = patchInventory.total_expected;
        }
        if (patchInventory.total_ordered != 0)
        {
            toPatch.total_ordered = patchInventory.total_ordered;
        }
        if (patchInventory.total_allocated != 0)
        {
            toPatch.total_allocated = patchInventory.total_allocated;
        }
        if (patchInventory.total_available != 0)
        {
            toPatch.total_available = patchInventory.total_available;
        }

        patchInventory.updated_at = DateTime.ParseExact(formattedDateTime, "yyyy-MM-dd HH:mm:ss", null);

        var json = JsonConvert.SerializeObject(inventories, Formatting.Indented);
        File.WriteAllText(Path, json);

        return toPatch;
    }
}
