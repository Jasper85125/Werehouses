using Microsoft.AspNetCore.Http.Features;

namespace ServicesV2;
public interface IItemService
{
    List<ItemCS> GetAllItems();
    ItemCS GetItemById(string uid);
    ItemCS CreateItem(ItemCS item);
    List<ItemCS> CreateItems(List<ItemCS> items);
    ItemCS UpdateItem(string uid, ItemCS item);
    IEnumerable<ItemCS> GetAllItemsInItemType(int id);
    ItemCS PatchItem(string uid, string property, object newvalue);
    void DeleteItem(string uid);
    void DeleteItems(List<string> uids);

}
