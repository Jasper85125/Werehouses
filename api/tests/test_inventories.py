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
        self.assertGreater(item_totals,0)
    
    def test_add_inventory(self):
        new_inventory = self.inventories.__new__(Inventories())
        inventories_count = len(self.inventories.get_inventories())
        self.inventories.add_inventory(new_inventory)
        self.assertGreater(self.inventories.get_inventories(), inventories_count)
    
    def test_update_inventory(self):
        updated_inventory = self.inventories.get_inventorty(1)
        updated_inventory["description"] = "Noooooo"
        self.inventories.update_inventory(1, updated_inventory)
        self.assertEqual(self.inventories.get_inventory(1)["description"], "Noooooo")
    
    def test_remove_inventory(self):
        self.inventories.remove_inventory(1)
        self.assertIsNone(self.inventories.get_inventory(1))



if __name__ == "__main__":
    unittest.main()