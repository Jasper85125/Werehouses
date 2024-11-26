namespace ServicesV1;

public interface IWarehouseService
{
    List<WarehouseCS> GetAllWarehouses();
    WarehouseCS GetWarehouseById(int id);
    WarehouseCS CreateWarehouse(WarehouseCS newWarehouse);
    List<WarehouseCS> CreateMultipleWarehouse(List<WarehouseCS>newWarehouse);
    WarehouseCS UpdateWarehouse(int id, WarehouseCS warehouse);
    void DeleteWarehouse(int id);
}