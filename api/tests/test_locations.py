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

import httpx
import unittest

def checkLocation(location):

    if len(location) != 6:
        return False

    # po zei dat we later met hem kunnen vragen / valideren welke properties een object moet hebben,
    # maar laten we er voor nu maar uitgaan dat inprincipe elke property er moet zijn bij elke object

    # als de warehouse niet die property heeft, return False
    if location.get("id") == None:
        return False
    if location.get("warehouse_id") == None:
        return False
    if location.get("code") == None:
        return False
    if location.get("name") == None:
        return False
    if location.get("created_at") == None:
        return False
    if location.get("updated_at") == None:
        return False

    # het heeft elke property dus return true
    return True

class TestClass(unittest.TestCase):
    def setUp(self):
        self.client = httpx.Client()
        self.url = "http://localhost:3000/api/v1"
        self.headers = httpx.Headers({ 'API_KEY': 'a1b2c3d4e5' })


    def test_01_get_locations(self):
        
        # Stuur de request
        response = self.client.get(url=(self.url + "/locations"), headers=self.headers)
        
        # Check de status code
        self.assertEqual(response.status_code, 200)
        
        # Check dat de response een list is
        self.assertEqual(type(response.json()), list)
        
        # Als de list iets bevat (want een list van 0 objects is inprincipe "legaal")
        if (len(response.json()) > 0):
            # Check of de object in de list ook echt een "object" (eigenlijk overal een dictionary) is,
            # dus niet dat het een list van ints, strings etc. zijn
            self.assertEqual(type(response.json()[0]), dict)


    def test_02_get_location_id(self):
        # Stuur de request
        response = self.client.get(url=(self.url + "/locations/1"), headers=self.headers)
        
        # Check de status code
        self.assertEqual(response.status_code, 200)
        
        # Check dat de response een dictionary is (representatief voor een enkel warehouse object)
        self.assertEqual(type(response.json()), dict)
        
        # Check dat het warehouse object de juiste properties heeft
        self.assertTrue(checkLocation(response.json()))

    # deze voegt een nieuwe warehouse object
    def test_03_post_location(self):
        data = {
        "id": 99999,
        "warehouse_id": 373,
        "code": None,
        "name": None,
        "created_at": None,
        "updated_at": None
        }

        # Stuur de request
        response = self.client.post(url=(self.url + "/locations"), headers=self.headers, json=data)

        # Check de status code
        self.assertEqual(response.status_code, 201)


    
    # Overschrijft een warehouse op basis van de opgegeven warehouse-id
    def test_04_put_location_id(self):
        data = {
        "id": 69696,
        "warehouse_id": 20, 
        "code": "AAAAAAA",
        "name": None,
        "created_at": None,
        "updated_at": None
        }

        # Stuur de request
        response = self.client.put(url=(self.url + "/locations/1"), headers=self.headers, json=data)

        # Check de status code
        self.assertEqual(response.status_code, 200)

    def test_05_delete_location_id(self):
        # Stuur de request
        response = self.client.delete(url=(self.url + "/locations/2"), headers=self.headers)

        # Check de status code
        self.assertEqual(response.status_code, 200)



# to run the file: python -m unittest test_warehouses.py
# # git checkout . -f


if __name__ == "__main__":
    unittest.main()
