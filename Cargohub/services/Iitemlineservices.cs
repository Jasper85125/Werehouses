using System.Collections.Generic;
using System.Threading.Tasks;
using Services;

public interface IItemLineService
{
    List<ItemLineCS> GetAllItemlines();
    ItemLineCS GetItemLineById(int id);
    Task<ItemLineCS> AddItemLine(ItemLineCS newItemType);
    Task<ItemLineCS> UpdateItemLine(int id, ItemLineCS itemLine);
    void DeleteItemLine(int id);
    List<ItemCS> GetItemsByItemLineId(int itemlineId);
    void DeleteItemLines(List<int> ids);
}
