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
}
