using Microsoft.AspNetCore.Http.Features;

namespace Services;
public interface IItemService
{
    List<ItemCS> GetAllItems();
    ItemCS GetItemById(string uid);
    ItemCS CreateItem(ItemCS item);
    ItemCS UpdateItem(string uid, ItemCS item);
    IEnumerable<ItemCS> GetAllItemsInItemType(int id);
    void DeleteItem(string uid);

}