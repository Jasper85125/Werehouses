namespace ServicesV2;

public interface ITransferService
{
    List<TransferCS> GetAllTransfers();
    TransferCS GetTransferById(int id);
    List<ItemIdAndAmount> GetItemsInTransfer(int transfer_id);
    TransferCS CreateTransfer(TransferCS transfer);
    List<TransferCS> CreateMultipleTransfers(List<TransferCS> transfers);
    TransferCS UpdateTransfer(int id, TransferCS transfer);
    TransferCS CommitTransfer(int id);
    public TransferCS PatchTransfer(int id, string property, object newvalue);
    void DeleteTransfer(int id);
    void DeleteTransfers(List<int> ids);
    List<TransferCS> GetLatestTransfers(int count = 5);
}
