import unittest
import httpx


def checkItemType(item_type):
    required_properties = ["id", "name", "description",
                           "created_at", "updated_at"]
    for prop in required_properties:
        if item_type.get(prop) is None:
            return False
    return True


class TestItemTypesAPI(unittest.TestCase):
    def setUp(self):
        self.client = httpx.Client()
        self.headers = httpx.Headers({'API_KEY': 'a1b2c3d4e5'})
        self.versions = ["http://localhost:5001/api/v1",
                         "http://localhost:5002/api/v2"]

    def test_02_get_item_type_id(self):
        for version in self.versions:
            with self.subTest(version=version):
                # Send the request
                response = self.client.get(
                    url=(version + "/itemtypes/8"), headers=self.headers
                )
                # Check the status code
                self.assertEqual(response.status_code, 200)

                # Check that the response is a dictionary
                # (representative of a single item type object)
                self.assertEqual(type(response.json()), dict)

                # Check that the item type object has the correct properties
                self.assertTrue(checkItemType(response.json()))

    def test_03_get_item_types(self):
        for version in self.versions:
            with self.subTest(version=version):
                # Send the request
                response = self.client.get(
                    url=(version + "/itemtypes"), headers=self.headers
                )

                # Check the status code
                self.assertEqual(response.status_code, 200)

                # Check that the response is a list
                # (representative of a list of item types)
                self.assertEqual(type(response.json()), list)

                # If the list contains something, check the
                # first object in the list
                if len(response.json()) > 0:
                    # Check that each object in the list is a dictionary
                    self.assertEqual(type(response.json()[0]), dict)

                    # Check that each item type object has
                    # the correct properties
                    self.assertTrue(
                        all(
                            checkItemType(item_type)
                            for item_type in response.json()
                        )
                    )

    def test_05_put_item_type_id(self):
        for version in self.versions:
            with self.subTest(version=version):
                data = {
                    "id": 4,
                    "name": "Updated Item Type",
                    "description": "An updated item type",
                    "created_at": "2023-10-01T00:00:00",
                    "updated_at": "2023-10-01T00:00:00",
                }

                # Send the request
                response = self.client.put(
                    url=(version + "/itemtypes/4"),
                    headers=self.headers,
                    json=data
                )

                # Check the status code
                self.assertEqual(response.status_code, 200)

    def test_06_delete_item_type_id(self):
        for version in self.versions:
            with self.subTest(version=version):
                # Send the request
                response = self.client.delete(
                    url=(version + "/itemtypes/3"), headers=self.headers
                )

                # Check the status code
                self.assertEqual(response.status_code, 200)


# to run the file: python -m unittest test_item_types.py
