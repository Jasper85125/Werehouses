using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

public class TransferCS
{
    public int Id { get; set;}
    public string? Reference { get; set; }
    public int TransferFrom { get; set; }
    public int TransferTo { get; set; }
    public string? TransferStatus { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<ItemCS> Items { get; set; }
}
