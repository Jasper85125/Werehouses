import unittest
import os
import json
import httpx
# from api.models.items import Items


# class TestItems(unittest.TestCase):
#     def setUp(self) -> None:
#         # Create a temporary data path for testing
#         self.test_data_path = "./test_data/"
#         os.makedirs(self.test_data_path, exist_ok=True)
#         self.items_file = os.path.join(self.test_data_path, "items.json")

#         # Sample data for testing
#         self.sample_data = [
#             {"uid": 1, "item_line": 1, "item_group": 1,
#              "item_type": 1, "supplier_id": 1},
#             {"uid": 2, "item_line": 1, "item_group": 2,
#              "item_type": 1, "supplier_id": 2},
#             {"uid": 3, "item_line": 2, "item_group": 1,
#              "item_type": 2, "supplier_id": 1}
#         ]

#         with open(self.items_file, "w") as f:
#             json.dump(self.sample_data, f)

#         self.items = Items(self.test_data_path)

#     def tearDown(self) -> None:
#         # Clean up the temporary data path after tests
#         if os.path.exists(self.items_file):
#             os.remove(self.items_file)
#         if os.path.exists(self.test_data_path):
#             os.rmdir(self.test_data_path)

#     def test_loaded(self):
#         self.assertGreater(len(self.items.get_items()), 0)

#     def test_get_item(self):
#         item = self.items.get_item(1)
#         self.assertIsNotNone(item)
#         self.assertEqual(item["uid"], 1)

#     def test_get_items_for_item_line(self):
#         items = self.items.get_items_for_item_line(1)
#         self.assertEqual(len(items), 2)

#     def test_get_items_for_item_group(self):
#         items = self.items.get_items_for_item_group(1)
#         self.assertEqual(len(items), 2)

#     def test_get_items_for_item_type(self):
#         items = self.items.get_items_for_item_type(1)
#         self.assertEqual(len(items), 2)

#     def test_get_items_for_supplier(self):
#         items = self.items.get_items_for_supplier(1)
#         self.assertEqual(len(items), 2)

#     def test_add_item(self):
#         new_item = {"uid": 4, "item_line": 3, "item_group": 3,
#                     "item_type": 3, "supplier_id": 3}
#         self.items.add_item(new_item)
#         self.assertEqual(len(self.items.get_items()), 4)
#         self.assertIsNotNone(self.items.get_item(4))

#     def test_update_item(self):
#         original_item = self.items.get_item(1)
#         original_updated_at = original_item.get("updated_at")

#         updated_item = {"uid": 1, "item_line": 1, "item_group": 1,
#                         "item_type": 1, "supplier_id": 1,
#                         "updated_at": "new_timestamp"}
#         self.items.update_item(1, updated_item)

#         item = self.items.get_item(1)
#         self.assertNotEqual(item["updated_at"], original_updated_at)

#     def test_remove_item(self):
#         self.items.remove_item(1)
#         self.assertIsNone(self.items.get_item(1))
#         self.assertEqual(len(self.items.get_items()), 2)


# if __name__ == "__main__":
#     unittest.main()


def checkItem(item):
    required_properties = ["uid", "item_line", "item_group", "item_type",
                           "supplier_id"]
    for prop in required_properties:
        if item.get(prop) is None:
            return False
    return True


class TestClass(unittest.TestCase):
    def setUp(self):
        self.client = httpx.Client()
        self.url = "http://localhost:3000/api/v1"
        self.headers = httpx.Headers({'API_KEY': 'a1b2c3d4e5'})

    def test_02_get_item_id(self):
        # Send the request
        response = self.client.get(
            url=(self.url + "/items/P000006"), headers=self.headers
        )
        # Check the status code
        self.assertEqual(response.status_code, 200)

        # Check that the response is a dictionary
        # (representative of a single item object)
        self.assertEqual(type(response.json()), dict)

        # Check that the item object has the correct properties
        self.assertTrue(checkItem(response.json()))

    def test_03_get_items(self):
        # Send the request
        response = self.client.get(
            url=(self.url + "/items"),
            headers=self.headers
        )

        # Check the status code
        self.assertEqual(response.status_code, 200)

        # Check that the response is a list
        # (representative of a list of items)
        self.assertEqual(type(response.json()), list)

        # If the list contains something, check the first object in the list
        if len(response.json()) > 0:
            # Check that each object in the list is a dictionary
            self.assertEqual(type(response.json()[0]), dict)

            # Check that each item object has the correct properties
            self.assertTrue(
                all(
                    checkItem(item)
                    for item in response.json()
                )
            )

    # This adds a new item object
    def test_04_post_item(self):
        data = {
            "uid": "P000001",
            "item_line": 3,
            "item_group": 3,
            "item_type": 3,
            "supplier_id": 3
        }

        # Send the request
        response = self.client.post(
            url=(self.url + "/items"),
            headers=self.headers,
            json=data
        )

        # Check the status code
        self.assertEqual(response.status_code, 201)

    # Overwrites an item based on the given item-id
    def test_05_put_item_id(self):
        data = {
            "uid": "P000003",
            "item_line": 1,
            "item_group": 1,
            "item_type": 1,
            "supplier_id": 1
        }

        # Send the request
        response = self.client.put(
            url=(self.url + "/items/P000003"),
            headers=self.headers,
            json=data
        )

        # Check the status code
        self.assertEqual(response.status_code, 200)

    # This deletes an item based on an id
    def test_06_delete_item_id(self):
        # Send the request
        response = self.client.delete(
            url=(self.url + "/items/P000001"), headers=self.headers
        )

        # Check the status code
        self.assertEqual(response.status_code, 200)


# to run the file: python -m unittest test_items.py
# # git checkout . -f
