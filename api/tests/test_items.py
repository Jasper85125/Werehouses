import unittest
import os
import json
import httpx


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
    
    def test_get_wrong_path(self):
        response = self.client.get(url=(self.url + "/items/1/error"), headers=self.headers)

        self.assertEqual(response.status_code, 404)

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
