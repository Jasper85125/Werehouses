# from models.locations import Locations
# import unittest
# import os
# import json


# class TestLocations(unittest.TestCase):
#     def setUp(self) -> None:
#         self.locations = Locations("../data/")

#         self.test_data_path = "./test_data/"
#         os.makedirs(self.test_data_path, exist_ok=True)
#         self.locations_file = os.path.join(self.test_data_path,
#                                            "locations.json")

#         # Sample data for testing
#         self.sample_data = [
#             {"id": 1, "warehouse_id": 1},
#             {"id": 2, "warehouse_id": 1},
#             {"id": 3, "warehouse_id": 2}
#         ]

#         with open(self.locations_file, "w") as f:
#             json.dump(self.sample_data, f)

#         self.locations = Locations(self.test_data_path)

#     def tearDown(self) -> None:
#         # Clean up the temporary data path after tests
#         if os.path.exists(self.locations_file):
#             os.remove(self.locations_file)
#         if os.path.exists(self.test_data_path):
#             os.rmdir(self.test_data_path)

#     def test_loaded(self):
#         self.assertGreater(len(self.locations.get_locations()), 0)

#     def test_get_locations_in_item_warehouse(self):
#         locations = self.locations.get_locations_in_warehouse(1)
#         self.assertEqual(len(locations), 2)

#     def test_get_location(self):
#         item = self.locations.get_location(1)
#         self.assertIsNotNone(item)
#         self.assertEqual(item["id"], 1)

#     def test_add_location(self):
#         new_location = {"id": 4, "warehouse_id": 1}
#         self.locations.add_location(new_location)
#         self.assertEqual(len(self.locations.get_locations()), 4)
#         self.assertIsNotNone(self.locations.get_location(4))

#     def test_update_location(self):
#         original_location = self.locations.get_location(1)
#         original_updated_at = original_location.get("updated_at")

#         updated_location = {"id": 1, "warehouse_id": 2, "updated_at":
#                             "new_timestamp"}
#         self.locations.update_location(1, updated_location)

#         location = self.locations.get_location(1)
#         self.assertNotEqual(location["updated_at"], original_updated_at)

#     def test_remove_location(self):
#         self.locations.remove_location(1)
#         self.assertIsNone(self.locations.get_location(1))
#         self.assertEqual(len(self.locations.get_locations()), 2)

import unittest
import requests

class TestClass(unittest.TestCase):
    def setUp(self):
        self.url = "http://localhost:3000/api/v1"
        self.headers = { 'API_KEY': 'a1b2c3d4e5' }

    def test_get_locations(self):
        response = requests.get(url=(self.url + "/locations"), headers=self.headers)
        
        self.assertEqual(response.status_code, 200)

    def test_get_location_id(self):
        response = requests.get(url=(self.url + "/locations/1"), headers=self.headers)
        
        self.assertEqual(response.status_code, 200)

    def test_post_location(self):
        data = {
        "id": 98983,
        "warehouse_id": 373,
        "code": "R.E.0",
        "name": None,
        "created_at": None,
        "updated_at": None
        }

        response = requests.post(url=(self.url + "/locations"), headers=self.headers, json=data)

        self.assertEqual(response.status_code, 201)

    def test_put_location_id(self):
        data = {
        "id": 69696,
        "warehouse_id": 20, 
        "code": "A.D.0",
        "name": None,
        "created_at": None,
        "updated_at": None
        }

        response = requests.put(url=(self.url + "/locations/1"), headers=self.headers, json=data)

        self.assertEqual(response.status_code, 200)

    def test_delete_location_id(self):
        response = requests.delete(url=(self.url + "/locations/2"), headers=self.headers)

        self.assertEqual(response.status_code, 200)

if __name__ == "__main__":
    unittest.main()
