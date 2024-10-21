using System.Collections.Generic;
using System.Threading.Tasks;

namespace itemlines.Services
{
    public interface IItemLineService
    {
        List<ItemLineCS> GetAllItemline();
        ItemLineCS GetItemLineById(int id);
        Task<ItemLineCS> AddItemLine(ItemLineCS newItemType);
        Task<ItemLineCS> UpdateItemLine(ItemLineCS updatedItemType);
        Task<bool> DeleteItemLine(int id);
    }
}