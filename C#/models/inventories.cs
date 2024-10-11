using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

public class InventoryCS
{
    public int Id { get; set; }
    public int ItemId { get; set; }
    public string? Description { get; set; }
    public string? ItemReference { get; set; }
    public List<int> Locations {get; set;}
    public int TotalOnHand { get; set; }
    public int TotalExpected { get; set; }
    public int TotalOrdered { get; set; }
    public int TotalAllocated { get; set; }
    public int TotalAvailable { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class InventoriesCS
{
    private string dataPath;
    private List<InventoryCS> data;

    public InventoriesCS(string rootPath, bool isDebug = false)
    {
        dataPath = Path.Combine(rootPath, "inventories.json");
        LoadCS(isDebug);
    }

    public List<InventoryCS> GetInventoriesCS()
    {
        return data;
    }

    public InventoryCS GetInventoryCS(int inventoryId)
    {
        return data.Find(x => x.Id == inventoryId);
    }

    public List<InventoryCS> GetInventoriesForItemCS(int itemId)
    {
        return data.FindAll(x => x.ItemId == itemId);
    }

    public Dictionary<string, int> GetInventoryTotalsForItemCS(int itemId)
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

    public void AddInventoryCS(InventoryCS inventory)
    {
        inventory.CreatedAt = DateTime.Now;
        inventory.UpdatedAt = DateTime.Now;
        data.Add(inventory);
    }

    public void UpdateInventoryCS(int inventoryId, InventoryCS inventory)
    {
        inventory.UpdatedAt = DateTime.Now;
        var index = data.FindIndex(x => x.Id == inventoryId);
        if (index != -1)
        {
            data[index] = inventory;
        }
    }

    public void RemoveInventoryCS(int inventoryId)
    {
        data.RemoveAll(x => x.Id == inventoryId);
    }

    private void LoadCS(bool isDebug)
    {
        if (isDebug)
        {
            data = new List<InventoryCS>();
        }
        else
        {
            if (File.Exists(dataPath))
            {
                var jsonData = File.ReadAllText(dataPath);
                data = JsonConvert.DeserializeObject<List<InventoryCS>>(jsonData);
            }
            else
            {
                data = new List<InventoryCS>();
            }
        }
    }

    public void SaveCS()
    {
        var jsonData = JsonConvert.SerializeObject(data, Formatting.Indented);
        File.WriteAllText(dataPath, jsonData);
    }
}
