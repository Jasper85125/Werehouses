using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace warehouse.Services
{
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
    }
}