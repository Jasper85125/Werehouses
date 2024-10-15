using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace item.Services
{
    public class ItemService : IItemService
    {
        // Constructor
        public ItemService()
        {
            // Initialization code here
        }

        // Method to get all items
        public List<ItemCS> GetAllItems()
        {
            var Path = "data/items.json";
            if (!File.Exists(Path))
            {
                return new List<ItemCS>();
            }

            var jsonData = File.ReadAllText(Path);
            var items = JsonConvert.DeserializeObject<List<ItemCS>>(jsonData);
            return items ?? new List<ItemCS>();
        }

        // Method to get an item by ID
        public ItemCS GetItemById(string uid)
        {
            var items = GetAllItems();
            var item = items.FirstOrDefault(i => i.Uid == uid);
            return item;
        }

        // Method to add a new item
        public void AddItem(ItemCS item)
        {
            // Implementation code here
        }

        // Method to update an existing item
        public void UpdateItem(ItemCS item)
        {
            // Implementation code here
        }

        // Method to delete an item
        public void DeleteItem(int id)
        {
            // Implementation code here
        }
    }
}