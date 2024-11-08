namespace Services;

public interface ITransferService
{
    List<TransferCS> GetAllTransfers();
    TransferCS GetTransferById(int id);
    TransferCS CreateTransfer(TransferCS transfer);
    TransferCS UpdateTransfer(int id, TransferCS transfer);
    TransferCS CommitTransfer(int id, ItemIdAndAmount items);
    void DeleteTransfer(int id);
}