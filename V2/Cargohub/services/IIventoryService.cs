namespace ServicesV2;

public interface IInventoryService
{
    List<InventoryCS> GetAllInventories();
    InventoryCS GetInventoryById(int id);
    InventoryCS GetInventoriesForItem(string id);
    InventoryCS CreateInventory(InventoryCS inventory);
    List<InventoryCS> CreateMultipleInventories(List<InventoryCS>newInventories);
    InventoryCS UpdateInventoryById(int id, InventoryCS updatedinventory);
    void DeleteInventory(int id);
    void DeleteInventories(List<int> ids);
    InventoryCS PatchInventory(int id, InventoryCS updatedInventory);
    List<InventoryCS> GetInventoriesByLocationId(List<int> location);
}
