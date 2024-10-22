namespace Services;

public interface IShipmentService
{
    List<ShipmentCS> GetAllShipments();
    ShipmentCS GetShipmentById(int id);
    ShipmentCS CreateShipment(ShipmentCS newShipment);
    // void UpdateWarehouse();
    // void DeleteWarehouse();
}