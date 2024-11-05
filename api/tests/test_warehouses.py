import httpx
import unittest
import requests

def checkWarehouse(warehouse):
    required_properties = ["code", "address", "zip", "contact"]
    for prop in required_properties:
        if warehouse.get(prop) is None:
            return False
    return True

def checkLocation(location):
    required_properties = ["warehouse_id", "code", "name"]
    for prop in required_properties:
        if location.get(prop) is None:
            return False
    return True

class TestClass(unittest.TestCase):
    def setUp(self):
        self.url = "http://localhost:3000/api/v1"
        self.headers = { 'API_KEY': 'a1b2c3d4e5' }

    def test_get_warehouses(self):
        response = requests.get(url=(self.url + "/warehouses"), headers=self.headers)
        
        self.assertEqual(response.status_code, 200)
        self.assertEqual(type(response.json()), list)
        
        if len(response.json()) > 0:
            self.assertTrue(checkWarehouse(warehouse)for warehouse in response.json()[0])

    def test_get_warehouse_id(self):
        response = requests.get(url=(self.url + "/warehouses/1"), headers=self.headers)
        
        self.assertEqual(response.status_code, 200)
        self.assertEqual(type(response.json()), dict)

        if len(response.json()) == 1:
            self.assertTrue(checkWarehouse(warehouse)for warehouse in response.json())
    
    #C# fix
    def test_get_warehouse_non_existing_id(self):
        response = requests.get(url=(self.url + "/warehouses/-1"), headers=self.headers)
        
        self.assertEqual(response.status_code, 404)
        self.assertEqual(type(response.json()), type(None))

    def test_get_warehouse_id_locations(self):
        response = requests.get(url=(self.url + "/warehouses/1/locations"), headers=self.headers)
        
        self.assertEqual(response.status_code, 200)
        self.assertEqual(type(response.json()), list)
        if len(response.json()) > 0:
            self.assertTrue(checkLocation(location)for location in response.json()[0])
    
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
        "zip": None,
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
        "code": "AAAAAAA", #Required
        "name": None,
        "address": "Ohara 123", #Required
        "zip": "1234OH", #Required
        "city": None,
        "province": None,
        "country": None,
        "contact": {        #Required
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
        "code": None,           #Required
        "name": None,
        "address": "Ohara 123", #Required
        "zip": None,            #Required
        "city": None,
        "province": None,
        "country": None,
        "contact": {            #Required
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
