namespace Services;
public interface IItemService
{
    List<ItemCS> GetAllItems();
    ItemCS GetItemById(string uid);
    ItemCS CreateItem(ItemCS item);

}