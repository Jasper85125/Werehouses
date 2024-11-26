namespace ServicesV1;

public interface IInventoryService
{
    List<InventoryCS> GetAllInventories();
    InventoryCS GetInventoryById(int id);
    InventoryCS GetInventoriesForItem(string id);
    InventoryCS CreateInventory(InventoryCS inventory);
    InventoryCS UpdateInventoryById(int id, InventoryCS updatedinventory);
    void DeleteInventory(int id);
}