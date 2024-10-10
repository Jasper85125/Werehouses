using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

public class Orders : BaseCS
{
    private string dataPath;
    private List<Order> data;

    public Orders(string rootPath, bool isDebug = false)
    {
        dataPath = Path.Combine(rootPath, "orders.json");
        Load(isDebug);
    }

    public List<Order> GetOrders()
    {
        return data;
    }

    public Order GetOrder(int orderId)
    {
        return data.Find(x => x.Id == orderId);
    }

    public List<Item> GetItemsInOrder(int orderId)
    {
        var order = GetOrder(orderId);
        return order?.Items;
    }

    public List<int> GetOrdersInShipment(int shipmentId)
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

    public List<Order> GetOrdersForClient(int clientId)
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

    public void AddOrder(Order order)
    {
        order.CreatedAt = GetTimestamp();
        order.UpdatedAt = GetTimestamp();
        data.Add(order);
    }

    public void UpdateOrder(int orderId, Order order)
    {
        order.UpdatedAt = GetTimestamp();
        var index = data.FindIndex(x => x.Id == orderId);
        if (index != -1)
        {
            data[index] = order;
        }
    }

    public void UpdateItemsInOrder(int orderId, List<Item> items)
    {
        var order = GetOrder(orderId);
        var current = order.Items;

        foreach (var x in current)
        {
            bool found = items.Exists(y => y.ItemId == x.ItemId);
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
                if (x.ItemId == y.ItemId)
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
        UpdateOrder(orderId, order);
    }

    public void UpdateOrdersInShipment(int shipmentId, List<int> orders)
    {
        var packedOrders = GetOrdersInShipment(shipmentId);
        foreach (var x in packedOrders)
        {
            if (!orders.Contains(x))
            {
                var order = GetOrder(x);
                order.ShipmentId = -1;
                order.OrderStatus = "Scheduled";
                UpdateOrder(x, order);
            }
        }

        foreach (var x in orders)
        {
            var order = GetOrder(x);
            order.ShipmentId = shipmentId;
            order.OrderStatus = "Packed";
            UpdateOrder(x, order);
        }
    }

    public void RemoveOrder(int orderId)
    {
        var order = GetOrder(orderId);
        if (order != null)
        {
            data.Remove(order);
        }
    }

    private void Load(bool isDebug)
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

    public void Save()
    {
        using (var writer = new StreamWriter(dataPath))
        {
            var json = JsonConvert.SerializeObject(data, Formatting.Indented);
            writer.Write(json);
        }
    }
}

public class Order
{
    public int Id { get; set; }
    public int ShipmentId { get; set; }
    public int ShipTo { get; set; }
    public int BillTo { get; set; }
    public string OrderStatus { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<Item> Items { get; set; }
}

public class Item
{
    public int ItemId { get; set; }
    public int Amount { get; set; }
    public int TotalOnHand { get; set; }
    public int TotalOrdered { get; set; }
}

public class BaseCS
{
    protected DateTime GetTimestamp()
    {
        return DateTime.UtcNow;
    }
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
    public List<Inventory> GetInventoriesForItem(int itemId)
    {
        return new List<Inventory>(); // Replace with actual implementation
    }

    public void UpdateInventory(int id, Inventory inventory)
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
