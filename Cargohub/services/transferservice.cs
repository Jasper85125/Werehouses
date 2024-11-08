using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Services;

public class TransferService : ITransferService
{
    private string _path = "data/transfers.json";

    public TransferService()
    {
        // Initialization code here
    }

    public List<TransferCS> GetAllTransfers()
    {
        if (!File.Exists(_path))
        {
            return new List<TransferCS>();
        }
        var jsonData = File.ReadAllText(_path);
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
        List<TransferCS> transfers = GetAllTransfers();

        // Add the new transfer record to the list
        newTransfer.Id = transfers.Count > 0 ? transfers.Max(t => t.Id) + 1 : 1;
        transfers.Add(newTransfer);

        // Serialize the updated list back to the JSON file
        var jsonData = JsonConvert.SerializeObject(transfers, Formatting.Indented);
        File.WriteAllText(_path, jsonData);
        return newTransfer;
    }

    public TransferCS UpdateTransfer(int id, TransferCS updateTransfer)
    {
        var allTransfers = GetAllTransfers();
        var transferToUpdate = allTransfers.Single(transfer => transfer.Id == id);

        if (transferToUpdate is not null)
        {
            // Get the current date and time
            var currentDateTime = DateTime.Now;

            // Format the date and time to the desired format
            var formattedDateTime = currentDateTime.ToString("yyyy-MM-dd HH:mm:ss");

            transferToUpdate.Reference = updateTransfer.Reference;
            transferToUpdate.transfer_from = updateTransfer.transfer_from;
            transferToUpdate.transfer_to = updateTransfer.transfer_to;
            transferToUpdate.transfer_status = updateTransfer.transfer_status;
            transferToUpdate.updated_at = DateTime.ParseExact(formattedDateTime, "yyyy-MM-dd HH:mm:ss", null);
            transferToUpdate.Items = updateTransfer.Items;


            var jsonData = JsonConvert.SerializeObject(allTransfers, Formatting.Indented);
            File.WriteAllText(_path, jsonData);
            return transferToUpdate;
        }
        return null;
    }

    public TransferCS CommitTransfer(int id, ItemIdAndAmount items)
    {
        TransferCS transfer = GetTransferById(id);
        
    }

    public void DeleteTransfer(int id)
    {
        /*
        var path = "data/suppliers.json";
        List<SupplierCS> suppliers = GetAllSuppliers();
        SupplierCS supplier = suppliers.FirstOrDefault(supplier => supplier.Id == id);
        if (supplier != null)
        {
            suppliers.Remove(supplier);
            var jsonData = JsonConvert.SerializeObject(suppliers, Formatting.Indented);
            File.WriteAllText(path, jsonData);
        }
        */
        var Path = "data/transfers.json";

        List<TransferCS> transfers = GetAllTransfers();
        TransferCS transfer = transfers.FirstOrDefault(transfer => transfer.Id == id);
        if (transfer != null)
        {
            transfers.Remove(transfer);
            var jsonData = JsonConvert.SerializeObject(transfers, Formatting.Indented);
            File.WriteAllText(Path, jsonData);
        }
    }
}
