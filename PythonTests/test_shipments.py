import unittest
import requests


class TestClass(unittest.TestCase):
    def setUp(self):
        self.url = "http://localhost:5125/api/v2"
        self.headers = {'API_KEY': 'a1b2c3d4e5'}

    def test_get_shipments(self):
        response = requests.get(
            url=(self.url + "/shipments"), headers=self.headers)

        self.assertEqual(response.status_code, 200)

    def test_get_shipments_by_id(self):
        response = requests.get(
            url=(self.url + "/shipments/1"), headers=self.headers)

        self.assertEqual(response.status_code, 200)

    def test_get_shipments_by_id_items(self):
        response = requests.get(
            url=(self.url + "/shipments/1/items"), headers=self.headers)

        self.assertEqual(response.status_code, 200)

    def test_get_shipments_by_id_orders(self):
        response = requests.get(
            url=(self.url + "/shipments/1/orders"), headers=self.headers)

        self.assertEqual(response.status_code, 200)

    def test_post_shipment(self):
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

        self.assertEqual(response.status_code, 201)

    def test_put_shipment_by_id(self):
        data = {
            "id": 4,
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
        response = requests.put(
            url=(self.url + "/shipments/4"),
            headers=self.headers,
            json=data)
        self.assertEqual(response.status_code, 200)

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
            url=(self.url + "/shipments/4/items"),
            headers=self.headers,
            json=data
        )
        # print(response.status_code)
        # print(response.text)
        self.assertEqual(response.status_code, 200)

    def test_delete_shipment_by_id(self):
        response = requests.delete(
            url=(self.url + "/shipments/5"), headers=self.headers)

        self.assertEqual(response.status_code, 200)


if __name__ == "__main__":
    unittest.main()
