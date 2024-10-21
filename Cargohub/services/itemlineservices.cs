using System;
using System.Collections.Generic;
using Newtonsoft.Json;


namespace itemlines.Services
{
    public class ItemLineService : IItemLineService
    {
        // Constructor
        public ItemLineService()
        {
            // Initialization code here
        }

        // Method to get all items
        public List<ItemLineCS> GetAllItemline()
        {
            var path = "data/item_lines.json";
            if (!File.Exists(path))
            {
                return new List<ItemLineCS>();
            }

            var jsonData = File.ReadAllText(path);
            var items = JsonConvert.DeserializeObject<List<ItemLineCS>>(jsonData);
            return items ?? new List<ItemLineCS>();
        }

        // Method to get an item by ID
        public ItemLineCS GetItemLineById(int id)
        {
            var items = GetAllItemline();
            var item = items.FirstOrDefault(i => i.Id == id);
            return item;
        }

        // Method to add a new item
        public async Task<ItemLineCS> AddItemLine(ItemLineCS item)
        {
            // Implementation code here
            return await Task.FromResult(item);
        }

        // Method to update an existing item
        public async Task<ItemLineCS> UpdateItemLine(ItemLineCS item)
        {
            // Implementation code here
            return await Task.FromResult(item);
        }

        // Method to delete an item
        public async Task<bool> DeleteItemLine(int id)
        {
            // Implementation code here
            return await Task.FromResult(true);
        }
    }
}