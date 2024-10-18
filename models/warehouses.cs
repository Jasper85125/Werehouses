using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

public class WarehouseCS
{
    public int Id { get; set; }
    public string? Code { get; set; }
    public string? Name { get; set; }
    public string? Address { get; set; }
    public string? Zip { get; set; }
    public string? Province { get; set; }
    public string? Country { get; set; }
    public Dictionary<string, string> Contact { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
