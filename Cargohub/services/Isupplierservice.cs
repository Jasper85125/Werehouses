namespace Services;

public interface ISupplierService
{
    List<SupplierCS> GetAllSuppliers();
    SupplierCS GetSupplierById(int id);
    SupplierCS CreateSupplier(SupplierCS supplier);
    // void UpdateWarehouse();
    // void DeleteWarehouse();
}