using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

public class LocationCS
{
    public int Id { get; set; }
    public int WarehouseId { get; set; }
    public string? Code { get; set; }
    public string? Name { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
