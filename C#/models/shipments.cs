using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

public class ShipmentCS
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public int SourceId { get; set; }
    public DateTime OrderDate { get; set; }
    public DateTime RequestDate { get; set; }
    public DateTime ShipmentDate { get; set; }
    public string ShipmentType { get; set; }
    public string ShipmentStatus { get; set; }
    public string Notes { get; set; }
    public string CarrierCode { get; set; }
    public string CarrierDescription { get; set; }
    public string ServiceCode { get; set; }
    public string PaymentType { get; set; }
    public string TransferMode { get; set; }
    public int TotalPackageCount { get; set; }
    public double TotalPackageWeight { get; set; }
    public List<Item> Items { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class ShipmentsCS : BaseCS
{
    private string dataPath;
    private List<ShipmentCS> data;

    public ShipmentsCS(string rootPath, bool isDebug = false)
    {
        dataPath = Path.Combine(rootPath, "shipments.json");
        LoadCS(isDebug);
    }

    public List<ShipmentCS> GetShipmentsCS()
    {
        return data;
    }

    public ShipmentCS GetShipmentCS(int shipmentId)
    {
        return data.Find(x => x.Id == shipmentId);
    }

    public List<Item> GetItemsInShipmentCS(int shipmentId)
    {
        var shipment = GetShipmentCS(shipmentId);
        return shipment?.Items;
    }

    public void AddShipmentCS(ShipmentCS shipment)
    {
        shipment.CreatedAt = DateTime.Now;
        shipment.UpdatedAt = DateTime.Now;
        data.Add(shipment);
    }

    public void UpdateShipmentCS(int shipmentId, ShipmentCS shipment)
    {
        shipment.UpdatedAt = DateTime.Now;
        var index = data.FindIndex(x => x.Id == shipmentId);
        if (index != -1)
        {
            data[index] = shipment;
        }
    }

    public void UpdateItemsInShipmentCS(int shipmentId, List<Item> items)
    {
        var shipment = GetShipmentCS(shipmentId);
        if (shipment == null) return;

        var currentItems = shipment.Items;
        foreach (var currentItem in currentItems)
        {
            var found = items.Exists(x => x.ItemId == currentItem.ItemId);
            if (!found)
            {
                var inventories = DataProvider.FetchInventoryPool().GetInventoriesForItem(currentItem.ItemId);
                var maxInventory = inventories.OrderByDescending(x => x.TotalOrdered).FirstOrDefault();
                if (maxInventory != null)
                {
                    maxInventory.TotalOrdered -= currentItem.Amount;
                    maxInventory.TotalExpected = currentItem.TotalOnHand + currentItem.TotalOrdered;
                    DataProvider.FetchInventoryPool().UpdateInventory(maxInventory.Id, maxInventory);
                }
            }
        }

        foreach (var currentItem in currentItems)
        {
            foreach (var newItem in items)
            {
                if (currentItem.ItemId == newItem.ItemId)
                {
                    var inventories = DataProvider.FetchInventoryPool().GetInventoriesForItem(currentItem.ItemId);
                    var maxInventory = inventories.OrderByDescending(x => x.TotalOrdered).FirstOrDefault();
                    if (maxInventory != null)
                    {
                        maxInventory.TotalOrdered += newItem.Amount - currentItem.Amount;
                        maxInventory.TotalExpected = newItem.TotalOnHand + newItem.TotalOrdered;
                        DataProvider.FetchInventoryPool().UpdateInventory(maxInventory.Id, maxInventory);
                    }
                }
            }
        }

        shipment.Items = items;
        UpdateShipmentCS(shipmentId, shipment);
    }

    public void RemoveShipmenCSt(int shipmentId)
    {
        var shipment = GetShipmentCS(shipmentId);
        if (shipment != null)
        {
            data.Remove(shipment);
        }
    }

    private void LoadCS(bool isDebug)
    {
        if (isDebug)
        {
            data = new List<Shipment>(); // Assuming SHIPMENTS is an empty list
        }
        else
        {
            var jsonData = File.ReadAllText(dataPath);
            data = JsonConvert.DeserializeObject<List<Shipment>>(jsonData);
        }
    }

    public void SaveCS()
    {
        var jsonData = JsonConvert.SerializeObject(data, Formatting.Indented);
        File.WriteAllText(dataPath, jsonData);
    }
}
