namespace Services;

public interface IShipmentService
{
    List<ShipmentCS> GetAllShipments();
    ShipmentCS GetShipmentById(int id);
    ShipmentCS CreateShipment(ShipmentCS newShipment);
    Task<ShipmentCS> UpdateShipment(int id, ShipmentCS updateShipment);
    void DeleteShipment(int id);
}