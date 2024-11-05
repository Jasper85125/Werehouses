namespace Services;

public interface IInventoryService
{
    List<InventoryCS> GetAllInventories();
    InventoryCS GetInventoryById(int id);
    InventoryCS UpdateInventoryById(int id, InventoryCS updatedinventory);
    // void CreateWarehouse();
    InventoryCS CreateInventory(InventoryCS inventory);    
    // void UpdateWarehouse();
    void DeleteInventory(int id);
}