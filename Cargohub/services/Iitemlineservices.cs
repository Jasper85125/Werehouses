using System.Collections.Generic;
using System.Threading.Tasks;
using Services;

public interface IItemLineService
{
    List<ItemLineCS> GetAllItemlines();
    ItemLineCS GetItemLineById(int id);
    ItemLineCS AddItemLine(ItemLineCS newItemType);
    List<ItemLineCS> CreateMultipleItemLines(List<ItemLineCS>newItemLine);
    ItemLineCS UpdateItemLine(int id, ItemLineCS itemLine);
    void DeleteItemLine(int id);
    List<ItemCS> GetItemsByItemLineId(int itemlineId);
    void DeleteItemLines(List<int> ids);
}
