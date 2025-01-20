namespace ServicesV1;

public interface ILocationService
{
    List<LocationCS> GetAllLocations();
    LocationCS GetLocationById(int id);
    List<LocationCS> GetLocationsByWarehouseId(int warehouse_id);
    LocationCS CreateLocation(LocationCS location);
    LocationCS UpdateLocation(LocationCS location, int id);
    void DeleteLocation(int id);
}
