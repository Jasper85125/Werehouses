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
}
