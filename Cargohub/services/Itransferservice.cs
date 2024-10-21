namespace Services;

public interface ITransferService
{
    List<TransferCS> GetAllTransfers();
    TransferCS GetTransferById(int id);
    // void CreateWarehouse();
    // void UpdateWarehouse();
    // void DeleteWarehouse();
    // void AttendWarehouse();
    // void ModifyWarehouse();
}