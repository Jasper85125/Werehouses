import unittest
from models.transfers import Transfers
import os
import json


class TestTransfers(unittest.TestCase):
    def setUp(self):
        # Assuming a test path or in-memory path for testing purposes
        self.test_path = "./test_data/"
        os.makedirs(self.test_path, exist_ok=True)
        
        # Mock data for testing
        self.mock_data = [
            {"id": "1", "items": [{"item_id": "item_1", "quantity": 10}], "transfer_status": "Scheduled", "created_at": "2024-01-01", "updated_at": "2024-01-01"},
            {"id": "2", "items": [{"item_id": "item_2", "quantity": 5}], "transfer_status": "In Transit", "created_at": "2024-01-02", "updated_at": "2024-01-03"},
        ]

        # Write mock data to a test file
        with open(os.path.join(self.test_path, "transfers.json"), 'w') as f:
            json.dump(self.mock_data, f)
        
        # Initialize Transfers class with the test data path
        self.transfers = Transfers(self.test_path, is_debug=False)

    def tearDown(self):
        # Clean up the test directory after each test
        if os.path.exists(self.test_path):
            for file_name in os.listdir(self.test_path):
                file_path = os.path.join(self.test_path, file_name)
                os.remove(file_path)
            os.rmdir(self.test_path)

    def test_get_transfers(self):
        """Test to ensure all transfers are returned."""
        transfers = self.transfers.get_transfers()
        self.assertEqual(len(transfers), 2)
        self.assertEqual(transfers[0]["id"], "1")
        self.assertEqual(transfers[1]["id"], "2")

    def test_get_transfer(self):
        """Test to ensure a specific transfer is fetched correctly."""
        transfer = self.transfers.get_transfer("1")
        self.assertIsNotNone(transfer)
        self.assertEqual(transfer["id"], "1")
        self.assertEqual(transfer["transfer_status"], "Scheduled")

    def test_add_transfer(self):
        """Test to ensure a new transfer is added."""
        new_transfer = {
            "id": "3",
            "items": [{"item_id": "item_3", "quantity": 20}],
            "transfer_status": "Scheduled"
        }
        self.transfers.add_transfer(new_transfer)
        transfers = self.transfers.get_transfers()
        self.assertEqual(len(transfers), 3)
        self.assertEqual(transfers[-1]["id"], "3")

    def test_update_transfer(self):
        """Test to ensure a transfer is updated."""
        updated_transfer = {
            "id": "1",
            "items": [{"item_id": "item_1", "quantity": 15}],
            "transfer_status": "Completed"
        }
        self.transfers.update_transfer("1", updated_transfer)
        transfer = self.transfers.get_transfer("1")
        self.assertEqual(transfer["transfer_status"], "Completed")
        self.assertEqual(transfer["items"][0]["quantity"], 15)

    def test_remove_transfer(self):
        """Test to ensure a transfer is removed."""
        self.transfers.remove_transfer("1")
        transfers = self.transfers.get_transfers()
        self.assertEqual(len(transfers), 1)
        self.assertIsNone(self.transfers.get_transfer("1"))

    def test_get_items_in_transfer(self):
        """Test to ensure items in a specific transfer are returned."""
        items = self.transfers.get_items_in_transfer("1")
        self.assertEqual(len(items), 1)
        self.assertEqual(items[0]["item_id"], "item_1")
        self.assertEqual(items[0]["quantity"], 10)


if __name__ == "__main__":
    unittest.main()
