using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Services;

public class WarehouseService : IWarehouseService
{
    private string _path = "data/warehouses.json";
    public WarehouseService()
    {
        // Initialization code here
    }

    public List<WarehouseCS> GetAllWarehouses()
    {
        if (!File.Exists(_path))
        {
            return new List<WarehouseCS>();
        }
        var jsonData = File.ReadAllText(_path);
        List<WarehouseCS> warehouses = JsonConvert.DeserializeObject<List<WarehouseCS>>(jsonData);
        return warehouses ?? new List<WarehouseCS>();
    }

    public WarehouseCS GetWarehouseById(int id)
    {
        List<WarehouseCS> warehouses = GetAllWarehouses();
        WarehouseCS warehouse = warehouses.FirstOrDefault(ware => ware.Id == id);
        return warehouse;
    }

    public WarehouseCS CreateWarehouse(WarehouseCS newWarehouse)
    {
        List<WarehouseCS> warehouses = GetAllWarehouses();

        // Add the new warehouse record to the list
        newWarehouse.Id = warehouses.Count > 0 ? warehouses.Max(w => w.Id) + 1 : 1;
        warehouses.Add(newWarehouse);

        // Serialize the updated list back to the JSON file
        var jsonData = JsonConvert.SerializeObject(warehouses, Formatting.Indented);
        File.WriteAllText(_path, jsonData);
        return newWarehouse;
    }

    public WarehouseCS UpdateWarehouse(int id, WarehouseCS updateWarehouse)
    {
        var allWarehouses = GetAllWarehouses();
        var warehouseToUpdate = allWarehouses.Single(warehouse => warehouse.Id == id);

        if (warehouseToUpdate is not null)
        {
            warehouseToUpdate.Code = updateWarehouse.Code;
            warehouseToUpdate.Name = updateWarehouse.Name;
            warehouseToUpdate.Address = updateWarehouse.Address;
            warehouseToUpdate.Zip = updateWarehouse.Zip;
            warehouseToUpdate.City = updateWarehouse.City;
            warehouseToUpdate.Province = updateWarehouse.Province;
            warehouseToUpdate.Country = updateWarehouse.Country;
            warehouseToUpdate.Contact = updateWarehouse.Contact;
            warehouseToUpdate.updated_at = DateTime.UtcNow;

            var jsonData = JsonConvert.SerializeObject(allWarehouses, Formatting.Indented);
            File.WriteAllText(_path, jsonData);
            return warehouseToUpdate;
        }
        return null;
    }
}