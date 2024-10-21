using System.Collections.Generic;
using System.Threading.Tasks;

namespace itemgroup.Services
{
    public interface IitemgroupService
    {
        List<ItemGroupCS> GetAllItemGroups();
        ItemGroupCS GetItemById(int id);
        Task<ItemGroupCS> CreateItemGroup(ItemGroupCS newItemType);
        Task<ItemGroupCS> UpdateItemGroup(ItemGroupCS updatedItemType);
        Task<bool> DeleteItemGroup(int id);
    }
}