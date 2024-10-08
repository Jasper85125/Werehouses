from providers import data_provider
import unittest
import os
import json


class TestShipments(unittest.TestCase):
    def setUp(self) -> None:
        self.shipments = data_provider.Shipments("../data/")

        self.test_data_path = "./test_data/"
        os.makedirs(self.test_data_path, exist_ok=True)
        self.shipments_file = os.path.join(self.test_data_path,
                                           "shipments.json")

        self.sample_data = [
            {"id": 1, "source_id": 1, "order_id": 1, "items": [{
                "item_id": "P1",
                "amount": 1
            }]},
            {"id": 2, "source_id": 2, "order_id": 2, "items": [{
                "item_id": "P2",
                "amount": 20
            }]},
            {"id": 3, "source_id": 1, "order_id": 2, "items": [{
                "item_id": "P3",
                "amount": 30
            }]}
        ]

        with open(self.shipments_file, "w") as f:
            json.dump(self.sample_data, f)

        self.shipments = data_provider.Shipments(self.test_data_path)

    def tearDown(self) -> None:
        # Clean up the temporary data path after tests
        if os.path.exists(self.shipments_file):
            os.remove(self.shipments_file)
        if os.path.exists(self.test_data_path):
            os.rmdir(self.test_data_path)

    def test_loaded(self):
        self.assertGreater(len(self.shipments.get_shipments()), 0)

    def test_get_shipment(self):
        item = self.shipments.get_shipment(1)
        self.assertIsNotNone(item)
        self.assertEqual(item["id"], 1)

    def test_add_shipment(self):
        new_shipment = {"id": 4, "source_id": 3, "order_id": 3, "items": [{
                "item_id": "P4",
                "amount": 10
            }]}
        self.shipments.add_shipment(new_shipment)
        self.assertEqual(len(self.shipments.get_shipments()), 4)
        self.assertIsNotNone(self.shipments.get_shipment(4))

    def test_update_shipment(self):
        original_shipment = self.shipments.get_shipment(1)
        original_updated_at = original_shipment.get("updated_at")

        updated_shipment = {"id": 1, "source_id": 3, "order_id": 2,
                            "updated_at": "new_timestamp"}
        self.shipments.update_shipment(1, updated_shipment)

        shipment = self.shipments.get_shipment(1)
        self.assertNotEqual(shipment["updated_at"], original_updated_at)

    def test_remove_shipment(self):
        self.shipments.remove_shipment(1)
        self.assertIsNone(self.shipments.get_shipment(1))
        self.assertEqual(len(self.shipments.get_shipments()), 2)

    def test_get_items_in_shipment(self):
        items_in_shipment = self.shipments.get_items_in_shipment(1)
        items = [{"item_id": "P1", "amount": 1}]
        self.assertEqual(items, items_in_shipment)

    def test_update_items_in_shipment(self):
        original_shipment = self.shipments.get_shipment(1)
        original_items = original_shipment.get("items")

        updated_shipment = {"id": 1, "source_id": 3, "order_id": 2,
                            "updated_at": "new_timestamp", "items":
                            [{"item_id": "P2", "amount": 3}]}
        self.shipments.update_shipment(1, updated_shipment)

        shipment = self.shipments.get_shipment(1)
        self.assertNotEqual(shipment["items"], original_items)


if __name__ == "__main__":
    unittest.main()
