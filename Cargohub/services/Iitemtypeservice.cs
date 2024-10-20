using System.Collections.Generic;
using System.Threading.Tasks;

namespace itemtype.Services
{
    public interface IItemtypeService
    {
        List<ItemTypeCS> GetAllItemtypes();
        Task<ItemTypeCS> GetItemTypeByIdAsync(int id);
        Task<ItemTypeCS> CreateItemTypeAsync(ItemTypeCS newItemType);
        Task<ItemTypeCS> UpdateItemTypeAsync(ItemTypeCS updatedItemType);
        Task<bool> DeleteItemTypeAsync(int id);
    }
}