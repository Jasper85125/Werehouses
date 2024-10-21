using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Services;

public class WarehouseService : IWarehouseService
{
    // Constructor
    public WarehouseService()
    {
        // Initialization code here
    }

    public List<WarehouseCS> GetAllWarehouses()
    {
        var Path = "data/warehouses.json";
        if (!File.Exists(Path))
        {
            return new List<WarehouseCS>();
        }
        var jsonData = File.ReadAllText(Path);
        List<WarehouseCS> warehouses = JsonConvert.DeserializeObject<List<WarehouseCS>>(jsonData);
        return warehouses ?? new List<WarehouseCS>();
    }

    public WarehouseCS GetWarehouseById(int id)
    {
        List<WarehouseCS> warehouses = GetAllWarehouses();
        WarehouseCS warehouse = warehouses.FirstOrDefault(ware => ware.Id == id);
        return warehouse;
    }

    public void CreateWarehouse(WarehouseCS newWarehouse)
    {
        var Path = "data/warehouses.json";

        List<WarehouseCS> warehouses = GetAllWarehouses();

        // Add the new warehouse record to the list
        newWarehouse.Id = warehouses.Count > 0 ? warehouses.Max(w => w.Id) + 1 : 1;
        warehouses.Add(newWarehouse);

        // Serialize the updated list back to the JSON file
        var jsonData = JsonConvert.SerializeObject(warehouses, Formatting.Indented);
        File.WriteAllText(Path, jsonData);
    }


}
