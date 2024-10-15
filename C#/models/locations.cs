using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

public class LocationCS
{
    public int Id { get; set; }
    public int WarehouseId { get; set; }
    public string? Code { get; set; }
    public string? Name { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class LocationsCS : BaseCS
{
    private string dataPath;
    private List<LocationCS> data;

    public LocationsCS(string rootPath, bool isDebug = false)
    {
        dataPath = Path.Combine(rootPath, "locations.json");
        LoadCS(isDebug);
    }

    public List<LocationCS> GetLocationsCS()
    {
        return data;
    }

    public LocationCS GetLocationCS(int locationId)
    {
        return data.Find(x => x.Id == locationId);
    }

    public List<LocationCS> GetLocationsInWarehouseCS(int warehouseId)
    {
        return data.FindAll(x => x.WarehouseId == warehouseId);
    }

    public void AddLocationCS(LocationCS location)
    {
        location.CreatedAt = GetTimestampCS();
        location.UpdatedAt = GetTimestampCS();
        data.Add(location);
    }

    public void UpdateLocationCS(int locationId, LocationCS location)
    {
        location.UpdatedAt = GetTimestampCS();
        int index = data.FindIndex(x => x.Id == locationId);
        if (index != -1)
        {
            data[index] = location;
        }
    }

    public void RemoveLocationCS(int locationId)
    {
        data.RemoveAll(x => x.Id == locationId);
    }

    private void LoadCS(bool isDebug)
    {
        if (isDebug)
        {
            data = new List<LocationCS>(); // Initialize with empty list or mock data
        }
        else
        {
            using (StreamReader r = new StreamReader(dataPath))
            {
                string json = r.ReadToEnd();
                data = JsonConvert.DeserializeObject<List<LocationCS>>(json);
            }
        }
    }

    public void SaveCS()
    {
        using (StreamWriter w = new StreamWriter(dataPath))
        {
            string json = JsonConvert.SerializeObject(data, Formatting.Indented);
            w.Write(json);
        }
    }
}
