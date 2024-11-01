namespace Services;

public interface ISupplierService
{
    List<SupplierCS> GetAllSuppliers();
    SupplierCS GetSupplierById(int id);
    SupplierCS CreateSupplier(SupplierCS supplier);
    SupplierCS UpdateSupplier(int id, SupplierCS supplier);
    // void DeleteWarehouse();
}