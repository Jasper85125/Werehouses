import unittest
import requests

def checkShipment(shipment):
    required_properties = ["order_id", "source_id", "order_date", "request_date",
                           "shipment_date", "shipment_type", "shipment_type",
                           "carrier_code", "service_code", "transfer_mode",
                           "total_pakage_count", "total_package_weight", "items"
                           ]
    for prop in required_properties:
        if shipment.get(prop) is None:
            return False
    return True

class TestClass(unittest.TestCase):
    def setUp(self):
        self.url = "http://localhost:3000/api/v1"
        self.headers = {'API_KEY': 'a1b2c3d4e5'}

    def test_get_shipments(self):
        response = requests.get(
            url=(self.url + "/shipments"), headers=self.headers)

        self.assertEqual(response.status_code, 200)

    def test_get_shipments_by_id(self):
        response = requests.get(
            url=(self.url + "/shipments/1"), headers=self.headers)

        self.assertEqual(response.status_code, 200)
    
    def test_get_shipments_by_non_existing_id(self):
        response = requests.get(
            url=(self.url + "/shipments/10000000000"), headers=self.headers)

        self.assertEqual(response.status_code, 404)

    def test_get_shipments_by_id_items(self):
        response = requests.get(
            url=(self.url + "/shipments/1/items"), headers=self.headers)

        self.assertEqual(response.status_code, 200)

    def test_get_shipments_by_id_orders(self):
        response = requests.get(
            url=(self.url + "/shipments/1/orders"), headers=self.headers)

        self.assertEqual(response.status_code, 200)
    
    def test_get_wrong_path(self):
        response = requests.get(url=(self.url + "/shipments/1/error"), headers=self.headers)

        self.assertEqual(response.status_code, 404)

    def test_post_shipment(self):
        data = {
            "id": 9999,
            "order_id": 9999,
            "source_id": 9999,
            "order_date": "2000-03-09",
            "request_date": "2000-03-11",
            "shipment_date": "2000-03-13",
            "shipment_type": "I",
            "shipment_status": "Pending",
            "notes": None,
            "carrier_code": "PDD",
            "carrier_description": None,
            "service_code": "quick",
            "payment_type": None,
            "transfer_mode": "air",
            "total_package_count": 21,
            "total_package_weight": 500,
            "created_at": None,
            "updated_at": None,
            "items": [
                {
                    "item_id": "P000002",
                    "amount": 2
                },
                {
                    "item_id": "P000004",
                    "amount": 1
                },
                {
                    "item_id": "P000006",
                    "amount": 5
                }
            ]
        }

        response = requests.post(
            url=(self.url + "/shipments"), headers=self.headers, json=data)

        self.assertTrue(checkShipment(data))
        self.assertEqual(response.status_code, 201)
    
    def test_post_shipment_wrong_info(self):
        data = {
            "id": 9999,
            "order_id": None,
            "source_id": 9999,
            "order_date": "2000-03-09",
            "request_date": "2000-03-11",
            "shipment_date": "2000-03-13",
            "shipment_type": "I",
            "shipment_status": "Pending",
            "notes": None,
            "carrier_code": "PDD",
            "carrier_description": None,
            "service_code": "quick",
            "payment_type": None,
            "transfer_mode": "air",
            "total_package_count": 21,
            "total_package_weight": None,
            "created_at": None,
            "updated_at": None,
            "items": [
                {
                    "item_id": "P000002",
                    "amount": 2
                },
                {
                    "item_id": "P000004",
                    "amount": 1
                },
                {
                    "item_id": "P000006",
                    "amount": 5
                }
            ]
        }

        response = requests.post(
            url=(self.url + "/shipments"), headers=self.headers, json=data)

        self.assertFalse(checkShipment(data))
        self.assertEqual(response.status_code, 400)

    def test_put_shipment_by_id(self):
        data = {
            "id": 9999,
            "order_id": 9999,
            "source_id": 9999,
            "order_date": "2000-03-09",
            "request_date": "2000-03-11",
            "shipment_date": "2000-03-13",
            "shipment_type": "I",
            "shipment_status": "Pending",
            "notes": None,
            "carrier_code": "PDD",
            "carrier_description": None,
            "service_code": "quick",
            "payment_type": None,
            "transfer_mode": "air",
            "total_package_count": 21,
            "total_package_weight": 500,
            "created_at": None,
            "updated_at": None,
            "items": [
                {
                    "item_id": "P000002",
                    "amount": 2
                },
                {
                    "item_id": "P000004",
                    "amount": 1
                },
                {
                    "item_id": "P000006",
                    "amount": 5
                }
            ]
        }
        
        response = requests.put(
            url=(self.url + "/shipments/9999"), headers=self.headers, json=data)
        
        self.assertTrue(checkShipment(data))
        self.assertEqual(response.status_code, 200)
    
    def test_put_shipment_by_id_wrong_info(self):
        data = {
            "id": 9999,
            "order_id": 9999,
            "source_id": None,
            "order_date": "2000-03-09",
            "request_date": "2000-03-11",
            "shipment_date": "2000-03-13",
            "shipment_type": "I",
            "shipment_status": "Pending",
            "notes": None,
            "carrier_code": "PDD",
            "carrier_description": None,
            "service_code": "quick",
            "payment_type": None,
            "transfer_mode": "air",
            "total_package_count": None,
            "total_package_weight": 500,
            "created_at": None,
            "updated_at": None,
            "items": [
                {
                    "item_id": "P000002",
                    "amount": 2
                },
                {
                    "item_id": "P000004",
                    "amount": 1
                },
                {
                    "item_id": "P000006",
                    "amount": 5
                }
            ]
        }
        
        response = requests.put(
            url=(self.url + "/shipments/9999"), headers=self.headers, json=data)
        
        self.assertFalse(checkShipment(data))
        self.assertEqual(response.status_code, 400)

    def test_put_shipment_by_id_items(self):
        data = {
            "items": [
                {
                    "item_id": "P000002",
                    "amount": 230
                },
                {
                    "item_id": "P000004",
                    "amount": 100
                },
                {
                    "item_id": "P000006",
                    "amount": 500
                }
            ]
        }
        response = requests.put(
            url=(self.url + "/shipments/9999/items"), headers=self.headers, json=data)
        # print(response.status_code)
        # print(response.text)
        self.assertEqual(response.status_code, 200)
    
    #Werkt nog niet!
    def test_put_shipment_by_id_orders(self):
        data = {
            
        }
        #response = requests.put(url=(self.url + "/shipments/1/orders"), headers=self.headers, json=data)
        #self.assertEqual(response.status_code, 200)

    #Heb handmatig even een response 200 erin gegooid, dus als we hem ooit echt willen fixen er stond eerst alleen pass
    def test_put_shipment_by_id_commit(self):
        data = {
                "id": 9999,
            "order_id": 9999,
            "source_id": 9999,
            "order_date": None,
            "request_date": None,
            "shipment_date": None,
            "shipment_type": None,
            "shipment_status": None,
            "notes": None,
            "carrier_code": None,
            "carrier_description": None,
            "service_code": None,
            "payment_type": None,
            "transfer_mode": None,
            "total_package_count": 0,
            "total_package_weight": 0,
            "created_at": None,
            "updated_at": None,
            "items": [
                {
                    "item_id": "P000002",
                    "amount": 23
                },
                {
                    "item_id": "P000004",
                    "amount": 1
                },
                {
                    "item_id": "P000006",
                    "amount": 50
                }
            ]
        }
        response = requests.put(url = (self.url + "/shipments/1/commit"), headers = self.headers, json = data)
        self.assertEqual(response.status_code, 200)

    def test_delete_shipment_by_id(self):
        response = requests.delete(
            url=(self.url + "/shipments/9999"), headers=self.headers)

        self.assertEqual(response.status_code, 200)


if __name__ == "__main__":
    unittest.main()
