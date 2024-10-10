using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

public class Warehouse
{
    public int Id { get; set; }
    public string Name { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class Warehouses : Base
{
    private string dataPath;
    private List<Warehouse> data;

    public Warehouses(string rootPath, bool isDebug = false)
    {
        dataPath = Path.Combine(rootPath, "warehouses.json");
        Load(isDebug);
    }

    public List<Warehouse> GetWarehouses()
    {
        return data;
    }

    public Warehouse GetWarehouse(int warehouseId)
    {
        return data.Find(x => x.Id == warehouseId);
    }

    public void AddWarehouse(Warehouse warehouse)
    {
        warehouse.CreatedAt = GetTimestamp();
        warehouse.UpdatedAt = GetTimestamp();
        data.Add(warehouse);
    }

    public void UpdateWarehouse(int warehouseId, Warehouse warehouse)
    {
        warehouse.UpdatedAt = GetTimestamp();
        int index = data.FindIndex(x => x.Id == warehouseId);
        if (index != -1)
        {
            data[index] = warehouse;
        }
    }

    public void RemoveWarehouse(int warehouseId)
    {
        data.RemoveAll(x => x.Id == warehouseId);
    }

    private void Load(bool isDebug)
    {
        if (isDebug)
        {
            data = new List<Warehouse>();
        }
        else
        {
            if (File.Exists(dataPath))
            {
                string json = File.ReadAllText(dataPath);
                data = JsonConvert.DeserializeObject<List<Warehouse>>(json);
            }
            else
            {
                data = new List<Warehouse>();
            }
        }
    }

    public void Save()
    {
        string json = JsonConvert.SerializeObject(data, Formatting.Indented);
        File.WriteAllText(dataPath, json);
    }

    private DateTime GetTimestamp()
    {
        return DateTime.Now;
    }
}
