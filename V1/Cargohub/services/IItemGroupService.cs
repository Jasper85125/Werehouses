using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace ServicesV1;

public interface IitemGroupService
{
    List<ItemGroupCS> GetAllItemGroups();
    ItemGroupCS GetItemById(int id);
    ItemGroupCS CreateItemGroup(ItemGroupCS newItemType);
    ItemGroupCS UpdateItemGroup(int id, ItemGroupCS itemGroup);
    List<ItemCS> ItemsFromItemGroupId(int itemgroup_id);
    void DeleteItemGroup(int id);
}
