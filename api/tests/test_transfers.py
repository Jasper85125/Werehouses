import unittest
import requests

class TestClass(unittest.TestCase):
    def setUp(self):
        self.url = "http://localhost:3000/api/v1"
        self.headers = { 'API_KEY': 'a1b2c3d4e5' }

    def test_get_transfers(self):
        response = requests.get(url = (self.url + "/transfers"), headers = self.headers)
        
        self.assertEqual(response.status_code, 200)

    def test_get_transfer_id(self):
        response = requests.get(url = (self.url + "/transfers/1"), headers = self.headers)
        
        self.assertEqual(response.status_code, 200)

    def test_get_items_in_transfers(self):
        response = requests.get(url = self.url + "/transfers/1/items", headers = self.headers)

        self.assertEqual(response.status_code, 200)
    
    def test_get_wrong_path(self):
        response = requests.get(url = (self.url + "/transfers/1/error"), headers = self.headers)

        self.assertEqual(response.status_code, 404)

    def test_post_transfer(self):
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

        response = requests.post(url = (self.url + "/transfers"), headers = self.headers, json=data)

        self.assertEqual(response.status_code, 201)

    def test_put_transfer_id(self):
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

        response = requests.put(url = (self.url + "/transfers/2"), headers = self.headers, json=data)

        self.assertEqual(response.status_code, 200)
    
    def test_put_transfer_id_commit(self):
        data = {
            "id": 1,
            "reference": "TR00001",
            "transfer_from": None,
            "transfer_to": 9229,
            "transfer_status": "Completed",
            "created_at": "2000-03-11T13:11:14Z",
            "updated_at": "2000-03-12T16:11:14Z",
            "items": [{
                       "item_id": "P007435",
                       "amount": 23
                      }
                     ]
        }
        response = requests.put(url = (self.url + "/transfers/1/commit"), headers = self.headers, json = data)
        self.assertEqual(response.status_code, 200)

    def test_delete_transfer_id(self):
        response = requests.delete(url = (self.url + "/transfers/3"), headers = self.headers)

        self.assertEqual(response.status_code, 200)