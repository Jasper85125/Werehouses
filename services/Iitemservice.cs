public interface IItemService
{
    List<ItemCS> GetAllItems();
    ItemCS GetItemById(string uid);
    // void CreateItem();
    // void UpdateItem();
    // void DeleteItem();
    // void AttendItem();
    // void ModifyItem();
}