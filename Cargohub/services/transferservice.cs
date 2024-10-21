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
}
