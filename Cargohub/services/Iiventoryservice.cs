public interface IInventoryService
{
    List<InventoryCS> GetAllInventories();
    InventoryCS GetInventoryById(int id);
    // void CreateWarehouse();
    // void UpdateWarehouse();
    // void DeleteWarehouse();
}