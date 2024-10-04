from models.item_types import ItemTypes
import unittest
import os
import json

class TestItemTypes(unittest.TestCase):
    def setUp(self) -> None:
        self.test_data_path = "../data/test_item_types.json"
        self.itemTypes = ItemTypes(self.test_data_path, is_debug=True)
        self.sample_item_type = {
            "id": 1,
            "name": "Sample Item",
            "description": "This is a sample item type."
        }
        self.itemTypes.add_item_type(self.sample_item_type)

    def tearDown(self) -> None:
        if os.path.exists(self.test_data_path):
            os.remove(self.test_data_path)

    def test_loaded(self):
        self.assertGreater(len(self.itemTypes.get_item_types()), 0)

    def test_get_item_type(self):
        item_type = self.itemTypes.get_item_type(1)
        self.assertIsNotNone(item_type)
        self.assertEqual(item_type["name"], "Sample Item")

    def test_add_item_type(self):
        new_item_type = {
            "id": 2,
            "name": "New Item",
            "description": "This is a new item type."
        }
        self.itemTypes.add_item_type(new_item_type)
        self.assertEqual(len(self.itemTypes.get_item_types()), 2)
        self.assertIsNotNone(self.itemTypes.get_item_type(2))

    def test_update_item_type(self):
        updated_item_type = {
            "id": 1,
            "name": "Updated Item",
            "description": "This is an updated item type."
        }
        self.itemTypes.update_item_type(1, updated_item_type)
        item_type = self.itemTypes.get_item_type(1)
        self.assertEqual(item_type["name"], "Updated Item")

    def test_remove_item_type(self):
        self.itemTypes.remove_item_type(1)
        self.assertIsNone(self.itemTypes.get_item_type(1))
        self.assertEqual(len(self.itemTypes.get_item_types()), 0)

    def test_save_and_load(self):
        self.itemTypes.save()
        new_item_types = ItemTypes(self.test_data_path)
        self.assertEqual(len(new_item_types.get_item_types()), 1)
        self.assertEqual(new_item_types.get_item_type(1)["name"], "Sample Item")


if __name__ == "__main__":
    unittest.main()