namespace Services;

public interface ITransferService
{
    List<TransferCS> GetAllTransfers();
    TransferCS GetTransferById(int id);
    List<ItemIdAndAmount> GetItemsInTransfer(int transfer_id);
    TransferCS CreateTransfer(TransferCS transfer);
    TransferCS UpdateTransfer(int id, TransferCS transfer);
    TransferCS CommitTransfer(int id);
    void DeleteTransfer(int id);
}