from models.inventories import Inventories
import unittest


class TestInventories(unittest.TestCase):
    def setUp(self) -> None:
        self.inventories = Inventories("../data/")

    def test_loaded(self):
        self.assertGreater(len(self.inventories.get_inventories()), 0)

    def test_get_inventories(self):
        NotEmptyList = self.inventories.get_inventories()
        self.assertGreater(len(NotEmptyList), 0)

    def test_get_inventory(self):
        exists = self.inventories.get_inventory(1)
        self.assertIsNotNone(exists)

    def test_get_inventories_for_item(self):
        in_inventories = self.inventories.get_inventories_for_item("P000001")
        self.assertIsNotNone(in_inventories)

    def test_get_inventory_totals_for_item(self):
        item_totals = self.inventories.get_inventory_totals_for_item("P000001")
        self.assertIsInstance(item_totals, dict)
        self.assertGreater(item_totals.get("total", 0), 0)

    def test_add_inventory(self):
        get_curr_inventories = self.inventories.get_inventories()
        # Create a valid inventory dictionary
        new_inventory = {
            "id": "new_id",
            "name": "New Inventory",
            "quantity": 10,
            "created_at": None  # This will be set by the add_inventory method
        }
        inventories_count = len(get_curr_inventories)
        self.inventories.add_inventory(new_inventory)
        self.assertEqual(
            len(self.inventories.get_inventories()), inventories_count + 1
        )

    def test_update_inventory(self):
        get_inventory_1 = self.inventories.get_inventory(1)
        updated_inventory = get_inventory_1
        updated_inventory["description"] = "Noooooo"
        self.inventories.update_inventory(1, updated_inventory)
        self.assertEqual(get_inventory_1["description"], "Noooooo")

    def test_remove_inventory(self):
        self.inventories.remove_inventory(1)
        self.assertIsNone(self.inventories.get_inventory(1))


if __name__ == "__main__":
    unittest.main()
