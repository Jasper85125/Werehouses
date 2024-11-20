using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

public class InventoryCS
{
    public int Id { get; set; }
    public string? item_id { get; set; }
    public string description { get; set; }
    public string item_reference { get; set; }
    public List<int> Locations {get; set;}
    public int total_on_hand { get; set; }
    public int total_expected { get; set; }
    public int total_ordered { get; set; }
    public int total_allocated { get; set; }
    public int total_available { get; set; }
    public DateTime created_at { get; set; }
    public DateTime updated_at { get; set; }
}
