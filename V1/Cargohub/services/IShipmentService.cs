namespace ServicesV1;

public interface IShipmentService
{
    List<ShipmentCS> GetAllShipments();
    ShipmentCS GetShipmentById(int id);
    List<ItemIdAndAmount> GetItemsInShipment(int shipmentId);
    ShipmentCS CreateShipment(ShipmentCS newShipment);
    Task<ShipmentCS> UpdateShipment(int id, ShipmentCS updateShipment);
    ShipmentCS UpdateItemsInShipment(int ShipmentId, List<ItemIdAndAmount> Items);
    void DeleteShipment(int id);
    void DeleteItemFromShipment(int shipmentId, string itemId);
}
