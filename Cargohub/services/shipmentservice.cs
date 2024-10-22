using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Services;

public class ShipmentService : IShipmentService
{
    // Constructor
    public ShipmentService()
    {
        // Initialization code here
    }

    public List<ShipmentCS> GetAllShipments()
    {
        var Path = "data/shipments.json";
        if (!File.Exists(Path))
        {
            return new List<ShipmentCS>();
        }
        var jsonData = File.ReadAllText(Path);
        List<ShipmentCS> shipments = JsonConvert.DeserializeObject<List<ShipmentCS>>(jsonData);
        return shipments ?? new List<ShipmentCS>();
    }

    public ShipmentCS GetShipmentById(int id)
    {
        List<ShipmentCS> shipments = GetAllShipments();
        ShipmentCS shipment = shipments.FirstOrDefault(ship => ship.Id == id);
        return shipment;
    }

    public ShipmentCS CreateShipment(ShipmentCS newShipment)
    {
        var Path = "data/shipments.json";

        List<ShipmentCS> shipments = GetAllShipments();

        // Add the new shipment record to the list
        newShipment.Id = shipments.Count > 0 ? shipments.Max(w => w.Id) + 1 : 1;
        shipments.Add(newShipment);

        // Serialize the updated list back to the JSON file
        var jsonData = JsonConvert.SerializeObject(shipments, Formatting.Indented);
        File.WriteAllText(Path, jsonData);
        return newShipment;
    }
}
