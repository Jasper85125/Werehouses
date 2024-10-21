using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

public class TransferCS
{
    public int Id { get; set;}
    public string? Reference { get; set; }
    public int? transfer_from { get; set; }
    public int? transfer_to { get; set; }
    public string? transfer_status { get; set; }
    public DateTime created_at { get; set; }
    public DateTime updated_at { get; set; }
    public List<ItemIdAndAmount> Items { get; set; }
}
