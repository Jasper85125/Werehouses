using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace itemtype.Services
{
    public class ItemTypeService : IItemtypeService
    {

        // Method to get all item types
        public List<ItemTypeCS> GetAllItemtypes()
        {
            var path = "data/item_types.json";
            if (!File.Exists(path))
            {
                return new List<ItemTypeCS>();
            }

            var jsonData = File.ReadAllText(path);
            var items = JsonConvert.DeserializeObject<List<ItemTypeCS>>(jsonData);
            return items ?? new List<ItemTypeCS>();
        }

        // Method to get an item type by ID
        public async Task<ItemTypeCS> GetItemTypeByIdAsync(int id)
        {
            return null;
        }

        // Method to create a new item type
        public async Task<ItemTypeCS> CreateItemTypeAsync(ItemTypeCS newItemType)
        {
            if (newItemType == null)
            {
                throw new ArgumentNullException(nameof(newItemType));
            }

            return null;
        }

        // Method to update an existing item type
        public async Task<ItemTypeCS> UpdateItemTypeAsync(ItemTypeCS updatedItemType)
        {
            if (updatedItemType == null)
            {
                throw new ArgumentNullException(nameof(updatedItemType));
            }

            return null;
        }

        // Method to delete an item type by ID
        public async Task<bool> DeleteItemTypeAsync(int id)
        {
            // Implementation for deleting an item type by ID
            return true; // Placeholder return value
        }

    }
}