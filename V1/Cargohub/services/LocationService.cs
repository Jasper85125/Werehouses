using System;
using System.Collections.Generic;
using Microsoft.VisualBasic;
using Newtonsoft.Json;

namespace ServicesV1;

public class LocationService : ILocationService
{
    private string _path = "../../data/locations.json";
    public LocationService()
    {
    }

    public List<LocationCS> GetAllLocations()
    {
        if (!File.Exists(_path))
        {
            return new List<LocationCS>();
        }
        var jsonData = File.ReadAllText(_path);
        List<LocationCS> locations = JsonConvert.DeserializeObject<List<LocationCS>>(jsonData);
        return locations ?? new List<LocationCS>();
    }

    public LocationCS GetLocationById(int id)
    {
        List<LocationCS> locations = GetAllLocations();
        LocationCS location = locations.FirstOrDefault(loc => loc.Id == id);
        return location;
    }

    public List<LocationCS> GetLocationsByWarehouseId(int warehouse_id)
    {
        List<LocationCS> locations = GetAllLocations();
        List<LocationCS> locationsByWarehouseId = locations.FindAll(loc => loc.warehouse_id == warehouse_id);
        return locationsByWarehouseId;
    }

    public LocationCS CreateLocation(LocationCS newLocation)
    {
        List<LocationCS> locations = GetAllLocations();
        var currentDateTime = DateTime.Now;
        var formattedDateTime = currentDateTime.ToString("yyyy-MM-dd HH:mm:ss");

        newLocation.Id = locations.Count > 0 ? locations.Max(o => o.Id) + 1 : 1;
        newLocation.created_at = DateTime.ParseExact(formattedDateTime, "yyyy-MM-dd HH:mm:ss", null);
        newLocation.updated_at = DateTime.ParseExact(formattedDateTime, "yyyy-MM-dd HH:mm:ss", null);
        locations.Add(newLocation);

        var jsonData = JsonConvert.SerializeObject(locations, Formatting.Indented);
        File.WriteAllText(_path, jsonData);
        return newLocation;
    }

    public LocationCS UpdateLocation(LocationCS updatedLocation, int locationId)
    {
        var allLocations = GetAllLocations();
        var locationToUpdate = allLocations.SingleOrDefault(location => location.Id == locationId);

        if (locationToUpdate is not null)
        {
            var currentDateTime = DateTime.Now;

            var formattedDateTime = currentDateTime.ToString("yyyy-MM-dd HH:mm:ss");

            locationToUpdate.warehouse_id = updatedLocation.warehouse_id;
            locationToUpdate.code = updatedLocation.code;
            locationToUpdate.name = updatedLocation.name;
            locationToUpdate.updated_at = DateTime.ParseExact(formattedDateTime, "yyyy-MM-dd HH:mm:ss", null);

            var jsonData = JsonConvert.SerializeObject(allLocations, Formatting.Indented);
            File.WriteAllText(_path, jsonData);
            return locationToUpdate;
        }
        return null;


    }
    public void DeleteLocation(int locationId)
    {
        var locations = GetAllLocations();
        var location = locations.FirstOrDefault(l => l.Id == locationId);
        if (location != null)
        {
            locations.Remove(location);
            var jsonData = JsonConvert.SerializeObject(locations, Formatting.Indented);
            File.WriteAllText(_path, jsonData);
        }
        
    }
}
