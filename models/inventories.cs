using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

public class InventoryCS
{
    public int Id { get; set; }
    public int ItemId { get; set; }
    public string? Description { get; set; }
    public string? ItemReference { get; set; }
    public List<int> Locations {get; set;}
    public int TotalOnHand { get; set; }
    public int TotalExpected { get; set; }
    public int TotalOrdered { get; set; }
    public int TotalAllocated { get; set; }
    public int TotalAvailable { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
