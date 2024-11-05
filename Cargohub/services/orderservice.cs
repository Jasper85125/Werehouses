using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Services;

public class OrderService : IOrderService
{
    // Constructor
    public OrderService()
    {
        // Initialization code here
    }

    public List<OrderCS> GetAllOrders()
    {
        var Path = "data/orders.json";
        if (!File.Exists(Path))
        {
            return new List<OrderCS>();
        }
        var jsonData = File.ReadAllText(Path);
        List<OrderCS> orders = JsonConvert.DeserializeObject<List<OrderCS>>(jsonData);
        return orders ?? new List<OrderCS>();
    }

    public OrderCS GetOrderById(int id)
    {
        List<OrderCS> orders = GetAllOrders();
        OrderCS order = orders.FirstOrDefault(order => order.Id == id);
        return order;
    }
    public OrderCS CreateOrder(OrderCS newOrder)
    {
        var Path = "data/orders.json";

        List<OrderCS> orders = GetAllOrders();

        // Add the new order record to the list
        newOrder.Id = orders.Count > 0 ? orders.Max(o => o.Id) + 1 : 1;
        orders.Add(newOrder);

        // Serialize the updated list back to the JSON file
        var jsonData = JsonConvert.SerializeObject(orders, Formatting.Indented);
        File.WriteAllText(Path, jsonData);
        return newOrder;
    }

    public void DeleteOrder(int id)
    {
        var Path = "data/orders.json";

        List<OrderCS> orders = GetAllOrders();

        // Remove the order record from the list
        OrderCS order = orders.FirstOrDefault(order => order.Id == id);
        if (order != null)
        {
            orders.Remove(order);

            // Serialize the updated list back to the JSON file
            var jsonData = JsonConvert.SerializeObject(orders, Formatting.Indented);
            File.WriteAllText(Path, jsonData);
        }
    }
}
