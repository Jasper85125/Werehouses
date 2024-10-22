using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Services;

public class TransferService : ITransferService
{
    // Constructor
    public TransferService()
    {
        // Initialization code here
    }

    public List<TransferCS> GetAllTransfers()
    {
        var Path = "data/transfers.json";
        if (!File.Exists(Path))
        {
            return new List<TransferCS>();
        }
        var jsonData = File.ReadAllText(Path);
        List<TransferCS> transfers = JsonConvert.DeserializeObject<List<TransferCS>>(jsonData);
        return transfers ?? new List<TransferCS>();
    }

    public TransferCS GetTransferById(int id)
    {
        List<TransferCS> transfers = GetAllTransfers();
        TransferCS transfer = transfers.FirstOrDefault(trans => trans.Id == id);
        return transfer;
    }
    public TransferCS CreateTransfer(TransferCS newTransfer)
    {
        var Path = "data/transfers.json";

        List<TransferCS> transfers = GetAllTransfers();

        // Add the new transfer record to the list
        newTransfer.Id = transfers.Count > 0 ? transfers.Max(t => t.Id) + 1 : 1;
        transfers.Add(newTransfer);

        // Serialize the updated list back to the JSON file
        var jsonData = JsonConvert.SerializeObject(transfers, Formatting.Indented);
        File.WriteAllText(Path, jsonData);
        return newTransfer;
    }
}
