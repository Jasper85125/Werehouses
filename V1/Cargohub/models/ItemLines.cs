using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

public class ItemLineCS
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set;}
    public DateTime created_at { get; set; }
    public DateTime updated_at { get; set; }
}
