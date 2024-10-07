from models.item_groups import ItemGroups
import unittest
from unittest.mock import patch, mock_open
import json

class TestItemGroups(unittest.TestCase):
    def setUp(self) -> None:
        self.test_data = json.dumps([{
            "id": 1,
            "name": "Sample Group",
            "description": "This is a sample item group."
        }])
        self.itemGroups = ItemGroups(root_path="test_path", is_debug=True)
        self.sample_item_group = {
            "id": 1,
            "name": "Sample Group",
            "description": "This is a sample item group."
        }
        self.itemGroups.add_item_group(self.sample_item_group)

    @patch("builtins.open", new_callable=mock_open, read_data='[]')
    def test_loaded(self, mock_file):
        self.assertGreater(len(self.itemGroups.get_item_groups()), 0)

    @patch("builtins.open", new_callable=mock_open, read_data='[]')
    def test_get_item_group(self, mock_file):
        item_group = self.itemGroups.get_item_group(1)
        self.assertIsNotNone(item_group)
        self.assertEqual(item_group["name"], "Sample Group")

    @patch("builtins.open", new_callable=mock_open, read_data='[]')
    def test_add_item_group(self, mock_file):
        new_item_group = {
            "id": 2,
            "name": "New Group",
            "description": "This is a new item group."
        }
        self.itemGroups.add_item_group(new_item_group)
        self.assertEqual(len(self.itemGroups.get_item_groups()), 2)
        self.assertIsNotNone(self.itemGroups.get_item_group(2))

    @patch("builtins.open", new_callable=mock_open, read_data='[]')
    def test_update_item_group(self, mock_file):
        updated_item_group = {
            "id": 1,
            "name": "Updated Group",
            "description": "This is an updated item group."
        }
        self.itemGroups.update_item_group(1, updated_item_group)
        item_group = self.itemGroups.get_item_group(1)
        self.assertEqual(item_group["name"], "Updated Group")

    # @patch("builtins.open", new_callable=mock_open, read_data='[]')
    # def test_remove_item_group(self, mock_file):
    #     self.itemGroups.remove_item_group(1)
    #     self.assertIsNone(self.itemGroups.get_item_group(1))
    #     self.assertEqual(len(self.itemGroups.get_item_groups()), 0)

    # @patch("builtins.open", new_callable=mock_open)
    # def test_save_and_load(self, mock_file):
    #     # Mock the file operations for save
    #     mock_file().write = lambda x: None
    #     self.itemGroups.save()
        
    #     # Mock the file operations for load
    #     mock_file().read = lambda: json.dumps([{
    #         "id": 1,
    #         "name": "Sample Group",
    #         "description": "This is a sample item group."
    #     }])
        
    #     new_itemGroups = ItemGroups(root_path="test_path", is_debug=True)
    #     new_itemGroups.load(is_debug=True)
    #     self.assertEqual(len(new_itemGroups.get_item_groups()), 1)
    #     self.assertEqual(new_itemGroups.get_item_group(1)["name"], "Sample Group")


if __name__ == "__main__":
    unittest.main()