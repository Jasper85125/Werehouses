using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

public class Supplier
{
    public int Id { get; set; }
    public string Name { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class Suppliers
{
    private string dataPath;
    private List<Supplier> data;
    private static List<Supplier> SUPPLIERS = new List<Supplier>();

    public Suppliers(string rootPath, bool isDebug = false)
    {
        this.dataPath = Path.Combine(rootPath, "suppliers.json");
        this.Load(isDebug);
    }

    public List<Supplier> GetSuppliers()
    {
        return this.data;
    }

    public Supplier GetSupplier(int supplierId)
    {
        return this.data.Find(supplier => supplier.Id == supplierId);
    }

    public void AddSupplier(Supplier supplier)
    {
        supplier.CreatedAt = DateTime.Now;
        supplier.UpdatedAt = DateTime.Now;
        this.data.Add(supplier);
    }

    public void UpdateSupplier(int supplierId, Supplier supplier)
    {
        supplier.UpdatedAt = DateTime.Now;
        int index = this.data.FindIndex(s => s.Id == supplierId);
        if (index != -1)
        {
            this.data[index] = supplier;
        }
    }

    public void RemoveSupplier(int supplierId)
    {
        this.data.RemoveAll(supplier => supplier.Id == supplierId);
    }

    private void Load(bool isDebug)
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
                this.data = JsonConvert.DeserializeObject<List<Supplier>>(json);
            }
            else
            {
                this.data = new List<Supplier>();
            }
        }
    }

    public void Save()
    {
        string json = JsonConvert.SerializeObject(this.data, Formatting.Indented);
        File.WriteAllText(this.dataPath, json);
    }
}
