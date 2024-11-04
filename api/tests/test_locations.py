import unittest
import requests

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

    def test_get_locations(self):
        response = requests.get(url=(self.url + "/locations"), headers=self.headers)
        
        self.assertEqual(response.status_code, 200)

    def test_get_location_id(self):
        response = requests.get(url=(self.url + "/locations/1"), headers=self.headers)
        
        self.assertEqual(response.status_code, 200)
    
    #c# fix
    def test_get_location_non_existing_id(self):
        response = requests.get(url=(self.url + "/locations/10000000"), headers=self.headers)
        
        self.assertEqual(response.status_code, 404)

    def test_post_location(self):
        data = {
        "id": 98983,
        "warehouse_id": 373,
        "code": "R.3.0",
        "name": "Row: R, Rack: 3, Shelf: 0",
        "created_at": None,
        "updated_at": None
        }

        response = requests.post(url=(self.url + "/locations"), headers=self.headers, json=data)

        self.assertTrue(checkLocation(data))
        self.assertEqual(response.status_code, 201)
    #C# fix
    def test_post_location_wrong_info(self):
        data = {
        "id": 98983,
        "warehouse_id": 373,
        "code": None,
        "name": None,
        "created_at": None,
        "updated_at": None
        }

        response = requests.post(url=(self.url + "/locations"), headers=self.headers, json=data)

        self.assertFalse(checkLocation(data))
        self.assertEqual(response.status_code, 400)

    def test_put_location_id(self):
        data = {
        "id": 69696,
        "warehouse_id": 20, 
        "code": "A.D.0",
        "name": "Row: A, Rack: D, Shelf: 0",
        "created_at": None,
        "updated_at": None
        }

        response = requests.put(url=(self.url + "/locations/1"), headers=self.headers, json=data)

        self.assertTrue(checkLocation(data))
        self.assertEqual(response.status_code, 200)
    #C# fix
    def test_put_location_id_wrong_info(self):
        data = {
        "id": 69696,
        "warehouse_id": None, 
        "code": "A.D.0",
        "name": "Row: A, Rack: D, Shelf: 0",
        "created_at": None,
        "updated_at": None
        }

        response = requests.put(url=(self.url + "/locations/1"), headers=self.headers, json=data)

        self.assertFalse(checkLocation(data))
        self.assertEqual(response.status_code, 400)

    def test_delete_location_id(self):
        response = requests.delete(url=(self.url + "/locations/2"), headers=self.headers)

        self.assertEqual(response.status_code, 200)

if __name__ == "__main__":
    unittest.main()
