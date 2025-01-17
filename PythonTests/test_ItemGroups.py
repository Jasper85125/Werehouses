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
        self.headers = httpx.Headers({'Api-Key': 'AdminKey'})

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
        for version in self.versions:
            with self.subTest(version=version):
                # Send the request
                # Get the list of item groups
                get_response = self.client.get(
                    url=(version + "/itemgroups"),
                    headers=self.headers
                )
                self.assertEqual(get_response.status_code, 200)
                item_groups = get_response.json()

                # Get the id of the last item group
                if item_groups:
                    last_item_group_id = item_groups[-1]["id"]

                    data = {
                        "id": last_item_group_id,
                        "name": "Updated Group",
                        "description": "This is an updated item group."
                    }

                    # Send the request to update the last item group
                    response = self.client.put(
                        url=(version + f"/itemgroups/{last_item_group_id}"),
                        headers=self.headers,
                        json=data
                    )
                else:
                    self.fail("No item groups available to update.")

                # Check the status code
                self.assertEqual(response.status_code, 200)

    # This deletes an item group based on an id
    def test_06_delete_item_group_id(self):
        for version in self.versions:
            with self.subTest(version=version):
                # Get the list of item groups
                get_response = self.client.get(
                    url=(version + "/itemgroups"),
                    headers=self.headers
                )
                self.assertEqual(get_response.status_code, 200)
                item_groups = get_response.json()

                # Get the id of the last item group
                if item_groups:
                    last_item_group_id = item_groups[-1]["id"]

                    # Send the request to delete the last item group
                    response = self.client.delete(
                        url=(version + f"/itemgroups/{last_item_group_id}"),
                        headers=self.headers
                    )
                else:
                    self.fail("No item groups available to delete.")

                # Check the status code
                self.assertEqual(response.status_code, 200)

    def test_07_create_in_v1_get_and_delete_in_v2(self):
        # Create in v1
        data = {
            "name": "Cross Version Group",
            "description": "Item group is created in v1 and accessed in v2."
        }
        create_response = self.client.post(
            url="http://localhost:5001/api/v1/itemgroups",
            headers=self.headers,
            json=data
        )
        self.assertEqual(create_response.status_code, 201)
        created_item_group = create_response.json()
        created_item_group_id = created_item_group["id"]

        # Get in v2
        get_response = self.client.get(
            url=(
                f"http://localhost:5002/api/v2/itemgroups/"
                f"{created_item_group_id}"
            ),
            headers=self.headers
        )
        self.assertEqual(get_response.status_code, 200)
        self.assertEqual(type(get_response.json()), dict)
        self.assertTrue(checkItemGroup(get_response.json()))

        self.assertEqual(get_response.json()["name"], data["name"])
        self.assertEqual(get_response.json()["description"],
                         data["description"])

        # Delete in v2
        delete_response = self.client.delete(
            url=(
                f"http://localhost:5002/api/v2/itemgroups/"
                f"{created_item_group_id}"
            ),
            headers=self.headers
        )
        self.assertEqual(delete_response.status_code, 200)

# to run the file: python -m unittest test_item_groups.py
# # git checkout . -f
