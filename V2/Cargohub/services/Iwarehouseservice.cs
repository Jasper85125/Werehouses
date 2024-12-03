namespace ServicesV2;

public interface IWarehouseService
{
    List<WarehouseCS> GetAllWarehouses();
    WarehouseCS GetWarehouseById(int id);
    WarehouseCS CreateWarehouse(WarehouseCS newWarehouse);
    List<WarehouseCS> CreateMultipleWarehouse(List<WarehouseCS>newWarehouse);
    WarehouseCS UpdateWarehouse(int id, WarehouseCS warehouse);
    WarehouseCS PatchWarehouse(int id, string property, object value);
    void DeleteWarehouse(int id);
    void DeleteWarehouses(List<int> ids);
}
