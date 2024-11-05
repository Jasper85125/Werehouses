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

    public async Task<ShipmentCS> UpdateShipment(int id, ShipmentCS updateShipment)
    {
        List<ShipmentCS> shipments = GetAllShipments();
        var existingShipment = shipments.FirstOrDefault(s => s.Id == id);
        if (existingShipment == null)
        {
            return null;
        }

        // Get the current date and time
        var currentDateTime = DateTime.Now;

        // Format the date and time to the desired format
        var formattedDateTime = currentDateTime.ToString("yyyy-MM-dd HH:mm:ss");

        // Update the existing shipment with new values
        existingShipment.order_id = updateShipment.order_id;
        existingShipment.source_id = updateShipment.source_id;
        existingShipment.order_date = updateShipment.order_date;
        existingShipment.request_date = updateShipment.request_date;
        existingShipment.shipment_date = updateShipment.shipment_date;
        existingShipment.shipment_type = updateShipment.shipment_type;
        existingShipment.shipment_status = updateShipment.shipment_status;
        existingShipment.Notes = updateShipment.Notes;
        existingShipment.carrier_code = updateShipment.carrier_code;
        existingShipment.carrier_description = updateShipment.carrier_description;
        existingShipment.service_code = updateShipment.service_code;
        existingShipment.payment_type = updateShipment.payment_type;
        existingShipment.transfer_mode = updateShipment.transfer_mode;
        existingShipment.total_package_count = updateShipment.total_package_count;
        existingShipment.total_package_weight = updateShipment.total_package_weight;
        existingShipment.Items = updateShipment.Items;
        existingShipment.updated_at = DateTime.ParseExact(formattedDateTime, "yyyy-MM-dd HH:mm:ss", null);

        var jsonData = JsonConvert.SerializeObject(shipments, Formatting.Indented);
        await File.WriteAllTextAsync("data/shipments.json", jsonData);

        return existingShipment;
    }
    public void DeleteShipment(int id)
    {
        var path = "data/shipments.json";
        List<ShipmentCS> shipments = GetAllShipments();
        var shipment = shipments.FirstOrDefault(s => s.Id == id);
        if (shipment != null)
        {
            shipments.Remove(shipment);
            var jsonData = JsonConvert.SerializeObject(shipments, Formatting.Indented);
            File.WriteAllText(path, jsonData);
        }
    }
    public void DeleteItemFromShipment(int shipmentId, string itemId)
    {
        var path = "data/shipments.json";
        List<ShipmentCS> shipments = GetAllShipments();
        var shipment = shipments.FirstOrDefault(s => s.Id == shipmentId);
        if (shipment != null)
        {
            var item = shipment.Items.FirstOrDefault(i => i.item_id == itemId);
            if (item != null)
            {
                shipment.Items.Remove(item);
                var jsonData = JsonConvert.SerializeObject(shipments, Formatting.Indented);
                File.WriteAllText(path, jsonData);
            }
        }
    }
}
