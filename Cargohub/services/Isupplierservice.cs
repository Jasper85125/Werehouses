namespace Services;

public interface ISupplierService
{
    List<SupplierCS> GetAllSuppliers();
    SupplierCS GetSupplierById(int id);
    // void CreateWarehouse();
    // void UpdateWarehouse();
    // void DeleteWarehouse();
}