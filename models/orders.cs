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
}
