import unittest
import requests

def checkSupplier(supplier):
    required_properties = ["code", "address", "zip_code",
                           "contact_name", "phonenumber"]
    for prop in required_properties:
        if supplier.get(prop) is None:
            return False
    return True

class TestClass(unittest.TestCase):
    def setUp(self):
        self.url = "http://localhost:3000/api/v1"
        self.headers = { 'API_KEY': 'a1b2c3d4e5' }

    def test_get_suppliers(self):
        response = requests.get(url=(self.url + "/suppliers"), headers=self.headers)
        
        self.assertEqual(response.status_code, 200)

    def test_get_supplier_id(self):
        response = requests.get(url=(self.url + "/suppliers/1"), headers=self.headers)
        
        self.assertEqual(response.status_code, 200)
    
    def test_get_supplier_non_existing_id(self):
        response = requests.get(url=(self.url + "/suppliers/10000000"), headers=self.headers)
        
        self.assertEqual(response.status_code, 404)

    def test_get_items_from_supplier(self):
        response = requests.get(url=(self.url + "/suppliers/1/items"), headers=self.headers)

        self.assertEqual(response.status_code, 200)
    
    def test_get_wrong_path(self):
        response = requests.get(url=(self.url + "/suppliers/1/error"), headers=self.headers)

        self.assertEqual(response.status_code, 404)

    def test_post_supplier(self):
        data = {
        "id": 678098,
        "code": "SUP",
        "name": "FRANKY",
        "address": "SUNNY",
        "address_extra": None,
        "city": None,
        "zip_code": 12342,
        "province": "East Blue",
        "country": "Water Seven",
        "contact_name": "Iceberg",
        "phonenumber": "0612345678",
        "reference": "FRANKY-SUP",
        "created_at": None,
        "updated_at": None
        }

        response = requests.post(url=(self.url + "/suppliers"), headers=self.headers, json=data)

        self.assertTrue(checkSupplier(data))
        self.assertEqual(response.status_code, 201)
    
    def test_post_supplier_wrong_info(self):
        data = {
        "id": 678098,
        "code": "SUP",
        "name": "FRANKY",
        "address": "SUNNY",
        "address_extra": None,
        "city": None,
        "zip_code": None,
        "province": "East Blue",
        "country": "Water Seven",
        "contact_name": "Iceberg",
        "phonenumber": None,
        "reference": "FRANKY-SUP",
        "created_at": None,
        "updated_at": None
        }

        response = requests.post(url=(self.url + "/suppliers"), headers=self.headers, json=data)

        self.assertFalse(checkSupplier(data))
        self.assertEqual(response.status_code, 400)

    def test_put_supplier_id(self):
        data = {
        "id": 2,
        "code": "SUP",
        "name": "FRANKY",
        "address": "SUNNY",
        "address_extra": None,
        "city": None,
        "zip_code": 12312,
        "province": "East Blue",
        "country": "Water Severn",
        "contact_name": "Iceberg",
        "phonenumber": "0612345678",
        "reference": "FRANKY-SUP",
        "created_at": None,
        "updated_at": None
        }

        response = requests.put(url=(self.url + "/suppliers/2"), headers=self.headers, json=data)

        self.assertTrue(checkSupplier(data))
        self.assertEqual(response.status_code, 200)
    
    def test_put_supplier_id_wrong_info(self):
        data = {
        "id": 2,
        "code": "SUP",
        "name": "FRANKY",
        "address": "SUNNY",
        "address_extra": None,
        "city": None,
        "zip_code": None,
        "province": "East Blue",
        "country": "Water Severn",
        "contact_name": "Iceberg",
        "phonenumber": "0612345678",
        "reference": "FRANKY-SUP",
        "created_at": None,
        "updated_at": None
        }

        response = requests.put(url=(self.url + "/suppliers/2"), headers=self.headers, json=data)

        self.assertFalse(checkSupplier(data))
        self.assertEqual(response.status_code, 400)

    def test_delete_supplier_id(self):
        response = requests.delete(url=(self.url + "/suppliers/3"), headers=self.headers)

        self.assertEqual(response.status_code, 200)
