using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

public class ClientCS
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? zip_code { get; set; }
    public string? Province { get; set; }
    public string? Country { get; set; }
    public string? contact_name { get; set; }
    public string? contact_phone { get; set; }
    public string? contact_email { get; set; }
    public DateTime created_at { get; set; }
    public DateTime updated_at { get; set; }
}
