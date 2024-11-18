using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace Services;

public interface IitemGroupService
{
    List<ItemGroupCS> GetAllItemGroups();
    ItemGroupCS GetItemById(int id);
    ItemGroupCS CreateItemGroup(ItemGroupCS newItemType);
    ItemGroupCS UpdateItemGroup(int id, ItemGroupCS itemGroup);
    List<ItemCS> ItemsFromItemGroupId(int itemgroup_id);
    void DeleteItemGroup(int id);
    void DeleteItemGroups(List<int> ids);
    ItemGroupCS PatchItemGroup(int Id, ItemGroupCS itemGroup);
}
