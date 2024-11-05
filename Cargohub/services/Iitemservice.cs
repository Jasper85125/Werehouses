namespace Services;
public interface IItemService
{
    List<ItemCS> GetAllItems();
    ItemCS GetItemById(string uid);
    ItemCS CreateItem(ItemCS item);
    ItemCS UpdateItem(string uid, ItemCS item);
    void DeleteItem(string uid);

}