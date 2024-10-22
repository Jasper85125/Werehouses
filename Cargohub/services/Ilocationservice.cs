namespace Services;

public interface ILocationService
{
    List<LocationCS> GetAllLocations();
    LocationCS GetLocationById(int id);
    LocationCS CreateLocation(LocationCS location);
    // void UpdateWarehouse();
    // void DeleteWarehouse();
}