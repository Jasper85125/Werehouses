import unittest
import requests


class TestClass(unittest.TestCase):
    def setUp(self):
        self.url = "http://localhost:5125/api/v2"
        self.headers = {'API_KEY': 'a1b2c3d4e5'}

    def test_get_transfers(self):
        response = requests.get(
            url=(self.url + "/transfers"), headers=self.headers)
        self.assertEqual(response.status_code, 200)
        self.assertEqual(response.status_code, 200)

        response = requests.get(
            url=(self.url + "/transfers/1"), headers=self.headers)
        response = requests.get(
            url=(self.url + "/transfers/1"), headers=self.headers
        )

        self.assertEqual(response.status_code, 200)
        response = requests.get(
            url=self.url + "/transfers/1/items", headers=self.headers)

    def test_get_items_in_transfers(self):
        response = requests.get(
            url=self.url + "/transfers/1/items", headers=self.headers
        )

        self.assertEqual(response.status_code, 200)
        data = {
            "id": 70202,
            "reference": None,
            "transfer_from": None,
            "transfer_to": None,
            "transfer_status": None,
            "created_at": None,
            "updated_at": None,
            "items": None
        }
        response = requests.post(
            url=(self.url + "/transfers"), headers=self.headers, json=data)

        data = {
            "id": 99999,
            "reference": None,
            "transfer_from": None,
            "transfer_to": None,
            "transfer_status": None,
            "created_at": None,
            "updated_at": None,
            "items": None
        }
        response = requests.put(
            url=(self.url + "/transfers/2"), headers=self.headers, json=data)

        response = requests.delete(
            url=(self.url + "/transfers/3"), headers=self.headers)

        self.assertEqual(response.status_code, 200)

    def test_delete_transfer_id(self):
        response = requests.delete(
            url=(self.url + "/transfers/3"), headers=self.headers)

        self.assertEqual(response.status_code, 200)
