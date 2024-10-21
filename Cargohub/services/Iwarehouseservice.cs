namespace Services;

public interface IWarehouseService
{
    List<WarehouseCS> GetAllWarehouses();
    WarehouseCS GetWarehouseById(int id);
    void CreateWarehouse(WarehouseCS newWarehouse);
    // void UpdateWarehouse();
    // void DeleteWarehouse();
    // void AttendWarehouse();
    // void ModifyWarehouse();
}