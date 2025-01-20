namespace ServicesV2;

public interface ISupplierService
{
    List<SupplierCS> GetAllSuppliers();
    SupplierCS GetSupplierById(int id);
    SupplierCS CreateSupplier(SupplierCS supplier);
    List<SupplierCS> CreateMultipleSuppliers(List<SupplierCS> suppliers);
    SupplierCS UpdateSupplier(int id, SupplierCS supplier);
    SupplierCS PatchSupplier(int id, string property, object newvalue);
    void DeleteSupplier(int id);
    void DeleteSuppliers(List<int> ids);
    List<ItemCS> GetItemsBySupplierId(int supplierId);
}
