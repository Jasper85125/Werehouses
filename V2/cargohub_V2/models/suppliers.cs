using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

public class SupplierCS
{
    public int Id { get; set; }
    public string? Code { get; set; }
    public string? Name { get; set; }
    public string? Address { get; set; }
    public string? address_extra { get; set; }
    public string? City { get; set; }
    public string? zip_code { get; set; }
    public string? Province { get; set; }
    public string? Country { get; set; }
    public string? contact_name { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Reference { get; set; }
    public DateTime created_at { get; set; }
    public DateTime updated_at { get; set; }
}
