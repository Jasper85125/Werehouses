using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ServicesV1;

public class OrderService : IOrderService
{
    private string path = "../../data/orders.json";
    public OrderService()
    {
    }

    public List<OrderCS> GetAllOrders()
    {
        if (!File.Exists(path))
        {
            return new List<OrderCS>();
        }
        var jsonData = File.ReadAllText(path);
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
        List<OrderCS> orders = GetAllOrders();
        var currentDateTime = DateTime.Now;
        var formattedDateTime = currentDateTime.ToString("yyyy-MM-dd HH:mm:ss");

        // Add the new order record to the list
        newOrder.Id = orders.Count > 0 ? orders.Max(o => o.Id) + 1 : 1;
        newOrder.created_at = DateTime.ParseExact(formattedDateTime, "yyyy-MM-dd HH:mm:ss", null);
        newOrder.updated_at = DateTime.ParseExact(formattedDateTime, "yyyy-MM-dd HH:mm:ss", null);
        orders.Add(newOrder);

        // Serialize the updated list back to the JSON file
        var jsonData = JsonConvert.SerializeObject(orders, Formatting.Indented);
        File.WriteAllText(path, jsonData);
        return newOrder;
    }

    public List<OrderCS> GetOrdersByClient(int client_id)
    {
        List<OrderCS> orders = GetAllOrders();
        List<OrderCS> clientOrders = orders.Where(order => order.ship_to == client_id || order.bill_to == client_id).ToList();
        if (clientOrders.Count == 0)
        {
            return null;
        }
        return clientOrders;
    }

    public List<OrderCS> GetOrdersByShipmentId(int shipmentId)
    {
        List<OrderCS> orders = GetAllOrders();
        List<OrderCS> shipmentOrders = orders.Where(order => order.shipment_id == shipmentId).ToList();
        if (shipmentOrders == null)
        {
            return null;
        }
        return shipmentOrders;
    }

    public OrderCS UpdateOrder(int id, OrderCS updateOrder)
    {
        List<OrderCS> orders = GetAllOrders();
        var existingOrder = orders.FirstOrDefault(o => o.Id == id);
        if (existingOrder == null)
        {
            return null;
        }

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
        existingOrder.updated_at = DateTime.ParseExact(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "yyyy-MM-dd HH:mm:ss", null);

        var jsonData = JsonConvert.SerializeObject(orders, Formatting.Indented);
        File.WriteAllText(path, jsonData);

        return existingOrder;
    }

    public void DeleteOrder(int id)
    {

        List<OrderCS> orders = GetAllOrders();

        OrderCS order = orders.FirstOrDefault(order => order.Id == id);
        if (order != null)
        {
            orders.Remove(order);

            var jsonData = JsonConvert.SerializeObject(orders, Formatting.Indented);
            File.WriteAllText(path, jsonData);
        }
    }

    public List<ItemIdAndAmount> GetItemsByOrderId(int orderId)
    {
        var order = GetOrderById(orderId);
        if (order == null)
        {
            return null;
        }

        return order.items;
    }

    public OrderCS UpdateOrderItems(int orderId, List<ItemIdAndAmount> items)
    {
        List<OrderCS> orders = GetAllOrders();
        var existingOrder = orders.FirstOrDefault(o => o.Id == orderId);
        if (existingOrder == null)
        {
            return null;
        }

        existingOrder.items = items;
        existingOrder.updated_at = DateTime.ParseExact(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "yyyy-MM-dd HH:mm:ss", null);

        var jsonData = JsonConvert.SerializeObject(orders, Formatting.Indented);
        File.WriteAllText(path, jsonData);

        return existingOrder;
    }

}
