using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Services;

public class LocationService : ILocationService
{
    // Constructor
    public LocationService()
    {
        // Initialization code here
    }

    public List<LocationCS> GetAllLocations()
    {
        var Path = "data/locations.json";
        if (!File.Exists(Path))
        {
            return new List<LocationCS>();
        }
        var jsonData = File.ReadAllText(Path);
        List<LocationCS> locations = JsonConvert.DeserializeObject<List<LocationCS>>(jsonData);
        return locations ?? new List<LocationCS>();
    }

    public LocationCS GetLocationById(int id)
    {
        List<LocationCS> locations = GetAllLocations();
        LocationCS location = locations.FirstOrDefault(loc => loc.Id == id);
        return location;
    }

    public LocationCS CreateLocation(LocationCS newLocation)
    {
        var Path = "data/locations.json";

        List<LocationCS> locations = GetAllLocations();

        newLocation.Id = locations.Count > 0 ? locations.Max(o => o.Id) + 1 : 1;
        locations.Add(newLocation);

        var jsonData = JsonConvert.SerializeObject(locations, Formatting.Indented);
        File.WriteAllText(Path, jsonData);
        return newLocation;
    }
}
