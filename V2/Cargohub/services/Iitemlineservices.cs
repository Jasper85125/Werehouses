using System.Collections.Generic;
using System.Threading.Tasks;
using ServicesV2;

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
    ItemLineCS PatchItemLine(int id, ItemLineCS itemLine);
}
