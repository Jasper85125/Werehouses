namespace Services;

public interface IShipmentService
{
    List<ShipmentCS> GetAllShipments();
    ShipmentCS GetShipmentById(int id);
    List<ItemIdAndAmount> GetItemsInShipment(int shipmentId);
    ShipmentCS CreateShipment(ShipmentCS newShipment);
    List<ShipmentCS> CreateMultipleShipments(List<ShipmentCS> shipments);
    Task<ShipmentCS> UpdateShipment(int id, ShipmentCS updateShipment);
    ShipmentCS UpdateItemsInShipment(int ShipmentId, List<ItemIdAndAmount> Items);
    void DeleteShipment(int id);
    void DeleteItemFromShipment(int shipmentId, string itemId);
    void DeleteShipments(List<int> ids);
}