from providers import data_provider
import unittest
import os
import json


class TestOrders(unittest.TestCase):
    def setUp(self) -> None:
        self.orders = data_provider.Orders("../data/")

        self.test_data_path = "./test_data/"
        os.makedirs(self.test_data_path, exist_ok=True)
        self.orders_file = os.path.join(self.test_data_path, "orders.json")
        
        # Sample data for testing
        self.sample_data = [
            {"id": 1, "source_id":1 "warehouse_id": 1, "shipment_id": 1},
            {"id": 1, "source_id":1 "warehouse_id": 1, "shipment_id": 1},
            {"id": 1, "source_id":1 "warehouse_id": 1, "shipment_id": 1}
            ]

        with open(self.orders_file, "w") as f:
            json.dump(self.sample_data, f)
        
        self.orders = Orders(self.test_data_path)
    
    def tearDown(self) -> None:
        # Clean up the temporary data path after tests
        if os.path.exists(self.orders_file):
            os.remove(self.orders_file)
        if os.path.exists(self.test_data_path):
            os.rmdir(self.test_data_path)

    def test_loaded(self):
        self.assertGreater(len(self.orders.get_orders()), 0)

    def test_get_location(self):
        item = self.orders.get_order(1)
        self.assertIsNotNone(item)
        self.assertEqual(item["id"], 1)
    
    def test_add_location(self):
        new_location = {"id": 4, "warehouse_id": 1}
        self.locations.add_location(new_location)
        self.assertEqual(len(self.locations.get_locations()), 4)
        self.assertIsNotNone(self.locations.get_location(4))
    
    def test_update_location(self):
        original_location = self.locations.get_location(1)
        original_updated_at = original_location.get("updated_at")

        updated_location = {"id": 1, "warehouse_id": 2, "updated_at": "new_timestamp"}
        self.locations.update_location(1, updated_location)

        location = self.locations.get_location(1)
        self.assertNotEqual(location["updated_at"], original_updated_at)

    def test_remove_location(self):
        self.locations.remove_location(1)
        self.assertIsNone(self.locations.get_location(1))
        self.assertEqual(len(self.locations.get_locations()), 2)


if __name__ == "__main__":
    unittest.main()