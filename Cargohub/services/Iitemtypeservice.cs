using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services;

public interface IItemtypeService
{
    List<ItemTypeCS> GetAllItemtypes();
    ItemTypeCS GetItemById(int id);
    Task<ItemTypeCS> CreateItemType(ItemTypeCS newItemType);
    Task<ItemTypeCS> UpdateItemType(int id, ItemTypeCS itemType);
}
