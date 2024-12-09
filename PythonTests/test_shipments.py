import unittest
import httpx


class TestClass(unittest.TestCase):
    def setUp(self):
        self.client = httpx.Client()
        self.url = "http://localhost:5125/api/v2"
        self.headers = {'API_KEY': 'a1b2c3d4e5'}

    def test_get_shipments(self):
        response = self.client.get(
            url=(self.url + "/shipments"), headers=self.headers)

        self.assertEqual(response.status_code, 200)

    def test_get_shipments_by_id(self):
        response = self.client.get(
            url=(self.url + "/shipments/15"), headers=self.headers)

        self.assertEqual(response.status_code, 200)

    def test_get_shipments_by_id_items(self):
        response = self.client.get(
            url=(self.url + "/shipments/15/items"), headers=self.headers)

        self.assertEqual(response.status_code, 200)

    def test_get_shipments_by_id_orders(self):
        response = self.client.get(
            url=(self.url + "/shipments/15/orders"), headers=self.headers)

        self.assertEqual(response.status_code, 405)

    def test_post_shipment(self):
        data = {
            "order_id": 9999,
            "source_id": 9999,
            "order_date": "2023-01-01T00:00:00Z",
            "request_date": "2023-01-01T00:00:00Z",
            "shipment_date": "2023-01-01T00:00:00Z",
            "shipment_type": "TypeA",
            "shipment_status": "Pending",
            "notes": "Test shipment",
            "carrier_code": "CARRIER123",
            "carrier_description": "Carrier Description",
            "service_code": "SERVICE123",
            "payment_type": "Prepaid",
            "transfer_mode": "Air",
            "total_package_count": 3,
            "total_package_weight": 10.5,
            "created_at": "2023-01-01T00:00:00Z",
            "updated_at": "2023-01-01T00:00:00Z",
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

        response = self.client.post(
            url=(self.url + "/shipments"), headers=self.headers, json=data)

        self.assertEqual(response.status_code, 201)

    def test_put_shipment_by_id(self):
        data = {
            "id": 5,
            "order_id": 9999,
            "source_id": 9999,
            "order_date": "2023-01-01T00:00:00Z",
            "request_date": "2023-01-01T00:00:00Z",
            "shipment_date": "2023-01-01T00:00:00Z",
            "shipment_type": "TypeA",
            "shipment_status": "Pending",
            "notes": "Test shipment",
            "carrier_code": "CARRIER123",
            "carrier_description": "Carrier Description",
            "service_code": "SERVICE123",
            "payment_type": "Prepaid",
            "transfer_mode": "Air",
            "total_package_count": 3,
            "total_package_weight": 10.5,
            "created_at": "2023-01-01T00:00:00Z",
            "updated_at": "2023-01-01T00:00:00Z",
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

        response = self.client.put(
            url=(self.url + "/shipments/5"), headers=self.headers, json=data)

        self.assertEqual(response.status_code, 200)

    def test_delete_shipment_by_id(self):
        response = self.client.delete(
            url=(self.url + "/shipments/5"), headers=self.headers)

        self.assertEqual(response.status_code, 200)
