using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ServicesV1;

public class ShipmentService : IShipmentService
{
    // Constructor
    private string Path = "../../data/shipments.json";
    public ShipmentService()
    {
        // Initialization code here
    }

    public List<ShipmentCS> GetAllShipments()
    {
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

    public List<ItemIdAndAmount> GetItemsInShipment(int shipmentId)
    {
        List<ShipmentCS> shipments = GetAllShipments();
        var shipment = shipments.FirstOrDefault(s => s.Id == shipmentId);
        if (shipment != null)
        {
            return shipment.Items;
        }
        return null;
    }
    public ShipmentCS CreateShipment(ShipmentCS newShipment)
    {

        List<ShipmentCS> shipments = GetAllShipments();

        // Add the new shipment record to the list
        newShipment.Id = shipments.Count > 0 ? shipments.Max(w => w.Id) + 1 : 1;
        shipments.Add(newShipment);

        // Serialize the updated list back to the JSON file
        var jsonData = JsonConvert.SerializeObject(shipments, Formatting.Indented);
        File.WriteAllText(Path, jsonData);
        return newShipment;
    }

    public ShipmentCS UpdateShipment(int id, ShipmentCS updateShipment)
    {
        List<ShipmentCS> shipments = GetAllShipments();
        var existingShipment = shipments.FirstOrDefault(s => s.Id == id);
        if (existingShipment == null)
        {
            return null;
        }

        var currentDateTime = DateTime.Now;
        var formattedDateTime = currentDateTime.ToString("yyyy-MM-dd HH:mm:ss");

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
        File.WriteAllText(Path, jsonData);

        return existingShipment;
    }


    public ShipmentCS UpdateItemsInShipment(int shipmentId, List<ItemIdAndAmount> items)
    {
        var shipments = GetAllShipments();
        var updatedShipment = shipments.Find(_ => _.Id == shipmentId);
        if (updatedShipment is null)
        {
            return null;
        }
        updatedShipment.Items = items;
        var json = JsonConvert.SerializeObject(shipments, Formatting.Indented);
        File.WriteAllText(Path, json);
        return updatedShipment;
    }
    public void DeleteShipment(int id)
    {
        List<ShipmentCS> shipments = GetAllShipments();
        var shipment = shipments.FirstOrDefault(s => s.Id == id);
        if (shipment != null)
        {
            shipments.Remove(shipment);
            var jsonData = JsonConvert.SerializeObject(shipments, Formatting.Indented);
            File.WriteAllText(Path, jsonData);
        }
    }
    public void DeleteItemFromShipment(int shipmentId, string itemId)
    {
        List<ShipmentCS> shipments = GetAllShipments();
        var shipment = shipments.FirstOrDefault(s => s.Id == shipmentId);
        if (shipment != null)
        {
            var item = shipment.Items.FirstOrDefault(i => i.item_id == itemId);
            if (item != null)
            {
                shipment.Items.Remove(item);
                var jsonData = JsonConvert.SerializeObject(shipments, Formatting.Indented);
                File.WriteAllText(Path, jsonData);
            }
        }
    }
}
