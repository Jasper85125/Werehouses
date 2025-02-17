namespace ServicesV2;

public interface ILocationService
{
    List<LocationCS> GetAllLocations();
    LocationCS GetLocationById(int id);
    List<LocationCS> GetLocationsByWarehouseId(int warehouse_id);
    LocationCS CreateLocation(LocationCS location);
    List<LocationCS> CreateMultipleLocations(List<LocationCS> locations);
    LocationCS UpdateLocation(LocationCS location, int id);
    LocationCS PatchLocation(int id, string property, object newvalue);
    void DeleteLocation(int id);
    void DeleteLocations(List<int> ids);
}
