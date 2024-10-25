import httpx
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
        self.headers = { 'API_KEY': 'a1b2c3d4e5' }

    def test_get_warehouses(self):
        response = self.client.get(url=(self.url + "/warehouses"), headers=self.headers)
        
        self.assertEqual(response.status_code, 200)

    def test_get_warehouse_id(self):
        response = requests.get(url=(self.url + "/warehouses/1"), headers=self.headers)
        
        self.assertEqual(response.status_code, 200)

    def test_get_warehouse_id_locations(self):
        response = requests.get(url=(self.url + "/warehouses/1/locations"), headers=self.headers)
        
        self.assertEqual(response.status_code, 200)

    def test_post_warehouse(self):
        data = {
        "id": 99999,
        "code": None,
        "name": None,
        "address": None,
        "zip": None,
        "city": None,
        "province": None,
        "country": None,
        "contact": None,
        "created_at": None,
        "updated_at": None
        }

        response = requests.post(url=(self.url + "/warehouses"), headers=self.headers, json=data)

        self.assertEqual(response.status_code, 201)

    def test_put_warehouse_id(self):
        data = {
        "id": 99999,
        "code": "AAAAAAA",
        "name": None,
        "address": None,
        "zip": None,
        "city": None,
        "province": None,
        "country": None,
        "contact": None,
        "created_at": None,
        "updated_at": None
        }

        response = requests.put(url=(self.url + "/warehouses/2"), headers=self.headers, json=data)

        self.assertEqual(response.status_code, 200)

    def test_delete_warehouse_id(self):
        response = requests.delete(url=(self.url + "/warehouses/3"), headers=self.headers)

        self.assertEqual(response.status_code, 200)
