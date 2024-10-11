using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

public class Order
{
    public int Id { get; set; }
    public int SourceId { get; set; }
    public string? OrderDate { get; set; }
    public string? RequestDate { get; set; }
    public string? Reference { get; set; }
    public string? ReferenceExtra { get; set;}
    public string? OrderStatus { get; set; }
    public string? Notes { get; set;}
    public string? ShippingNotes { get; set; }
    public string? PickingNotes { get; set; }
    public string? WarehouseId { get; set; }
    public int ShipTo { get; set; }
    public int BillTo { get; set; }
    public int ShipmentId { get; set; }
    public double TotalAmount { get; set; }
    public double TotalDiscount { get; set; }
    public double TotalTax { get; set; }
    public double TotalSurcharge { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<ItemCS> Items { get; set; }
}
public class OrdersCS : BaseCS
{
    private string dataPath;
    private List<Order> data;

    public OrdersCS(string rootPath, bool isDebug = false)
    {
        dataPath = Path.Combine(rootPath, "orders.json");
        LoadCS(isDebug);
    }

    public List<Order> GetOrdersCS()
    {
        return data;
    }

    public Order GetOrderCS(int orderId)
    {
        return data.Find(x => x.Id == orderId);
    }

    public List<ItemCS> GetItemsInOrderCS(int orderId)
    {
        var order = GetOrderCS(orderId);
        return order?.Items;
    }

    public List<int> GetOrdersInShipmentCS(int shipmentId)
    {
        var result = new List<int>();
        foreach (var order in data)
        {
            if (order.ShipmentId == shipmentId)
            {
                result.Add(order.Id);
            }
        }
        return result;
    }

    public List<Order> GetOrdersForClientCS(int clientId)
    {
        var result = new List<Order>();
        foreach (var order in data)
        {
            if (order.ShipTo == clientId || order.BillTo == clientId)
            {
                result.Add(order);
            }
        }
        return result;
    }

    public void AddOrderCS(Order order)
    {
        order.CreatedAt = GetTimestampCS();
        order.UpdatedAt = GetTimestampCS();
        data.Add(order);
    }

    public void UpdateOrderCS(int orderId, Order order)
    {
        order.UpdatedAt = GetTimestampCS();
        var index = data.FindIndex(x => x.Id == orderId);
        if (index != -1)
        {
            data[index] = order;
        }
    }

    public void UpdateItemsInOrderCS(int orderId, List<ItemCS> items)
    {
        var order = GetOrderCS(orderId);
        var current = order.Items;

        foreach (var x in current)
        {
            bool found = items.Exists(y => y.Uid == x.Uid);
            if (!found)
            {
                var inventories = DataProvider.FetchInventoryPool().GetInventoriesForItem(x.ItemId);
                var minInventory = inventories.OrderBy(z => z.TotalAllocated).First();
                minInventory.TotalAllocated -= x.Amount;
                minInventory.TotalExpected = x.TotalOnHand + x.TotalOrdered;
                DataProvider.FetchInventoryPool().UpdateInventory(minInventory.Id, minInventory);
            }
        }

        foreach (var x in current)
        {
            foreach (var y in items)
            {
                if (x.Uid == y.Uid)
                {
                    var inventories = DataProvider.FetchInventoryPool().GetInventoriesForItem(x.ItemId);
                    var minInventory = inventories.OrderBy(z => z.TotalAllocated).First();
                    minInventory.TotalAllocated += y.Amount - x.Amount;
                    minInventory.TotalExpected = y.TotalOnHand + y.TotalOrdered;
                    DataProvider.FetchInventoryPool().UpdateInventory(minInventory.Id, minInventory);
                }
            }
        }

        order.Items = items;
        UpdateOrderCS(orderId, order);
    }

    public void UpdateOrdersInShipmentCS(int shipmentId, List<int> orders)
    {
        var packedOrders = GetOrdersInShipmentCS(shipmentId);
        foreach (var x in packedOrders)
        {
            if (!orders.Contains(x))
            {
                var order = GetOrderCS(x);
                order.ShipmentId = -1;
                order.OrderStatus = "Scheduled";
                UpdateOrderCS(x, order);
            }
        }

        foreach (var x in orders)
        {
            var order = GetOrderCS(x);
            order.ShipmentId = shipmentId;
            order.OrderStatus = "Packed";
            UpdateOrderCS(x, order);
        }
    }

    public void RemoveOrderCS(int orderId)
    {
        var order = GetOrderCS(orderId);
        if (order != null)
        {
            data.Remove(order);
        }
    }

    private void LoadCS(bool isDebug)
    {
        if (isDebug)
        {
            data = new List<Order>(); // Replace with actual debug data if available
        }
        else
        {
            using (var reader = new StreamReader(dataPath))
            {
                var json = reader.ReadToEnd();
                data = JsonConvert.DeserializeObject<List<Order>>(json);
            }
        }
    }

    public void SaveCS()
    {
        using (var writer = new StreamWriter(dataPath))
        {
            var json = JsonConvert.SerializeObject(data, Formatting.Indented);
            writer.Write(json);
        }
    }
}

public class Item
{
    public int ItemId { get; set; }
    public int Amount { get; set; }
    public int TotalOnHand { get; set; }
    public int TotalOrdered { get; set; }
}

public static class DataProvider
{
    public static InventoryPool FetchInventoryPool()
    {
        return new InventoryPool();
    }
}

public class InventoryPool
{
    public List<InventoryCS> GetInventoriesForItem(int itemId)
    {
        return new List<InventoryCS>(); // Replace with actual implementation
    }

    public void UpdateInventory(int id, InventoryCS inventory)
    {
        // Replace with actual implementation
    }
}

public class Inventory
{
    public int Id { get; set; }
    public int TotalAllocated { get; set; }
    public int TotalExpected { get; set; }
}
