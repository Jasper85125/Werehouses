namespace Services;

public interface ISupplierService
{
    List<SupplierCS> GetAllSuppliers();
    SupplierCS GetSupplierById(int id);
    SupplierCS CreateSupplier(SupplierCS supplier);
    SupplierCS UpdateSupplier(int id, SupplierCS supplier);
    void DeleteSupplier(int id);
    void DeleteSuppliers(List<int> ids);
    List<ItemCS> GetItemsBySupplierId(int supplierId);
    SupplierCS PatchSupplier(int id, SupplierCS updateSupplier);
}