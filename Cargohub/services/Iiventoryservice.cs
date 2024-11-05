namespace Services;

public interface IInventoryService
{
    List<InventoryCS> GetAllInventories();
    InventoryCS GetInventoryById(int id);
    InventoryCS CreateInventory(InventoryCS inventory);    
    // void UpdateWarehouse();
    // void DeleteWarehouse();
}