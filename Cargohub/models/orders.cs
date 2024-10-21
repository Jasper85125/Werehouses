using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

public class OrderCS
{
    public int Id { get; set; }
    public int source_id { get; set; }
    public string? order_date { get; set; }
    public string? request_date { get; set; }
    public string? Reference { get; set; }
    public string? reference_extra { get; set;}
    public string? order_status { get; set; }
    public string? Notes { get; set;}
    public string? shipping_notes { get; set; }
    public string? picking_notes { get; set; }
    public string? warehouse_id { get; set; }
    public int? ship_to { get; set; }
    public int? bill_to { get; set; }
    public int? shipment_id { get; set; }
    public double total_amount { get; set; }
    public double total_discount { get; set; }
    public double total_tax { get; set; }
    public double total_surcharge { get; set; }
    public DateTime created_at { get; set; }
    public DateTime updated_at { get; set; }
    public List<ItemIdAndAmount> items { get; set; }
}
