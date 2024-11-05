using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services;

public interface IitemGroupService
{
    List<ItemGroupCS> GetAllItemGroups();
    ItemGroupCS GetItemById(int id);
    Task<ItemGroupCS> CreateItemGroup(ItemGroupCS newItemType);
    Task<ItemGroupCS> UpdateItemGroup(int id, ItemGroupCS itemGroup);
}
