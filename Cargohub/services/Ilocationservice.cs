namespace Services;

public interface ILocationService
{
    List<LocationCS> GetAllLocations();
    LocationCS GetLocationById(int id);
    LocationCS CreateLocation(LocationCS location);
    LocationCS UpdateLocation(LocationCS location, int id);
    // void DeleteWarehouse();
}