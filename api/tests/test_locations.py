from models.locations import Locations
import unittest
import os
import json


class TestLocations(unittest.TestCase):
    def setUp(self) -> None:
        self.locations = Locations("../data/")
    
        self.test_data_path = "./test_data/"
        os.makedirs(self.test_data_path, exist_ok=True)
        self.locations_file = os.path.join(self.test_data_path, "locations.json")
        
        # Sample data for testing
        self.sample_data = [
            {"id": 1, "warehouse_id": 1},
            {"id": 2, "warehouse_id": 1},
            {"id": 3, "warehouse_id": 2}
        ]
        
        with open(self.locations_file, "w") as f:
            json.dump(self.sample_data, f)
        
        self.locations = Locations(self.test_data_path)
    
    def tearDown(self) -> None:
        # Clean up the temporary data path after tests
        if os.path.exists(self.locations_file):
            os.remove(self.locations_file)
        if os.path.exists(self.test_data_path):
            os.rmdir(self.test_data_path)

    def test_loaded(self):
        self.assertGreater(len(self.locations.get_locations()), 0)
    
    def test_get_Location(self):
        item = self.locations.get_location(1)
        self.assertIsNotNone(item)
        self.assertEqual(item["id"], 1)


if __name__ == "__main__":
    unittest.main()