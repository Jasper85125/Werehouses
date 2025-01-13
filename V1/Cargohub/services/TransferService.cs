using System;
using System.Collections.Generic;
using Microsoft.Testing.Platform.Extensions.Messages;
using Newtonsoft.Json;

namespace ServicesV1;

public class TransferService : ITransferService
{
    private string _path = "../../data/transfers.json";

    public TransferService()
    {

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
    public List<ItemIdAndAmount> GetItemsInTransfer(int transfer_id)
    {
        List<TransferCS> transfers = GetAllTransfers();
        TransferCS transfer = transfers.FirstOrDefault(t => t.Id == transfer_id);
        if (transfer != null)
        {
            return transfer.Items;
        }
        return null;
    }
    public TransferCS CreateTransfer(TransferCS newTransfer)
    {
        List<TransferCS> transfers = GetAllTransfers();

        newTransfer.Id = transfers.Count > 0 ? transfers.Max(t => t.Id) + 1 : 1;
        transfers.Add(newTransfer);

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
            var currentDateTime = DateTime.Now;

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

    public TransferCS CommitTransfer(int id)
    {
        InventoryService inventoryService = new InventoryService();
        TransferCS transfer = GetTransferById(id);
        if (transfer is null)
        {
            return null;
        }
        
        foreach (ItemIdAndAmount items in transfer.Items)
        {
            InventoryCS inventory = inventoryService.GetInventoriesForItem(items.item_id);
            foreach (int location in inventory.Locations)
            {
                if (location == transfer.transfer_from)
                {
                    inventory.total_on_hand -= items.amount;
                    inventory.total_expected = inventory.total_on_hand + inventory.total_expected;
                    inventory.total_available = inventory.total_on_hand - inventory.total_allocated;
                    inventoryService.UpdateInventoryById(inventory.Id, inventory); 
                }
                else if(location == transfer.transfer_to)
                {
                    inventory.total_on_hand += items.amount;
                    inventory.total_expected = inventory.total_on_hand + inventory.total_ordered;
                    inventory.total_available = inventory.total_on_hand - inventory.total_allocated;
                    inventoryService.UpdateInventoryById(inventory.Id, inventory); 
                }
            }
        } 
        transfer.transfer_status = "Processed";
        TransferCS updatedTransfer = UpdateTransfer(transfer.Id, transfer);
        return updatedTransfer;
    }

    public void DeleteTransfer(int id)
    {
        

        List<TransferCS> transfers = GetAllTransfers();
        TransferCS transfer = transfers.FirstOrDefault(transfer => transfer.Id == id);
        if (transfer != null)
        {
            transfers.Remove(transfer);
            var jsonData = JsonConvert.SerializeObject(transfers, Formatting.Indented);
            File.WriteAllText(_path, jsonData);
        }
    }
}
