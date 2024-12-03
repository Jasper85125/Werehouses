# import httpx
import unittest
import requests

# class TestWarehouses(unittest.TestCase):
#     def setUp(self) -> None:
#         self.warehouses = Warehouses("../data/")

#     def test_loaded(self):
#         self.assertGreater(len(self.warehouses.get_warehouses()), 0)


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

    def test_post_inventory(self):
        data = {
            "id": 99999,
            "item_id": None,
            "description": None,
            "item_reference": None,
            "locations": None,
            "total_on_hand": None,
            "total_expected": None,
            "total_ordered": None,
            "total_allocated": None,
            "total_available": None,
            "created_at": None,
            "updated_at": None
        }

        response = requests.post(
            url=(self.url + "/inventories"), headers=self.headers, json=data)

        self.assertEqual(response.status_code, 201)

    def test_put_inventory_id(self):
        data = {
            "id": 99999,
            "item_id": None,
            "description": None,
            "item_reference": None,
            "locations": None,
            "total_on_hand": None,
            "total_expected": None,
            "total_ordered": None,
            "total_allocated": None,
            "total_available": None,
            "created_at": None,
            "updated_at": None
        }

        response = requests.put(
            url=(self.url + "/inventories/99999"),
            headers=self.headers,
            json=data
        )

        self.assertEqual(response.status_code, 200)

    def test_delete_inventory_id(self):
        response = requests.delete(
            url=(self.url + "/inventories/99999"), headers=self.headers)

        self.assertEqual(response.status_code, 200)
