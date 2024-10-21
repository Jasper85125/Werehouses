namespace Services;

public interface IWarehouseService
{
    List<WarehouseCS> GetAllWarehouses();
    WarehouseCS GetWarehouseById(int id);
    // void CreateWarehouse();
    // void UpdateWarehouse();
    // void DeleteWarehouse();
    // void AttendWarehouse();
    // void ModifyWarehouse();
}