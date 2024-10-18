using System;
using System.Collections.Generic;
using System.IO;

using System.Text.Json.Serialization;

public class WarehouseCS
{
    public int Id { get; set; }
    public string? Code { get; set; }
    public string? Name { get; set; }
    public string? Address { get; set; }
    public string? Zip { get; set; }
    public string? City { get; set; }
    public string? Province { get; set; }
    public string? Country { get; set; }
    public Dictionary<string, string> Contact { get; set; }
    public DateTime created_at { get; set; }
    public DateTime updated_at { get; set; }
}

// public class WarehousesCS : BaseCS
// {
//     private string dataPath;
//     private List<WarehouseCS> data;

//     public WarehousesCS(string rootPath, bool isDebug = false)
//     {
//         dataPath = Path.Combine(rootPath, "warehouses.json");
//         LoadCS(isDebug);
//     }

//     public List<WarehouseCS> GetWarehousesCS()
//     {
//         return data;
//     }

//     public WarehouseCS GetWarehouseCS(int warehouseId)
//     {
//         return data.Find(x => x.Id == warehouseId);
//     }

//     public void AddWarehouseCS(WarehouseCS warehouse)
//     {
//         warehouse.CreatedAt = GetTimestampCS();
//         warehouse.UpdatedAt = GetTimestampCS();
//         data.Add(warehouse);
//     }

//     public void UpdateWarehouseCS(int warehouseId, WarehouseCS warehouse)
//     {
//         warehouse.UpdatedAt = GetTimestampCS();
//         int index = data.FindIndex(x => x.Id == warehouseId);
//         if (index != -1)
//         {
//             data[index] = warehouse;
//         }
//     }

//     public void RemoveWarehouseCS(int warehouseId)
//     {
//         data.RemoveAll(x => x.Id == warehouseId);
//     }

//     private void LoadCS(bool isDebug)
//     {
//         if (isDebug)
//         {
//             data = new List<WarehouseCS>();
//         }
//         else
//         {
//             if (File.Exists(dataPath))
//             {
//                 string json = File.ReadAllText(dataPath);
//                 data = JsonConvert.DeserializeObject<List<WarehouseCS>>(json);
//             }
//             else
//             {
//                 data = new List<WarehouseCS>();
//             }
//         }
//     }

//     public void SaveCS()
//     {
//         string json = JsonConvert.SerializeObject(data, Formatting.Indented);
//         File.WriteAllText(dataPath, json);
//     }
// }
