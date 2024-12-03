import unittest
import requests


class TestClass(unittest.TestCase):
    def setUp(self):
        self.url = "http://localhost:5125/api/v1"  # Add the base URL
        self.headers = {'API_KEY': 'a1b2c3d4e5'}

    def test_get_location_id(self):
        response = requests.get(
            url=(self.url + "/locations/1"), headers=self.headers
        )
        self.assertEqual(response.status_code, 200)
        data = response.json()
        self.assertEqual(data["id"], 98983)
        self.assertEqual(data["warehouse_id"], 373)
        self.assertEqual(data["code"], "R.E.0")
        self.assertIsNone(data["name"])
        self.assertIsNone(data["created_at"])
        self.assertIsNone(data["updated_at"])

    def test_create_location(self):
        data = {
            "id": 69696,
            "warehouse_id": 20,
            "code": "A.D.0",
            "name": None,
            "created_at": None,
        }
        response = requests.post(
            url=(self.url + "/locations"), headers=self.headers, json=data
        )
        self.assertEqual(response.status_code, 201)

    def test_update_location(self):
        data = {
            "id": 69696,
            "warehouse_id": 20,
            "code": "A.D.0",
            "created_at": None,
        }
        response = requests.put(
            url=(self.url + "/locations/1"), headers=self.headers, json=data
        )
        self.assertEqual(response.status_code, 200)

    def test_delete_location_id(self):
        response = requests.delete(
            url=(self.url + "/locations/2"), headers=self.headers
        )
        self.assertEqual(response.status_code, 200)


if __name__ == "__main__":
    unittest.main()
