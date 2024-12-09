using System.Collections.Generic;
using System.Threading.Tasks;

namespace ServicesV2;

public interface IItemtypeService
{
    List<ItemTypeCS> GetAllItemtypes();
    ItemTypeCS GetItemById(int id);
    Task<ItemTypeCS> CreateItemType(ItemTypeCS newItemType);
    Task<ItemTypeCS> UpdateItemType(int id, ItemTypeCS itemType);
    ItemTypeCS PatchItemType(int id, string property, object newvalue);
    void DeleteItemType(int id);
    void DeleteItemTypes(List<int> ids);
}
