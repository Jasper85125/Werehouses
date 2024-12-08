using System;
using System.Collections.Generic;
using ApiKeyAuthentication.Authentication;
using Newtonsoft.Json;

namespace ServicesV2;

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

        ApiKeyModel newInventoryManager = new ApiKeyModel {Key = $"InventoryManagerKey{newWarehouse.Id}", Role = "Inventory Manager", WarehouseID = newWarehouse.Id};
        ApiKeyModel newFloorManager = new ApiKeyModel {Key = $"FloorManagerKey{newWarehouse.Id}", Role = "Floor Manager", WarehouseID = newWarehouse.Id};
        ApiKeyModel newOperative = new ApiKeyModel {Key = $"OperativeKey{newWarehouse.Id}", Role = "Operative", WarehouseID = newWarehouse.Id};
        ApiKeyModel newSupervisor = new ApiKeyModel {Key = $"Supervisor{newWarehouse.Id}", Role = "Supervisor", WarehouseID = newWarehouse.Id};
        var apikeys = ApiKeyStorage.GetApiKeys();
        apikeys.Add(newInventoryManager);
        apikeys.Add(newFloorManager);
        apikeys.Add(newOperative);
        apikeys.Add(newSupervisor);
        ApiKeyStorage.UpdateApiKey(apikeys);

        

        // Serialize the updated list back to the JSON file
        var jsonData = JsonConvert.SerializeObject(warehouses, Formatting.Indented);
        File.WriteAllText(_path, jsonData);
        return newWarehouse;
    }

    public List<WarehouseCS> CreateMultipleWarehouse(List<WarehouseCS>newWarehouse)
    {
        List<WarehouseCS> addedWarehouses = new List<WarehouseCS>();
        foreach(WarehouseCS warehouse in newWarehouse)
        {
            WarehouseCS addWarehouse = CreateWarehouse(warehouse);
            addedWarehouses.Add(addWarehouse);
        }
        return addedWarehouses;
    }

    public WarehouseCS UpdateWarehouse(int id, WarehouseCS updateWarehouse)
    {
        var allWarehouses = GetAllWarehouses();
        var warehouseToUpdate = allWarehouses.Single(warehouse => warehouse.Id == id);

        if (warehouseToUpdate is not null)
        {
            // Get the current date and time
            var currentDateTime = DateTime.Now;

            // Format the date and time to the desired format
            var formattedDateTime = currentDateTime.ToString("yyyy-MM-dd HH:mm:ss");

            warehouseToUpdate.Code = updateWarehouse.Code;
            warehouseToUpdate.Name = updateWarehouse.Name;
            warehouseToUpdate.Address = updateWarehouse.Address;
            warehouseToUpdate.Zip = updateWarehouse.Zip;
            warehouseToUpdate.City = updateWarehouse.City;
            warehouseToUpdate.Province = updateWarehouse.Province;
            warehouseToUpdate.Country = updateWarehouse.Country;
            warehouseToUpdate.Contact = updateWarehouse.Contact;
            warehouseToUpdate.updated_at = DateTime.ParseExact(formattedDateTime, "yyyy-MM-dd HH:mm:ss", null);


            var jsonData = JsonConvert.SerializeObject(allWarehouses, Formatting.Indented);
            File.WriteAllText(_path, jsonData);
            return warehouseToUpdate;
        }
        return null;
    }
    public void DeleteWarehouse(int id){
        var allWarehouses = GetAllWarehouses();
        var warehouseToDelete = allWarehouses.FirstOrDefault(warehouse => warehouse.Id == id);
        if(warehouseToDelete == null){
            return;
        }
        allWarehouses.Remove(warehouseToDelete);
        var jsonData = JsonConvert.SerializeObject(allWarehouses, Formatting.Indented);
        File.WriteAllText(_path, jsonData);
    }
    public void DeleteWarehouses(List<int> ids){
        var warehouses = GetAllWarehouses();
        foreach(int id in ids){
            var warehouse = warehouses.Find(_=>_.Id == id);
            if(warehouse is not null){
                warehouses.Remove(warehouse);
            }
        }
        var json = JsonConvert.SerializeObject(warehouses, Formatting.Indented);
        File.WriteAllText(_path, json);
    }
}