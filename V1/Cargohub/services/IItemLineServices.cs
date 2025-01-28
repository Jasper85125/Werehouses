using System.Collections.Generic;
using System.Threading.Tasks;
using ServicesV1;

public interface IItemLineService
{
    List<ItemLineCS> GetAllItemlines();
    ItemLineCS GetItemLineById(int id);
    ItemLineCS AddItemLine(ItemLineCS newItemType);
    ItemLineCS UpdateItemLine(int id, ItemLineCS itemLine);
    void DeleteItemLine(int id);
    List<ItemCS> GetItemsByItemLineId(int itemlineId);
}
