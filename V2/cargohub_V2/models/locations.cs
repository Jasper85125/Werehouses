using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

public class LocationCS
{
    public int Id { get; set; }
    public int warehouse_id { get; set; }
    public string? code { get; set; }
    public string? name { get; set; }
    public DateTime created_at { get; set; }
    public DateTime updated_at { get; set; }
}
