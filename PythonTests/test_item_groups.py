import unittest
import httpx


def checkItemGroup(item_group):
    required_properties = ["id", "name", "description"]
    for prop in required_properties:
        if item_group.get(prop) is None:
            return False
    return True


class TestItemGroups(unittest.TestCase):
    def setUp(self):
        self.client = httpx.Client()
        self.versions = ["http://localhost:5001/api/v1",
                         "http://localhost:5002/api/v2"]
        self.headers = httpx.Headers({'API_KEY': 'a1b2c3d4e5'})

    def test_01_post_item_groups(self):
        data = {
            "name": "Test Group",
            "description": "This is a test item group."
        }

        for version in self.versions:
            with self.subTest(version=version):
                # Send the request
                response = self.client.post(
                    url=(version + "/itemgroups"),
                    headers=self.headers,
                    json=data
                )

                # Check the status code
                self.assertEqual(response.status_code, 201)

    def test_02_get_item_group_id(self):
        for version in self.versions:
            with self.subTest(version=version):
                # Send the request
                response = self.client.get(
                    url=(version + "/itemgroups/1"), headers=self.headers
                )
                # Check the status code
                self.assertEqual(response.status_code, 200)

                # Check that the response is a dictionary
                # (representative of a single item group object)
                self.assertEqual(type(response.json()), dict)

                # Check that the item group object has the correct properties
                self.assertTrue(checkItemGroup(response.json()))

    def test_03_get_item_groups(self):
        for version in self.versions:
            with self.subTest(version=version):
                # Send the request
                response = self.client.get(
                    url=(version + "/itemgroups"),
                    headers=self.headers
                )

                # Check the status code
                self.assertEqual(response.status_code, 200)

                # Check that the response is a list
                # (representative of a list of item groups)
                self.assertEqual(type(response.json()), list)

                # If the list contains something, check the
                # first object in the list
                if len(response.json()) > 0:
                    # Check that each object in the list is a dictionary
                    self.assertEqual(type(response.json()[0]), dict)

                    # Check that each item group object has
                    # the correct properties
                    self.assertTrue(
                        all(
                            checkItemGroup(item_group)
                            for item_group in response.json()
                        )
                    )

    # Overwrites an item group based on the given item group-id
    def test_05_put_item_group_id(self):
        data = {
            "id": 1,
            "name": "Updated Group",
            "description": "This is an updated item group."
        }

        for version in self.versions:
            with self.subTest(version=version):
                # Send the request
                response = self.client.put(
                    url=(version + "/itemgroups/1"),
                    headers=self.headers,
                    json=data
                )

                # Check the status code
                self.assertEqual(response.status_code, 200)

    # This deletes an item group based on an id
    def test_06_delete_item_group_id(self):
        for version in self.versions:
            with self.subTest(version=version):
                # Send the request
                response = self.client.delete(
                    url=(version + "/itemgroups/19"), headers=self.headers
                )

                # Check the status code
                self.assertEqual(response.status_code, 200)


# to run the file: python -m unittest test_item_groups.py
# # git checkout . -f
