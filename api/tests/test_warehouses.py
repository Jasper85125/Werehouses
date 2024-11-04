import httpx
import unittest
import requests

def checkWarehouse(warehouse):
    required_properties = ["code", "address", "zip", "contact"]
    for prop in required_properties:
        if warehouse.get(prop) is None:
            return False
    return True

class TestClass(unittest.TestCase):
    def setUp(self):
        self.url = "http://localhost:3000/api/v1"
        self.headers = { 'API_KEY': 'a1b2c3d4e5' }

    def test_get_warehouses(self):
        response = requests.get(url=(self.url + "/warehouses"), headers=self.headers)
        
        self.assertEqual(response.status_code, 200)

    def test_get_warehouse_id(self):
        response = requests.get(url=(self.url + "/warehouses/1"), headers=self.headers)
        
        self.assertEqual(response.status_code, 200)
    
    #C# fix
    def test_get_warehouse_non_existing_id(self):
        response = requests.get(url=(self.url + "/warehouses/-1"), headers=self.headers)
        
        self.assertEqual(response.status_code, 404)

    def test_get_warehouse_id_locations(self):
        response = requests.get(url=(self.url + "/warehouses/1/locations"), headers=self.headers)
        
        self.assertEqual(response.status_code, 200)
    
    

    def test_get_wrong_path(self):
        response = requests.get(url=(self.url + "/warehouses/1/error"), headers=self.headers)

        self.assertEqual(response.status_code, 404)

    def test_post_warehouse(self):
        data = {
        "id": 99999,
        "code": "WAREHOUSE123",
        "name": None,
        "address": "Warestreet 123",
        "zip": "8007ST",
        "city": None,
        "province": None,
        "country": None,
        "contact": {
            "name": "Fem Keijzer",
            "phone": "(078) 0013363",
            "email": "blamore@example.net"
        },
        "created_at": None,
        "updated_at": None
        }

        response = requests.post(url=(self.url + "/warehouses"), headers=self.headers, json=data)

        self.assertTrue(checkWarehouse(data))
        self.assertEqual(response.status_code, 201)
    
    #C# fix
    def test_post_warehouse_wrong_info(self):
        data = {
        "id": None,
        "code": "WAREHOUSE123",
        "name": None,
        "address": "Warestreet 123",
        "zip": "8007ST",
        "city": None,
        "province": None,
        "country": None,
        "contact": {
            "name": "Fem Keijzer",
            "phone": "(078) 0013363",
            "email": "blamore@example.net"
        },
        "created_at": None,
        "updated_at": None
        }

        response = requests.post(url=(self.url + "/warehouses"), headers=self.headers, json=data)

        self.assertFalse(checkWarehouse(data))
        self.assertEqual(response.status_code, 400)

    def test_put_warehouse_id(self):
        data = {
        "id": 99999,
        "code": "AAAAAAA",
        "name": None,
        "address": "Ohara 123",
        "zip": "1234OH",
        "city": None,
        "province": None,
        "country": None,
        "contact": {
            "name": "Femboy Keizer",
            "phone": "(078) 0013363",
            "email": "blahahaha@example.net"
        },
        "created_at": None,
        "updated_at": None
        }

        response = requests.put(url=(self.url + "/warehouses/2"), headers=self.headers, json=data)

        self.assertTrue(checkWarehouse(data))
        self.assertEqual(response.status_code, 200)

    #C# fix
    def test_put_warehouse_id_wrong_info(self):
        data = {
        "id": 99999,
        "code": None,
        "name": None,
        "address": "Ohara 123",
        "zip": None,
        "city": None,
        "province": None,
        "country": None,
        "contact": {
            "name": "Femboy Keizer",
            "phone": "(078) 0013363",
            "email": "blahahaha@example.net"
        },
        "created_at": None,
        "updated_at": None
        }

        response = requests.put(url=(self.url + "/warehouses/2"), headers=self.headers, json=data)

        self.assertFalse(checkWarehouse(data))
        self.assertEqual(response.status_code, 400)

    def test_delete_warehouse_id(self):
        response = requests.delete(url=(self.url + "/warehouses/3"), headers=self.headers)

        self.assertEqual(response.status_code, 200)
