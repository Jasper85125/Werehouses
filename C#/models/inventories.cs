using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

public class Inventory
{
    public int Id { get; set; }
    public int ItemId { get; set; }
    public int TotalExpected { get; set; }
    public int TotalOrdered { get; set; }
    public int TotalAllocated { get; set; }
    public int TotalAvailable { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class Inventories
{
    private string dataPath;
    private List<Inventory> data;

    public Inventories(string rootPath, bool isDebug = false)
    {
        dataPath = Path.Combine(rootPath, "inventories.json");
        Load(isDebug);
    }

    public List<Inventory> GetInventories()
    {
        return data;
    }

    public Inventory GetInventory(int inventoryId)
    {
        return data.Find(x => x.Id == inventoryId);
    }

    public List<Inventory> GetInventoriesForItem(int itemId)
    {
        return data.FindAll(x => x.ItemId == itemId);
    }

    public Dictionary<string, int> GetInventoryTotalsForItem(int itemId)
    {
        var result = new Dictionary<string, int>
        {
            { "total_expected", 0 },
            { "total_ordered", 0 },
            { "total_allocated", 0 },
            { "total_available", 0 }
        };

        foreach (var x in data)
        {
            if (x.ItemId == itemId)
            {
                result["total_expected"] += x.TotalExpected;
                result["total_ordered"] += x.TotalOrdered;
                result["total_allocated"] += x.TotalAllocated;
                result["total_available"] += x.TotalAvailable;
            }
        }

        return result;
    }

    public void AddInventory(Inventory inventory)
    {
        inventory.CreatedAt = DateTime.Now;
        inventory.UpdatedAt = DateTime.Now;
        data.Add(inventory);
    }

    public void UpdateInventory(int inventoryId, Inventory inventory)
    {
        inventory.UpdatedAt = DateTime.Now;
        var index = data.FindIndex(x => x.Id == inventoryId);
        if (index != -1)
        {
            data[index] = inventory;
        }
    }

    public void RemoveInventory(int inventoryId)
    {
        data.RemoveAll(x => x.Id == inventoryId);
    }

    private void Load(bool isDebug)
    {
        if (isDebug)
        {
            data = new List<Inventory>();
        }
        else
        {
            if (File.Exists(dataPath))
            {
                var jsonData = File.ReadAllText(dataPath);
                data = JsonConvert.DeserializeObject<List<Inventory>>(jsonData);
            }
            else
            {
                data = new List<Inventory>();
            }
        }
    }

    public void Save()
    {
        var jsonData = JsonConvert.SerializeObject(data, Formatting.Indented);
        File.WriteAllText(dataPath, jsonData);
    }
}
