import unittest
import requests


class TestOrdersAPI(unittest.TestCase):
    def setUp(self):
        self.url = "http://localhost:5125/api/v2"
        self.headers = {'API_KEY': 'a1b2c3d4e5'}

    def test_get_orders(self):
        response = requests.get(f"{self.url}/orders", headers=self.headers)
        self.assertEqual(response.status_code, 200)

    def test_get_order_by_id(self):
        response = requests.get(f"{self.url}/orders/1", headers=self.headers)
        self.assertEqual(response.status_code, 200)

    def test_post_order(self):
        data = {
            "id": 4,
            "source_id": 3,
            "warehouse_id": 2,
            "shipment_id": 1,
            "items": [{"item_id": "P000001", "amount": 1}],
            "ship_to": 1
        }
        response = requests.post(
            f"{self.url}/orders", headers=self.headers, json=data
        )
        self.assertEqual(response.status_code, 201)

    def test_put_order(self):
        data = {
            "id": 1,
            "source_id": 3,
            "warehouse_id": 2,
            "updated_at": "new_timestamp"
        }
        response = requests.put(
            f"{self.url}/orders/1", headers=self.headers, json=data
        )
        self.assertEqual(response.status_code, 200)

    def test_get_items_in_order(self):
        response = requests.get(
            f"{self.url}/orders/1/items", headers=self.headers
        )
        self.assertEqual(response.status_code, 200)
        response = requests.get(
            f"{self.url}/clients/1/orders", headers=self.headers
        )

    def test_get_orders_for_client(self):
        response = requests.get(
            f"{self.url}/shipments/1/orders", headers=self.headers
        )
        self.assertEqual(response.status_code, 200)

    def test_get_orders_in_shipment(self):
        response = requests.get(
            f"{self.url}/shipments/1/orders", headers=self.headers
        )
        self.assertEqual(response.status_code, 200)


if __name__ == "__main__":
    unittest.main()
