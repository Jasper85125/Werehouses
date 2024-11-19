using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Services;

public interface IitemGroupService
{
    List<ItemGroupCS> GetAllItemGroups();
    ItemGroupCS GetItemById(int id);
    Task<ItemGroupCS> CreateItemGroup(ItemGroupCS newItemType);
    Task<ItemGroupCS> UpdateItemGroup(int id, ItemGroupCS itemGroup);
    List<ItemCS> ItemsFromItemGroupId(int itemgroup_id);
    void DeleteItemGroup(int id);
}
