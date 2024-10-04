from models.items import Items
import unittest
import os
import json

class TestItems(unittest.TestCase):
    def setUp(self) -> None:
        # Create a temporary data path for testing
        self.test_data_path = "./test_data/"
        os.makedirs(self.test_data_path, exist_ok=True)
        self.items_file = os.path.join(self.test_data_path, "items.json")
        
        # Sample data for testing
        self.sample_data = [
            {"id": 1, "item_line": 1, "item_group": 1, "item_type": 1, "supplier_id": 1},
            {"id": 2, "item_line": 1, "item_group": 2, "item_type": 1, "supplier_id": 2},
            {"id": 3, "item_line": 2, "item_group": 1, "item_type": 2, "supplier_id": 1}
        ]
        
        with open(self.items_file, "w") as f:
            json.dump(self.sample_data, f)
        
        self.items = Items(self.test_data_path)

    def tearDown(self) -> None:
        # Clean up the temporary data path after tests
        if os.path.exists(self.items_file):
            os.remove(self.items_file)
        if os.path.exists(self.test_data_path):
            os.rmdir(self.test_data_path)

    def test_loaded(self):
        self.assertGreater(len(self.items.get_items()), 0)

    def test_get_item(self):
        item = self.items.get_item(1)
        self.assertIsNotNone(item)
        self.assertEqual(item["id"], 1)

    def test_get_items_for_item_line(self):
        items = self.items.get_items_for_item_line(1)
        self.assertEqual(len(items), 2)

    def test_get_items_for_item_group(self):
        items = self.items.get_items_for_item_group(1)
        self.assertEqual(len(items), 2)

    def test_get_items_for_item_type(self):
        items = self.items.get_items_for_item_type(1)
        self.assertEqual(len(items), 2)

    def test_get_items_for_supplier(self):
        items = self.items.get_items_for_supplier(1)
        self.assertEqual(len(items), 2)

    def test_add_item(self):
        new_item = {"id": 4, "item_line": 3, "item_group": 3, "item_type": 3, "supplier_id": 3}
        self.items.add_item(new_item)
        self.assertEqual(len(self.items.get_items()), 4)
        self.assertIsNotNone(self.items.get_item(4))
    
    def test_update_item(self):
        original_item = self.items.get_item(1)
        original_updated_at = original_item.get("updated_at")

        updated_item = {"id": 1, "item_line": 1, "item_group": 1, "item_type": 1, "supplier_id": 1, "updated_at": "new_timestamp"}
        self.items.update_item(1, updated_item)

        item = self.items.get_item(1)
        self.assertNotEqual(item["updated_at"], original_updated_at)

    def test_remove_item(self):
        self.items.remove_item(1)
        self.assertIsNone(self.items.get_item(1))
        self.assertEqual(len(self.items.get_items()), 2)

if __name__ == "__main__":
    unittest.main()