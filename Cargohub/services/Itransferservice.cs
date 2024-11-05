namespace Services;

public interface ITransferService
{
    List<TransferCS> GetAllTransfers();
    TransferCS GetTransferById(int id);
    TransferCS CreateTransfer(TransferCS transfer);
    TransferCS UpdateTransfer(int id, TransferCS transfer);
    void DeleteTransfer(int id);
}