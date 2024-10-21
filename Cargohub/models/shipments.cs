using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

public class ShipmentCS
{
    public int Id { get; set; }
    public int order_id { get; set; }
    public int source_id { get; set; }
    public DateTime order_date { get; set; }
    public DateTime request_date { get; set; }
    public DateTime shipment_date { get; set; }
    public string? shipment_type { get; set; }
    public string? shipment_status { get; set; }
    public string? Notes { get; set; }
    public string? carrier_code { get; set; }
    public string? carrier_description { get; set; }
    public string? service_code { get; set; }
    public string? payment_type { get; set; }
    public string? transfer_mode { get; set; }
    public int total_package_count { get; set; }
    public double total_package_weight { get; set; }
    public List<ItemIdAndAmount> Items { get; set; }
    public DateTime created_at { get; set; }
    public DateTime updated_at { get; set; }
}