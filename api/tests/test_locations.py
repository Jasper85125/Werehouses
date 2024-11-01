
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
