# from providers import data_provider
# import unittest
# import os
# import json


# class TestShipments(unittest.TestCase):
#     def setUp(self) -> None:
#         self.shipments = data_provider.Shipments("../data/")

#         self.test_data_path = "./test_data/"
#         os.makedirs(self.test_data_path, exist_ok=True)
#         self.shipments_file = os.path.join(self.test_data_path,
#                                            "shipments.json")

#         self.sample_data = [
#             {"id": 1, "source_id": 1, "order_id": 1, "items": [{
#                 "item_id": "P1",
#                 "amount": 1
#             }]},
#             {"id": 2, "source_id": 2, "order_id": 2, "items": [{
#                 "item_id": "P2",
#                 "amount": 20
#             }]},
#             {"id": 3, "source_id": 1, "order_id": 2, "items": [{
#                 "item_id": "P3",
#                 "amount": 30
#             }]}
#         ]

#         with open(self.shipments_file, "w") as f:
#             json.dump(self.sample_data, f)

#         self.shipments = data_provider.Shipments(self.test_data_path)

#     def tearDown(self) -> None:
#         # Clean up the temporary data path after tests
#         if os.path.exists(self.shipments_file):
#             os.remove(self.shipments_file)
#         if os.path.exists(self.test_data_path):
#             os.rmdir(self.test_data_path)

#     def test_loaded(self):
#         self.assertGreater(len(self.shipments.get_shipments()), 0)

#     def test_get_shipment(self):
#         item = self.shipments.get_shipment(1)
#         self.assertIsNotNone(item)
#         self.assertEqual(item["id"], 1)

#     def test_add_shipment(self):
#         new_shipment = {"id": 4, "source_id": 3, "order_id": 3, "items": [{
#                 "item_id": "P4",
#                 "amount": 10
#             }]}
#         self.shipments.add_shipment(new_shipment)
#         self.assertEqual(len(self.shipments.get_shipments()), 4)
#         self.assertIsNotNone(self.shipments.get_shipment(4))

#     def test_update_shipment(self):
#         original_shipment = self.shipments.get_shipment(1)
#         original_updated_at = original_shipment.get("updated_at")

#         updated_shipment = {"id": 1, "source_id": 3, "order_id": 2,
#                             "updated_at": "new_timestamp"}
#         self.shipments.update_shipment(1, updated_shipment)

#         shipment = self.shipments.get_shipment(1)
#         self.assertNotEqual(shipment["updated_at"], original_updated_at)

#     def test_remove_shipment(self):
#         self.shipments.remove_shipment(1)
#         self.assertIsNone(self.shipments.get_shipment(1))
#         self.assertEqual(len(self.shipments.get_shipments()), 2)

#     def test_get_items_in_shipment(self):
#         items_in_shipment = self.shipments.get_items_in_shipment(1)
#         items = [{"item_id": "P1", "amount": 1}]
#         self.assertEqual(items, items_in_shipment)

#     def test_update_items_in_shipment(self):
#         original_shipment = self.shipments.get_shipment(1)
#         original_items = original_shipment.get("items")

#         updated_shipment = {"id": 1, "source_id": 3, "order_id": 2,
#                             "updated_at": "new_timestamp", "items":
#                             [{"item_id": "P2", "amount": 3}]}
#         self.shipments.update_shipment(1, updated_shipment)

#         shipment = self.shipments.get_shipment(1)
#         self.assertNotEqual(shipment["items"], original_items)
import unittest
import requests


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
        response = requests.put(
            url=(self.url + "/shipments/9999"), headers=self.headers, json=data)
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
