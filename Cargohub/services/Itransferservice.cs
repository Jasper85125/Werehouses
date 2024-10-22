namespace Services;

public interface ITransferService
{
    List<TransferCS> GetAllTransfers();
    TransferCS GetTransferById(int id);
    TransferCS CreateTransfer(TransferCS transfer);
}