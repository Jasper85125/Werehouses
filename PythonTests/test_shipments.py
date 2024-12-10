import unittest
import httpx


class TestClass(unittest.TestCase):
    def setUp(self):
        self.client = httpx.Client()
        self.versions = ["http://localhost:5001/api/v1",
                         "http://localhost:5002/api/v2"]
        self.headers = {'Api-Key': 'AdminKey'}

    def test_get_shipments(self):
        for version in self.versions:
            with self.subTest(version=version):
                response = self.client.get(
                    url=(version + "/shipments"), headers=self.headers)
                self.assertIn(response.status_code, [200, 204])

    def test_get_shipments_by_id(self):
        for version in self.versions:
            with self.subTest(version=version):
                response = self.client.get(
                    url=(version + "/shipments/1"), headers=self.headers)
                self.assertEqual(response.status_code, 200)

    def test_get_shipments_by_id_items(self):
        for version in self.versions:
            with self.subTest(version=version):
                response = self.client.get(
                    url=(version + "/shipments/1/items"), headers=self.headers)
                self.assertEqual(response.status_code, 200)

    def test_post_shipment(self):
        data = {
            "order_id": 1,
            "source_id": 1,
            "order_date": "2023-01-01T00:00:00Z",
            "request_date": "2023-01-01T00:00:00Z",
            "shipment_date": "2023-01-01T00:00:00Z",
            "shipment_type": "TypeA",
            "shipment_status": "Pending",
            "notes": "Updated test shipment",
            "carrier_code": "CARRIER123",
            "carrier_description": "Updated Carrier Description",
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

        for version in self.versions:
            with self.subTest(version=version):
                response = self.client.post(
                    url=(version + "/shipments"),
                    headers=self.headers, json=data)
                self.assertEqual(response.status_code, 201)

    def test_delete_shipment_by_id(self):
        for version in self.versions:
            with self.subTest(version=version):
                response = self.client.delete(
                    url=(version + "/shipments/10"), headers=self.headers)
                self.assertEqual(response.status_code, 200)
