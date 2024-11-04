import unittest
import requests

def checkInventory(inventory):
    required_properties = ["item_id", "locations", "total_on_hand",
                           "total_expected", "total_ordered",
                           "total_allocated", "total_available"]
    for prop in required_properties:
        if inventory.get(prop) is None:
            return False
    return True

class TestClass(unittest.TestCase):
    def setUp(self):
        self.url = "http://localhost:3000/api/v1"
        self.headers = {'API_KEY': 'a1b2c3d4e5'}

    def test_get_inventories(self):

        response = requests.get(
            url=(self.url + "/inventories"), headers=self.headers)
        self.assertEqual(response.status_code, 200)

    def test_get_inventory_id(self):
        response = requests.get(
            url=(self.url + "/inventories/1"), headers=self.headers)

        self.assertEqual(response.status_code, 200)

    # c# fix
    def test_get_inventory_non_existing_id(self):
        response = requests.get(
            url=(self.url + "/inventories/10000000000"), headers=self.headers)

        self.assertEqual(response.status_code, 404)

    def test_post_inventory(self):
        data = {
            "id": 99999,
            "item_id": 1,
            "description": None,
            "item_reference": None,
            "locations": [3211],
            "total_on_hand": 141,
            "total_expected": 2,
            "total_ordered": 15,
            "total_allocated": 11,
            "total_available": 100,
            "created_at": None,
            "updated_at": None
        }

        response = requests.post(
            url=(self.url + "/inventories"), headers=self.headers, json=data)

        self.assertTrue(checkInventory(data))
        self.assertEqual(response.status_code, 201)

    # c# fix
    def test_post_inventory_wrong_info(self):
        data = {
            "id": 99999,
            "item_id": 1,
            "description": None,
            "item_reference": None,
            "locations": [3211],
            "total_on_hand": 141,
            "total_expected": None,
            "total_ordered": None,
            "total_allocated": None,
            "total_available": 100,
            "created_at": None,
            "updated_at": None
        }

        response = requests.post(
            url=(self.url + "/inventories"), headers=self.headers, json=data)

        self.assertFalse(checkInventory(data))
        self.assertEqual(response.status_code, 400)


    def test_put_inventory_id(self):
        data = {
            "id": 99999,
            "item_id": 1,
            "description": None,
            "item_reference": None,
            "locations": [3211],
            "total_on_hand": 141,
            "total_expected": 2,
            "total_ordered": 15,
            "total_allocated": 11,
            "total_available": 100,
            "created_at": None,
            "updated_at": None
        }

        response = requests.put(
            url=(self.url + "/inventories/99999"), headers=self.headers, json=data)
        
        self.assertTrue(checkInventory(data))
        self.assertEqual(response.status_code, 200)
    
    #c# fix
    def test_put_inventory_id_wrong_info(self):
        data = {
            "id": 99999,
            "item_id": None,
            "description": None,
            "item_reference": None,
            "locations": [3211],
            "total_on_hand": 141,
            "total_expected": 2,
            "total_ordered": 15,
            "total_allocated": 11,
            "total_available": 100,
            "created_at": None,
            "updated_at": None
        }

        response = requests.put(
            url=(self.url + "/inventories/99999"), headers=self.headers, json=data)
        
        self.assertFalse(checkInventory(data))
        self.assertEqual(response.status_code, 400)

    def test_delete_inventory_id(self):
        response = requests.delete(
            url=(self.url + "/inventories/99999"), headers=self.headers)

        self.assertEqual(response.status_code, 200)
