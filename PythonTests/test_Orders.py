import unittest
import httpx


def checkOrder(order):
    if len(order) != 18:
        return False

    if order.get("id") is None:
        return False
    if order.get("client_id") is None:
        return False
    if order.get("order_date") is None:
        return False
    if order.get("request_date") is None:
        return False
    if order.get("shipment_date") is None:
        return False
    if order.get("shipment_type") is None:
        return False
    if order.get("shipment_status") is None:
        return False
    if order.get("notes") is None:
        return False
    if order.get("carrier_code") is None:
        return False
    if order.get("carrier_description") is None:
        return False
    if order.get("service_code") is None:
        return False
    if order.get("payment_type") is None:
        return False
    if order.get("transfer_mode") is None:
        return False
    if order.get("total_package_count") is None:
        return False
    if order.get("total_package_weight") is None:
        return False
    if order.get("created_at") is None:
        return False
    if order.get("updated_at") is None:
        return False

    return True


class TestOrdersAPI(unittest.TestCase):
    def setUp(self):
        self.client = httpx.Client()
        self.headers = {'Api-Key': 'AdminKey'}
        self.versions = ["http://localhost:5001/api/v1", 
                         "http://localhost:5002/api/v2"]

    def test_01_post_order(self):
        data = {
            "id": 9999,
            "client_id": 1,
            "order_date": "2023-01-01T00:00:00Z",
            "request_date": "2023-01-01T00:00:00Z",
            "shipment_date": "2023-01-01T00:00:00Z",
            "shipment_type": "Standard",
            "shipment_status": "Pending",
            "notes": "New order",
            "carrier_code": "UPS",
            "carrier_description": "United Parcel Service",
            "service_code": "Ground",
            "payment_type": "Credit Card",
            "transfer_mode": "Online",
            "total_package_count": 1,
            "total_package_weight": 1.5,
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
                }
            ]
        }
        for version in self.versions:
            response = self.client.post(
                url=(version + "/orders"), headers=self.headers, json=data)

            self.assertIn(response.status_code, [201, 405])

    def test_02_get_orders_v1(self):
        response = self.client.get(
            url=(self.versions[0] + "/orders"), headers=self.headers)

        self.assertEqual(response.status_code, 200)
        self.assertEqual(type(response.json()), list)

    def test_03_get_orders_v2(self):
        response = self.client.get(
            url=(self.versions[1] + "/orders"), headers=self.headers)

        self.assertEqual(response.status_code, 200)
        self.assertEqual(type(response.json()), dict)

    def test_04_get_orders_by_id(self):
        for version in self.versions:
            response = self.client.get(
                url=(version + "/orders/10"), headers=self.headers)

            self.assertEqual(response.status_code, 200)

    def test_05_get_orders_by_id_items(self):
        for version in self.versions:
            response = self.client.get(
                url=(version + "/orders/10/items"), headers=self.headers)

            self.assertEqual(response.status_code, 200)

    def test_06_get_orders_by_id_shipments(self):
        response = self.client.get(
            url=(self.versions[1] + "/orders/10/shipments"),
            headers=self.headers)

        self.assertIn(response.status_code, [200, 405])

    def test_07_put_order_id(self):
        # Get the last order ID
        for version in self.versions:
            response = self.client.get(
                url=(version + "/orders"), headers=self.headers)
            orders = response.json()
            last_order_id = (
                    next(reversed(orders.values()))[-1]["uid"] if orders else 1
            )
            data = {
                "id": last_order_id,
                "client_id": 1,
                "order_date": "2023-01-01T00:00:00Z",
                "request_date": "2023-01-01T00:00:00Z",
                "shipment_date": "2023-01-01T00:00:00Z",
                "shipment_type": "Standard",
                "shipment_status": "Pending",
                "notes": "Updated order",
                "carrier_code": "UPS",
                "carrier_description": "United Parcel Service",
                "service_code": "Ground",
                "payment_type": "Credit Card",
                "transfer_mode": "Online",
                "total_package_count": 1,
                "total_package_weight": 1.5,
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
                    }
                ]
            }

            response = self.client.put(
                url=(version + f"/orders/{last_order_id}"),
                headers=self.headers, json=data)

            self.assertEqual(response.status_code, 200)

    def test_08_delete_order_id(self):
        # Get the last order ID
        for version in self.versions:
            response = self.client.get(
                url=(version + "/orders"), headers=self.headers)
            orders = response.json()
            last_order_id = (
                    next(reversed(orders.values()))[-1]["uid"] if orders else 1
            )
            response = self.client.delete(
                url=(version + f"/orders/{last_order_id}"),
                headers=self.headers)

            self.assertEqual(response.status_code, 200)
