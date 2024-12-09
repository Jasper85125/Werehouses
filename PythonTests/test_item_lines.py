# import json
import unittest
import httpx
# from models.base import Base


def checkItemLine(item_line):
    required_properties = ["id", "name", "description",
                           "created_at", "updated_at"]
    for prop in required_properties:
        if item_line.get(prop) is None:
            return False
    return True


class TestItemLines(unittest.TestCase):
    def setUp(self):
        self.client = httpx.Client()
        self.url = "http://localhost:5125/api/v2"
        self.headers = httpx.Headers({'API_KEY': 'a1b2c3d4e5'})

    def test_get_item_line_id(self):
        # Send the request
        response = self.client.get(
            url=(self.url + "/itemlines/0"), headers=self.headers
        )
        # Check the status code
        self.assertEqual(response.status_code, 200)

        # Check that the response is a dictionary
        # (representative of a single item line object)
        self.assertEqual(type(response.json()), dict)

        # Check that the item line object has the correct properties
        self.assertTrue(checkItemLine(response.json()))

    def test_get_item_lines(self):
        # Send the request
        response = self.client.get(
            url=(self.url + "/itemlines"),
            headers=self.headers
        )

        # Check the status code
        self.assertEqual(response.status_code, 200)

        # Check that the response is a list
        # (representative of a list of item lines)
        self.assertEqual(type(response.json()), list)

        # If the list contains something, check the first object in the list
        if len(response.json()) > 0:
            # Check that each object in the list is a dictionary
            self.assertEqual(type(response.json()[0]), dict)

            # Check that each item line object has the correct properties
            self.assertTrue(
                all(
                    checkItemLine(item_line)
                    for item_line in response.json()
                )
            )

    # Overwrites an item line based on the given item line id
    def test_put_item_line_id(self):
        data = {
                "id": 5,
                "name": "Tech Gadgets",
                "description": "cool gadgets",
                "created_at": "2022-08-18T07:05:25",
                "updated_at": "2023-05-15T15:44:28"
            }

        # Send the request
        response = self.client.put(
            url=(self.url + "/itemlines/5"),
            headers=self.headers,
            json=data
        )

        # Check the status code
        self.assertEqual(response.status_code, 200)

    # This deletes an item line based on an id
    def test_delete_item_line_id(self):
        # Send the request
        response = self.client.delete(
            url=(self.url + "/itemlines/20"), headers=self.headers
        )

        # Check the status code
        self.assertEqual(response.status_code, 200)


# to run the file: python -m unittest test_item_lines.py
# # git checkout . -f
