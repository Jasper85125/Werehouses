using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

public class Shipment
{
    public int Id { get; set; }
    public List<Item> Items { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class Item
{
    public int ItemId { get; set; }
    public int Amount { get; set; }
    public int TotalOnHand { get; set; }
    public int TotalOrdered { get; set; }
}

public class Shipments : Base
{
    private string dataPath;
    private List<Shipment> data;

    public Shipments(string rootPath, bool isDebug = false)
    {
        dataPath = Path.Combine(rootPath, "shipments.json");
        Load(isDebug);
    }

    public List<Shipment> GetShipments()
    {
        return data;
    }

    public Shipment GetShipment(int shipmentId)
    {
        return data.Find(x => x.Id == shipmentId);
    }

    public List<Item> GetItemsInShipment(int shipmentId)
    {
        var shipment = GetShipment(shipmentId);
        return shipment?.Items;
    }

    public void AddShipment(Shipment shipment)
    {
        shipment.CreatedAt = DateTime.Now;
        shipment.UpdatedAt = DateTime.Now;
        data.Add(shipment);
    }

    public void UpdateShipment(int shipmentId, Shipment shipment)
    {
        shipment.UpdatedAt = DateTime.Now;
        var index = data.FindIndex(x => x.Id == shipmentId);
        if (index != -1)
        {
            data[index] = shipment;
        }
    }

    public void UpdateItemsInShipment(int shipmentId, List<Item> items)
    {
        var shipment = GetShipment(shipmentId);
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
        UpdateShipment(shipmentId, shipment);
    }

    public void RemoveShipment(int shipmentId)
    {
        var shipment = GetShipment(shipmentId);
        if (shipment != null)
        {
            data.Remove(shipment);
        }
    }

    private void Load(bool isDebug)
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

    public void Save()
    {
        var jsonData = JsonConvert.SerializeObject(data, Formatting.Indented);
        File.WriteAllText(dataPath, jsonData);
    }
}
