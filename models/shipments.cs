using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

public class ShipmentCS
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public int SourceId { get; set; }
    public DateTime OrderDate { get; set; }
    public DateTime RequestDate { get; set; }
    public DateTime ShipmentDate { get; set; }
    public string? ShipmentType { get; set; }
    public string? ShipmentStatus { get; set; }
    public string? Notes { get; set; }
    public string? CarrierCode { get; set; }
    public string? CarrierDescription { get; set; }
    public string? ServiceCode { get; set; }
    public string? PaymentType { get; set; }
    public string? TransferMode { get; set; }
    public int TotalPackageCount { get; set; }
    public double TotalPackageWeight { get; set; }
    public List<ItemCS> Items { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}