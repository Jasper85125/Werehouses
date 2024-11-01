# from providers import data_provider
import unittest
import httpx
# import os
# import json


# class TestOrders(unittest.TestCase):
#     def setUp(self) -> None:
#         self.orders = data_provider.Orders("../data/")

#         self.test_data_path = "./test_data/"
#         os.makedirs(self.test_data_path, exist_ok=True)
#         self.orders_file = os.path.join(self.test_data_path, "orders.json")

#         # Sample data for testing
#         self.sample_data = [
#             {"id": 1, "source_id": 1, "warehouse_id": 1,
#              "shipment_id": 1, "items":
#              [{"item_id": "P0074351", "amount": 1}],
#              "ship_to": 1},
#             {"id": 2, "source_id": 2, "warehouse_id": 2,
#              "shipment_id": 1, "items":
#              [{"item_id": "P000002", "amount": 20}],
#              "ship_to": 1},
#             {"id": 3, "source_id": 1, "warehouse_id": 2,
#              "shipment_id": 2, "items":
#              [{"item_id": "P000003", "amount": 30}],
#              "bill_to": 1}
#         ]

#         with open(self.orders_file, "w") as f:
#             json.dump(self.sample_data, f)

#         self.orders = data_provider.Orders(self.test_data_path)

#     def tearDown(self) -> None:
#         # Clean up the temporary data path after tests
#         if os.path.exists(self.orders_file):
#             os.remove(self.orders_file)
#         if os.path.exists(self.test_data_path):
#             os.rmdir(self.test_data_path)

#     def test_loaded(self):
#         self.assertGreater(len(self.orders.get_orders()), 0)

#     def test_get_order(self):
#         item = self.orders.get_order(1)
#         self.assertIsNotNone(item)
#         self.assertEqual(item["id"], 1)

#     def test_add_order(self):
#         new_order = {"id": 4, "source_id": 3, "warehouse_id": 2,
#                      "shipment_id": 1, "items":
#                      [{"item_id": "P000001", "amount": 1}],
#                      "ship_to": 1}
#         self.orders.add_order(new_order)
#         self.assertEqual(len(self.orders.get_orders()), 4)
#         self.assertIsNotNone(self.orders.get_order(4))

#     def test_update_order(self):
#         original_order = self.orders.get_order(1)
#         original_updated_at = original_order.get("updated_at")

#         updated_order = {"id": 1, "source_id": 3, "warehouse_id": 2,
#                          "shipment_id": 1, "updated_at": "new_timestamp"}
#         self.orders.update_order(1, updated_order)

#         order = self.orders.get_order(1)
#         self.assertNotEqual(order["updated_at"], original_updated_at)

#     def test_remove_order(self):
#         self.orders.remove_order(1)
#         self.assertIsNone(self.orders.get_order(1))
#         self.assertEqual(len(self.orders.get_orders()), 2)

#     def test_get_items_in_order(self):
#         items_in_order = self.orders.get_items_in_order(1)
#         items = [{"item_id": "P0074351", "amount": 1}]
#         self.assertEqual(items, items_in_order)

#     def test_get_orders_in_shipment(self):
#         orders_in_shipment = self.orders.get_orders_in_shipment(1)
#         self.assertEqual(len(orders_in_shipment), 2)

#     def Test_order_to_client(self):
#         orders_for_client = self.orders.get_orders_for_client(1)
#         self.assertEqual(len(orders_for_client), 3)


# if __name__ == "__main__":
#     unittest.main()

def checkOrder(order):
    required_properties = ["id", "source_id", "warehouse_id",
                           "shipment_id", "items", "ship_to"]
    for prop in required_properties:
        if order.get(prop) is None:
            return False
    return True


class TestOrdersAPI(unittest.TestCase):
    def setUp(self):
        self.client = httpx.Client()
        self.url = "http://localhost:3000/api/v1"
        self.headers = httpx.Headers({'API_KEY': 'a1b2c3d4e5'})

    def test_02_get_order_id(self):
        # Send the request
        response = self.client.get(
            url=(self.url + "/orders/5"), headers=self.headers
        )
        # Check the status code
        self.assertEqual(response.status_code, 200)

        # Check that the response is a dictionary
        # (representative of a single order object)
        self.assertEqual(type(response.json()), dict)

        # Check that the order object has the correct properties
        self.assertTrue(checkOrder(response.json()))

    def test_03_get_orders(self):
        # Send the request
        response = self.client.get(
            url=(self.url + "/orders"),
            headers=self.headers
        )

        # Check the status code
        self.assertEqual(response.status_code, 200)

        # Check that the response is a list
        # (representative of a list of orders)
        self.assertEqual(type(response.json()), list)

        # If the list contains something, check the first object in the list
        if len(response.json()) > 0:
            # Check that each object in the list is a dictionary
            self.assertEqual(type(response.json()[0]), dict)

            # Check that each order object has the correct properties
            self.assertTrue(
                all(
                    checkOrder(order)
                    for order in response.json()
                )
            )

    # This adds a new order object
    def test_04_post_order(self):
        data = {
            "id": 4,
            "source_id": 3,
            "warehouse_id": 2,
            "shipment_id": 1,
            "items": [{"item_id": "P000001", "amount": 1}],
            "ship_to": 1
        }

        # Send the request
        response = self.client.post(
            url=(self.url + "/orders"),
            headers=self.headers,
            json=data
        )

        # Check the status code
        self.assertEqual(response.status_code, 201)

    # Overwrites an order based on the given order-id
    def test_05_put_order_id(self):
        data = {
            "id": 1,
            "source_id": 3,
            "warehouse_id": 2,
            "shipment_id": 1,
            "items": [{"item_id": "P000001", "amount": 1}],
            "ship_to": 1
        }

        # Send the request
        response = self.client.put(
            url=(self.url + "/orders/1"),
            headers=self.headers,
            json=data
        )

        # Check the status code
        self.assertEqual(response.status_code, 200)

    # This deletes an order based on an id
    def test_06_delete_order_id(self):
        # Send the request
        response = self.client.delete(
            url=(self.url + "/orders/7"), headers=self.headers
        )

        # Check the status code
        self.assertEqual(response.status_code, 200)


# to run the file: python -m unittest test_orders.py
# # git checkout . -f
