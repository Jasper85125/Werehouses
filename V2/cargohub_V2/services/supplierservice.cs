using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Services;

public class SupplierService : ISupplierService
{
    private string _path = "data/suppliers.json";
    // Constructor
    public SupplierService()
    {
        // Initialization code here
    }

    public List<SupplierCS> GetAllSuppliers()
    {
        if (!File.Exists(_path))
        {
            return new List<SupplierCS>();
        }
        var jsonData = File.ReadAllText(_path);
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
        List<SupplierCS> suppliers = GetAllSuppliers();
        // Format the date and time to the desired format
        var currentDateTime = DateTime.Now;
        var formattedDateTime = currentDateTime.ToString("yyyy-MM-dd HH:mm:ss");

        newSupplier.Id = suppliers.Count > 0 ? suppliers.Max(o => o.Id) + 1 : 1;

        newSupplier.created_at = DateTime.ParseExact(formattedDateTime, "yyyy-MM-dd HH:mm:ss", null);
        newSupplier.updated_at = DateTime.ParseExact(formattedDateTime, "yyyy-MM-dd HH:mm:ss", null);
        suppliers.Add(newSupplier);

        var jsonData = JsonConvert.SerializeObject(suppliers, Formatting.Indented);
        File.WriteAllText(_path, jsonData);
        return newSupplier;
    }

    public List<SupplierCS> CreateMultipleSuppliers(List<SupplierCS>newSuppliers)
    {
        List<SupplierCS> addedSuppliers = new List<SupplierCS>();
        foreach(SupplierCS supplier in newSuppliers)
        {
            SupplierCS addSupplier = CreateSupplier(supplier);
            addedSuppliers.Add(addSupplier);
        }
        return addedSuppliers;
    }

    public SupplierCS UpdateSupplier(int id, SupplierCS updateSupplier)
    {
        var allSuppliers = GetAllSuppliers();
        var supplierToUpdate = allSuppliers.Single(supplier => supplier.Id == id);

        if (supplierToUpdate is not null)
        {
            // Get the current date and time
            var currentDateTime = DateTime.Now;

            // Format the date and time to the desired format
            var formattedDateTime = currentDateTime.ToString("yyyy-MM-dd HH:mm:ss");

            supplierToUpdate.Code = updateSupplier.Code;
            supplierToUpdate.Name = updateSupplier.Name;
            supplierToUpdate.Address = updateSupplier.Address;
            supplierToUpdate.address_extra = updateSupplier.address_extra;
            supplierToUpdate.City = updateSupplier.City;
            supplierToUpdate.zip_code = updateSupplier.zip_code;
            supplierToUpdate.Province = updateSupplier.Province;
            supplierToUpdate.Country = updateSupplier.Country;
            supplierToUpdate.contact_name = updateSupplier.contact_name;
            supplierToUpdate.PhoneNumber = updateSupplier.PhoneNumber;
            supplierToUpdate.Reference = updateSupplier.Reference;
            supplierToUpdate.updated_at = DateTime.ParseExact(formattedDateTime, "yyyy-MM-dd HH:mm:ss", null);

            var jsonData = JsonConvert.SerializeObject(allSuppliers, Formatting.Indented);
            File.WriteAllText(_path, jsonData);
            return supplierToUpdate;
        }
        return null;
    }

    public void DeleteSupplier(int id)
    {

        var path = "data/suppliers.json";
        List<SupplierCS> suppliers = GetAllSuppliers();
        SupplierCS supplier = suppliers.FirstOrDefault(supplier => supplier.Id == id);
        if (supplier != null)
        {
            suppliers.Remove(supplier);
            var jsonData = JsonConvert.SerializeObject(suppliers, Formatting.Indented);
            File.WriteAllText(path, jsonData);
        }

    }

    public List<ItemCS> GetItemsBySupplierId(int supplierId)
    {
        var itemsPath = "data/items.json";
        if (!File.Exists(itemsPath))
        {
            return new List<ItemCS>();
        }

        var jsonData = File.ReadAllText(itemsPath);
        List<ItemCS> items = JsonConvert.DeserializeObject<List<ItemCS>>(jsonData);

        return items?.Where(item => item.supplier_id == supplierId).ToList() ?? new List<ItemCS>();
    }

    public SupplierCS PatchSupplier(int id, SupplierCS updateSupplier)
    {
        var allSuppliers = GetAllSuppliers();
        var supplierToUpdate = allSuppliers.Single(supplier => supplier.Id == id);

        if (supplierToUpdate is not null)
        {
            // Get the current date and time
            var currentDateTime = DateTime.Now;

            // Format the date and time to the desired format
            var formattedDateTime = currentDateTime.ToString("yyyy-MM-dd HH:mm:ss");

            supplierToUpdate.Code = updateSupplier.Code ?? supplierToUpdate.Code;
            supplierToUpdate.Name = updateSupplier.Name ?? supplierToUpdate.Name;
            supplierToUpdate.Address = updateSupplier.Address ?? supplierToUpdate.Address;
            supplierToUpdate.address_extra = updateSupplier.address_extra ?? supplierToUpdate.address_extra;
            supplierToUpdate.City = updateSupplier.City ?? supplierToUpdate.City;
            supplierToUpdate.zip_code = updateSupplier.zip_code ?? supplierToUpdate.zip_code;
            supplierToUpdate.Province = updateSupplier.Province ?? supplierToUpdate.Province;
            supplierToUpdate.Country = updateSupplier.Country ?? supplierToUpdate.Country;
            supplierToUpdate.contact_name = updateSupplier.contact_name ?? supplierToUpdate.contact_name;
            supplierToUpdate.PhoneNumber = updateSupplier.PhoneNumber ?? supplierToUpdate.PhoneNumber;
            supplierToUpdate.Reference = updateSupplier.Reference ?? supplierToUpdate.Reference;
            supplierToUpdate.updated_at = DateTime.ParseExact(formattedDateTime, "yyyy-MM-dd HH:mm:ss", null);

            var jsonData = JsonConvert.SerializeObject(allSuppliers, Formatting.Indented);
            File.WriteAllText(_path, jsonData);
            return supplierToUpdate;
        }
        return null;
    }
    public void DeleteSuppliers(List<int> ids){
        var suppliers = GetAllSuppliers();
        foreach(int id in ids){
            var supplier = suppliers.Find(_=>_.Id == id);
            if(supplier is not null){
                suppliers.Remove(supplier);
            }
        }
        var json = JsonConvert.SerializeObject(suppliers, Formatting.Indented);
        File.WriteAllText(_path, json);
    }

}