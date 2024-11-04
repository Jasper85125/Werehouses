import unittest
import requests

def checkTransfer(transfer):
    required_properties = ["items", "transfer_status"]
    for prop in required_properties:
        if transfer.get(prop) is None:
            return False
    return True

class TestClass(unittest.TestCase):
    def setUp(self):
        self.url = "http://localhost:3000/api/v1"
        self.headers = { 'API_KEY': 'a1b2c3d4e5' }

    def test_get_transfers(self):
        response = requests.get(url=(self.url + "/transfers"), headers=self.headers)
        
        self.assertEqual(response.status_code, 200)

    def test_get_transfer_id(self):
        response = requests.get(url=(self.url + "/transfers/1"), headers=self.headers)
        
        self.assertEqual(response.status_code, 200)
    
    def test_get_transfer_non_existing_id(self):
        response = requests.get(url=(self.url + "/transfers/100000000"), headers=self.headers)
        
        #self.assertEqual(response.status_code, 404)

    def test_get_items_in_transfers(self):
        response = requests.get(url=self.url + "/transfers/1/items", headers=self.headers)

        self.assertEqual(response.status_code, 200)

    def test_post_transfer(self):
        data = {
        "id": 70202,
        "reference": None,
        "transfer_from": None,
        "transfer_to": None,
        "transfer_status": "Completed",
        "created_at": None,
        "updated_at": None,
        "items": [
            {
                "item_id": "P009553",
                "amount": 50
            }
        ]
        }

        response = requests.post(url=(self.url + "/transfers"), headers=self.headers, json=data)

        self.assertTrue(checkTransfer(data))
        self.assertEqual(response.status_code, 201)

    def test_post_transfer_wrong_info(self):
        data = {
        "id": 70202,
        "reference": None,
        "transfer_from": None,
        "transfer_to": None,
        "transfer_status": None,
        "created_at": None,
        "updated_at": None,
        "items": [
            {
                "item_id": "P009553",
                "amount": 50
            }
        ]
        }

        response = requests.post(url=(self.url + "/transfers"), headers=self.headers, json=data)

        self.assertFalse(checkTransfer(data))
        #self.assertEqual(response.status_code, 400)

    def test_put_transfer_id(self):
        data = {
        "id": 99999,
        "reference": None,
        "transfer_from": None,
        "transfer_to": None,
        "transfer_status": "Transferring",
        "created_at": None,
        "updated_at": None,
        "items": [
            {
                "item_id": "P009553",
                "amount": 50
            }
        ]
        }

        response = requests.put(url=(self.url + "/transfers/2"), headers=self.headers, json=data)

        self.assertTrue(checkTransfer(data))
        self.assertEqual(response.status_code, 200)
    
    def test_put_transfer_id_wrong_info(self):
        data = {
        "id": 99999,
        "reference": None,
        "transfer_from": None,
        "transfer_to": None,
        "transfer_status": None,
        "created_at": None,
        "updated_at": None,
        "items": [
            {
                "item_id": "P009553",
                "amount": 50
            }
        ]
        }

        response = requests.put(url=(self.url + "/transfers/2"), headers=self.headers, json=data)

        self.assertFalse(checkTransfer(data))
        #self.assertEqual(response.status_code, 400)

    def test_delete_transfer_id(self):
        response = requests.delete(url=(self.url + "/transfers/3"), headers=self.headers)

        self.assertEqual(response.status_code, 200)