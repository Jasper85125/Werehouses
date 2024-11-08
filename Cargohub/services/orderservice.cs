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

    public Task<OrderCS> UpdateOrder(int id, OrderCS updateOrder)
    {
        List<OrderCS> orders = GetAllOrders();
        var existingOrder = orders.FirstOrDefault(o => o.Id == id);
        if (existingOrder == null)
        {
            return null;
        }

        // Get the current date and time
        var currentDateTime = DateTime.Now;

        // Format the date and time to the desired format
        var formattedDateTime = currentDateTime.ToString("yyyy-MM-dd HH:mm:ss");

        // Update the existing order with new values
        existingOrder.source_id = updateOrder.source_id;
        existingOrder.order_date = updateOrder.order_date;
        existingOrder.request_date = updateOrder.request_date;
        existingOrder.Reference = updateOrder.Reference;
        existingOrder.reference_extra = updateOrder.reference_extra;
        existingOrder.order_status = updateOrder.order_status;
        existingOrder.Notes = updateOrder.Notes;
        existingOrder.shipping_notes = updateOrder.shipping_notes;
        existingOrder.picking_notes = updateOrder.picking_notes;
        existingOrder.warehouse_id = updateOrder.warehouse_id;
        existingOrder.ship_to = updateOrder.ship_to;
        existingOrder.bill_to = updateOrder.bill_to;
        existingOrder.shipment_id = updateOrder.shipment_id;
        existingOrder.total_amount = updateOrder.total_amount;
        existingOrder.total_discount = updateOrder.total_discount;
        existingOrder.total_tax = updateOrder.total_tax;
        existingOrder.total_surcharge = updateOrder.total_surcharge;
        existingOrder.items = updateOrder.items;
        existingOrder.updated_at = DateTime.ParseExact(formattedDateTime, "yyyy-MM-dd HH:mm:ss", null);

        var jsonData = JsonConvert.SerializeObject(orders, Formatting.Indented);
        File.WriteAllTextAsync("data/orders.json", jsonData);

        return Task.FromResult(existingOrder);
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

    

    public Task<OrderCS> UpdateOrderItems(int orderId, List<ItemIdAndAmount> updatedItems)
    {
        List<OrderCS> orders = GetAllOrders();
        var existingOrder = orders.FirstOrDefault(o => o.Id == orderId);
        if (existingOrder == null)
        {
            return null;
        }

        var jsonData = JsonConvert.SerializeObject(orders, Formatting.Indented);
        File.WriteAllTextAsync("data/orders.json", jsonData);

        return Task.FromResult(existingOrder);
    }
}
