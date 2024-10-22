using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Services;

public class SupplierService : ISupplierService
{
    // Constructor
    public SupplierService()
    {
        // Initialization code here
    }

    public List<SupplierCS> GetAllSuppliers()
    {
        var Path = "data/suppliers.json";
        if (!File.Exists(Path))
        {
            return new List<SupplierCS>();
        }
        var jsonData = File.ReadAllText(Path);
        List<SupplierCS> suppliers = JsonConvert.DeserializeObject<List<SupplierCS>>(jsonData);
        return suppliers ?? new List<SupplierCS>();
    }

    public SupplierCS GetSupplierById(int id)
    {
        List<SupplierCS> suppliers = GetAllSuppliers();
        SupplierCS supplier = suppliers.FirstOrDefault(supp => supp.Id == id);
        return supplier;
    }

    public SupplierCS CreateSupplier(SupplierCS newSupplier)
    {
        var Path = "data/suppliers.json";

        List<SupplierCS> suppliers = GetAllSuppliers();

        newSupplier.Id = suppliers.Count > 0 ? suppliers.Max(o => o.Id) + 1 : 1;
        suppliers.Add(newSupplier);

        var jsonData = JsonConvert.SerializeObject(suppliers, Formatting.Indented);
        File.WriteAllText(Path, jsonData);
        return newSupplier;
    }
}
