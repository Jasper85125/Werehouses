using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

public class ItemCS
{
    public required string Uid { get; set; }
    public string? Code { get; set; }
    public string? Description { get; set; }
    public string? ShortDescription { get; set; }
    public string? UpcCode { get; set; }
    public string? ModelNumber { get; set; }
    public string? CommodityCode { get; set; }
    public int? ItemLine { get; set; }
    public int? ItemGroup { get; set; }
    public int? ItemType { get; set; }
    public int UnitPurchaseQuantity {get; set;}
    public int UnitOrderQuantity {get; set;}
    public int PackOrderQuantity {get; set;}
    public int SupplierId { get; set; }
    public string? SupplierCode { get; set; }
    public string? SupplierPartNumber { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public int Amount { get; set; }
    public int TotalOnHand { get; set; }
    public int TotalOrdered { get; set; }
}

public class ItemsCS : BaseCS
{
    private string dataPath;
    private List<ItemCS> data;
    private static List<ItemCS> ITEMS = new List<ItemCS>();

    public ItemsCS(string rootPath, bool isDebug = false)
    {
        dataPath = Path.Combine(rootPath, "items.json");
        LoadCS(isDebug);
    }

    public List<ItemCS> GetItemsCS()
    {
        return data;
    }

    public ItemCS GetItemCS(string itemId)
    {
        return data.Find(x => x.Uid == itemId);
    }

    public List<ItemCS> GetItemsForItemLineCS(string itemLineId)
    {
        return data.FindAll(x => x.ItemLine == int.Parse(itemLineId));
    }

    public List<ItemCS> GetItemsForItemGroupCS(string itemGroupId)
    {
        return data.FindAll(x => x.ItemGroup == int.Parse(itemGroupId));
    }

    public List<ItemCS> GetItemsForItemTypeCS(string itemTypeId)
    {
        return data.FindAll(x => x.ItemType == int.Parse(itemTypeId));
    }

    public List<ItemCS> GetItemsForSupplierCS(int supplierId)
    {
        return data.FindAll(x => x.SupplierId == supplierId);
    }

    public void AddItemCS(ItemCS item)
    {
        item.CreatedAt = GetTimestampCS();
        item.UpdatedAt = GetTimestampCS();
        data.Add(item);
    }

    public void UpdateItemCS(string itemId, ItemCS item)
    {
        item.UpdatedAt = GetTimestampCS();
        int index = data.FindIndex(x => x.Uid == itemId);
        if (index != -1)
        {
            data[index] = item;
        }
    }

    public void RemoveItemCS(string itemId)
    {
        data.RemoveAll(x => x.Uid == itemId);
    }

    private void LoadCS(bool isDebug)
    {
        if (isDebug)
        {
            data = ITEMS;
        }
        else
        {
            using (StreamReader r = new StreamReader(dataPath))
            {
                string json = r.ReadToEnd();
                data = JsonConvert.DeserializeObject<List<ItemCS>>(json);
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
