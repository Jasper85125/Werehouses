from models.warehouses import Warehouses
import unittest
import json
import os


class TestWarehouses(unittest.TestCase):
    def setUp(self):
        # Assuming a test path for testing purposes
        self.test_path = "./test_data/"
        os.makedirs(self.test_path, exist_ok=True)
        
        # Mock data for testing
        self.mock_data = [
            {"id": "1", "name": "Main Warehouse", "location": "Location A", "capacity": 1000},
            {"id": "2", "name": "Secondary Warehouse", "location": "Location B", "capacity": 500},
        ]
        
        # Write mock data to a test file
        with open(os.path.join(self.test_path, "warehouses.json"), 'w') as f:
            json.dump(self.mock_data, f)
        
        # Initialize Warehouses class with the test data path
        self.warehouses = Warehouses(self.test_path, is_debug=False)

    def tearDown(self):
        # Clean up the test directory after each test
        if os.path.exists(self.test_path):
            for file_name in os.listdir(self.test_path):
                file_path = os.path.join(self.test_path, file_name)
                os.remove(file_path)
            os.rmdir(self.test_path)

    def test_get_warehouses(self):
        """Test fetching all warehouses"""
        warehouses = self.warehouses.get_warehouses()
        self.assertEqual(len(warehouses), 2)
        self.assertEqual(warehouses[0]["name"], "Main Warehouse")

    def test_get_warehouse(self):
        """Test fetching a specific warehouse by ID"""
        warehouse = self.warehouses.get_warehouse("1")
        self.assertIsNotNone(warehouse)
        self.assertEqual(warehouse["name"], "Main Warehouse")

    def test_add_warehouse(self):
        """Test adding a new warehouse"""
        new_warehouse = {
            "id": "3",
            "name": "New Warehouse",
            "location": "Location C",
            "capacity": 700
        }

        self.warehouses.add_warehouse(new_warehouse)
        warehouses = self.warehouses.get_warehouses()
        self.assertEqual(len(warehouses), 3)
        self.assertEqual(warehouses[-1]["name"], "New Warehouse")

    def test_update_warehouse(self):
        """Test updating an existing warehouse"""
        updated_warehouse = {
            "id": "1",
            "name": "Updated Warehouse",
            "location": "Updated Location",
            "capacity": 1200
        }

        self.warehouses.update_warehouse("1", updated_warehouse)
        warehouse = self.warehouses.get_warehouse("1")
        self.assertEqual(warehouse["name"], "Updated Warehouse")
        self.assertEqual(warehouse["capacity"], 1200)

    def test_remove_warehouse(self):
        """Test removing a warehouse"""
        self.warehouses.remove_warehouse("1")
        warehouses = self.warehouses.get_warehouses()
        self.assertEqual(len(warehouses), 1)
        self.assertIsNone(self.warehouses.get_warehouse("1"))


if __name__ == "__main__":
    unittest.main()
