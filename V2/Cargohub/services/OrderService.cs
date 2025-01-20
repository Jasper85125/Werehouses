using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ServicesV2;

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
    public OrderCS CreateOrder(OrderCS newOrder)
    {
        List<OrderCS> orders = GetAllOrders();
        var currentDateTime = DateTime.Now;
        var formattedDateTime = currentDateTime.ToString("yyyy-MM-dd HH:mm:ss");

        newOrder.Id = orders.Count > 0 ? orders.Max(o => o.Id) + 1 : 1;
        newOrder.created_at = DateTime.ParseExact(formattedDateTime, "yyyy-MM-dd HH:mm:ss", null);
        newOrder.updated_at = DateTime.ParseExact(formattedDateTime, "yyyy-MM-dd HH:mm:ss", null);
        orders.Add(newOrder);

        var jsonData = JsonConvert.SerializeObject(orders, Formatting.Indented);
        File.WriteAllText(path, jsonData);
        return newOrder;
    }

    public List<OrderCS> CreateMultipleOrders(List<OrderCS> newOrders)
    {
        List<OrderCS> addedOrder = new List<OrderCS>();
        foreach (OrderCS order in newOrders)
        {
            OrderCS addOrder = CreateOrder(order);
            addedOrder.Add(addOrder);
        }
        return addedOrder;
    }

    public List<OrderCS> GetOrdersByClient(int client_id)
    {
        List<OrderCS> orders = GetAllOrders();
        List<OrderCS> clientOrders = orders.Where(order => order.ship_to == client_id || order.bill_to == client_id).ToList();
        if (clientOrders == null)
        {
            return null;
        }
        return clientOrders;
    }

    public List<OrderCS> GetOrdersByWarehouse(int warehouseId)
    {
        List<OrderCS> orders = GetAllOrders();
        List<OrderCS> warehouseOrders = orders.Where(order => order.warehouse_id == warehouseId).ToList();
        if (warehouseOrders == null)
        {
            return null;
        }
        return warehouseOrders;
    }

    public OrderCS UpdateOrder(int id, OrderCS updateOrder)
    {
        List<OrderCS> orders = GetAllOrders();
        var existingOrder = orders.FirstOrDefault(o => o.Id == id);
        if (existingOrder == null)
        {
            return null;
        }

        var currentDateTime = DateTime.Now;
        var formattedDateTime = currentDateTime.ToString("yyyy-MM-dd HH:mm:ss");

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
        File.WriteAllText(path, jsonData);

        return existingOrder;
    }

    public OrderCS PatchOrder(int id, string property, object newvalue)
    {
        var now = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        var orders = GetAllOrders();
        var order = orders.Find(_ => _.Id == id);
        switch (property)
        {
            case "source_id":
                order.source_id = (int)newvalue;
                break;
            case "order_date":
                order.order_date = newvalue.ToString();
                break;
            case "request_date":
                order.request_date = newvalue.ToString();
                break;
            case "Reference":
                order.Reference = newvalue.ToString();
                break;
            case "reference_extra":
                order.reference_extra = newvalue.ToString();
                break;
            case "order_status":
                order.order_status = newvalue.ToString();
                break;
            case "Notes":
                order.Notes = newvalue.ToString();
                break;
            case "shipping_notes":
                order.shipping_notes = newvalue.ToString();
                break;
            case "picking_notes":
                order.picking_notes = newvalue.ToString();
                break;
            case "warehouse_id":
                order.warehouse_id = (int)newvalue;
                break;
            case "ship_to":
                order.ship_to = (int)newvalue;
                break;
            case "bill_to":
                order.bill_to = (int)newvalue;
                break;
            case "shipment_id":
                order.shipment_id = (int)newvalue;
                break;
            case "total_amount":
                order.total_amount = (int)newvalue;
                break;
            case "total_discount":
                order.total_discount = (int)newvalue;
                break;
            case "total_tax":
                order.total_tax = (int)newvalue;
                break;
            case "total_surcharge":
                order.total_surcharge = (int)newvalue;
                break;
            case "items":
                order.items = newvalue as List<ItemIdAndAmount>;
                break;
        }
        order.updated_at = DateTime.ParseExact(now, "yyyy-MM-dd HH:mm:ss", null);
        var json = JsonConvert.SerializeObject(orders, Formatting.Indented);
        File.WriteAllText(path, json);
        return order;
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
        var currentDateTime = DateTime.Now;
        var formattedDateTime = currentDateTime.ToString("yyyy-MM-dd HH:mm:ss");
        existingOrder.updated_at = DateTime.ParseExact(formattedDateTime, "yyyy-MM-dd HH:mm:ss", null);
        var jsonData = JsonConvert.SerializeObject(orders, Formatting.Indented);
        File.WriteAllText(path, jsonData);
        return existingOrder;
    }

    public void DeleteOrders(List<int> ids)
    {
        var orders = GetAllOrders();
        foreach (int id in ids)
        {
            var order = orders.Find(_ => _.Id == id);
            if (order is not null)
            {
                orders.Remove(order);
            }
        }
        var json = JsonConvert.SerializeObject(orders, Formatting.Indented);
        File.WriteAllText(path, json);
    }
}
