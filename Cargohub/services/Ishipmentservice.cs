namespace Services;

public interface IShipmentService
{
    List<ShipmentCS> GetAllShipments();
    ShipmentCS GetShipmentById(int id);
    // void CreateWarehouse();
    // void UpdateWarehouse();
    // void DeleteWarehouse();
}