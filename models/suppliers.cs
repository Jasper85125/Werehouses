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
    public string? AddressExtra { get; set; }
    public string? City { get; set; }
    public string? ZipCode { get; set; }
    public string? Province { get; set; }
    public string? Country { get; set; }
    public string? ContactName { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Reference { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class SuppliersCS
{
    private string dataPath;
    private List<SupplierCS> data;
    private static List<SupplierCS> SUPPLIERS = new List<SupplierCS>();

    public SuppliersCS(string rootPath, bool isDebug = false)
    {
        this.dataPath = Path.Combine(rootPath, "suppliers.json");
        this.LoadCS(isDebug);
    }

    public List<SupplierCS> GetSuppliers()
    {
        return this.data;
    }

    public SupplierCS GetSupplierCS(int supplierId)
    {
        return this.data.Find(supplier => supplier.Id == supplierId);
    }

    public void AddSupplierCS(SupplierCS supplier)
    {
        supplier.CreatedAt = DateTime.Now;
        supplier.UpdatedAt = DateTime.Now;
        this.data.Add(supplier);
    }

    public void UpdateSupplierCS(int supplierId, SupplierCS supplier)
    {
        supplier.UpdatedAt = DateTime.Now;
        int index = this.data.FindIndex(s => s.Id == supplierId);
        if (index != -1)
        {
            this.data[index] = supplier;
        }
    }

    public void RemoveSupplierCS(int supplierId)
    {
        this.data.RemoveAll(supplier => supplier.Id == supplierId);
    }

    private void LoadCS(bool isDebug)
    {
        if (isDebug)
        {
            this.data = SUPPLIERS;
        }
        else
        {
            if (File.Exists(this.dataPath))
            {
                string json = File.ReadAllText(this.dataPath);
                this.data = JsonConvert.DeserializeObject<List<SupplierCS>>(json);
            }
            else
            {
                this.data = new List<SupplierCS>();
            }
        }
    }

    public void SaveCS()
    {
        string json = JsonConvert.SerializeObject(this.data, Formatting.Indented);
        File.WriteAllText(this.dataPath, json);
    }
}
