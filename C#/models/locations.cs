using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

public class Location
{
    public int Id { get; set; }
    public int WarehouseId { get; set; }
    public string CreatedAt { get; set; }
    public string UpdatedAt { get; set; }
    // Add other properties as needed
}

public class Locations : BaseCS
{
    private string dataPath;
    private List<Location> data;

    public Locations(string rootPath, bool isDebug = false)
    {
        dataPath = Path.Combine(rootPath, "locations.json");
        Load(isDebug);
    }

    public List<Location> GetLocations()
    {
        return data;
    }

    public Location GetLocation(int locationId)
    {
        return data.Find(x => x.Id == locationId);
    }

    public List<Location> GetLocationsInWarehouse(int warehouseId)
    {
        return data.FindAll(x => x.WarehouseId == warehouseId);
    }

    public void AddLocation(Location location)
    {
        location.CreatedAt = GetTimestamp();
        location.UpdatedAt = GetTimestamp();
        data.Add(location);
    }

    public void UpdateLocation(int locationId, Location location)
    {
        location.UpdatedAt = GetTimestamp();
        int index = data.FindIndex(x => x.Id == locationId);
        if (index != -1)
        {
            data[index] = location;
        }
    }

    public void RemoveLocation(int locationId)
    {
        data.RemoveAll(x => x.Id == locationId);
    }

    private void Load(bool isDebug)
    {
        if (isDebug)
        {
            data = new List<Location>(); // Initialize with empty list or mock data
        }
        else
        {
            using (StreamReader r = new StreamReader(dataPath))
            {
                string json = r.ReadToEnd();
                data = JsonConvert.DeserializeObject<List<Location>>(json);
            }
        }
    }

    public void Save()
    {
        using (StreamWriter w = new StreamWriter(dataPath))
        {
            string json = JsonConvert.SerializeObject(data, Formatting.Indented);
            w.Write(json);
        }
    }

    private string GetTimestamp()
    {
        return DateTime.UtcNow.ToString("o"); // ISO 8601 format
    }
}
