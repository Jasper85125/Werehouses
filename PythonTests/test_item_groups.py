# from models.item_groups import ItemGroups
import unittest
import httpx
# from unittest.mock import patch, mock_open
# import json


# class TestItemGroups(unittest.TestCase):
#     def setUp(self) -> None:
#         self.test_data = json.dumps([{
#             "id": 1,
#             "name": "Sample Group",
#             "description": "This is a sample item group."
#         }])
#         self.itemGroups = ItemGroups(root_path="test_path", is_debug=True)
#         self.sample_item_group = {
#             "id": 1,
#             "name": "Sample Group",
#             "description": "This is a sample item group."
#         }
#         self.itemGroups.add_item_group(self.sample_item_group)

#     @patch("builtins.open", new_callable=mock_open, read_data='[]')
#     def test_loaded(self, mock_file):
#         self.assertGreater(len(self.itemGroups.get_item_groups()), 0)

#     @patch("builtins.open", new_callable=mock_open, read_data='[]')
#     def test_get_item_group(self, mock_file):
#         item_group = self.itemGroups.get_item_group(1)
#         self.assertIsNotNone(item_group)
#         self.assertEqual(item_group["name"], "Sample Group")

#     @patch("builtins.open", new_callable=mock_open, read_data='[]')
#     def test_add_item_group(self, mock_file):
#         new_item_group = {
#             "id": 2,
#             "name": "New Group",
#             "description": "This is a new item group."
#         }
#         self.itemGroups.add_item_group(new_item_group)
#         self.assertEqual(len(self.itemGroups.get_item_groups()), 2)
#         self.assertIsNotNone(self.itemGroups.get_item_group(2))

#     @patch("builtins.open", new_callable=mock_open, read_data='[]')
#     def test_update_item_group(self, mock_file):
#         updated_item_group = {
#             "id": 1,
#             "name": "Updated Group",
#             "description": "This is an updated item group."
#         }
#         self.itemGroups.update_item_group(1, updated_item_group)
#         item_group = self.itemGroups.get_item_group(1)
#         self.assertEqual(item_group["name"], "Updated Group")


# if __name__ == "__main__":
#     unittest.main()

def checkItemGroup(item_group):
    required_properties = ["id", "name", "description"]
    for prop in required_properties:
        if item_group.get(prop) is None:
            return False
    return True


class TestItemGroups(unittest.TestCase):
    def setUp(self):
        self.client = httpx.Client()
        self.url = "http://localhost:3000/api/v1"
        self.headers = httpx.Headers({'API_KEY': 'a1b2c3d4e5'})

    def test_02_get_item_group_id(self):
        # Send the request
        response = self.client.get(
            url=(self.url + "/item_groups/1"), headers=self.headers
        )
        # Check the status code
        self.assertEqual(response.status_code, 200)

        # Check that the response is a dictionary
        # (representative of a single item group object)
        self.assertEqual(type(response.json()), dict)

        # Check that the item group object has the correct properties
        self.assertTrue(checkItemGroup(response.json()))

    def test_03_get_item_groups(self):
        # Send the request
        response = self.client.get(
            url=(self.url + "/item_groups"),
            headers=self.headers
        )

        # Check the status code
        self.assertEqual(response.status_code, 200)

        # Check that the response is a list
        # (representative of a list of item groups)
        self.assertEqual(type(response.json()), list)

        # If the list contains something, check the first object in the list
        if len(response.json()) > 0:
            # Check that each object in the list is a dictionary
            self.assertEqual(type(response.json()[0]), dict)

            # Check that each item group object has the correct properties
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

        # Send the request
        response = self.client.put(
            url=(self.url + "/item_groups/1"),
            headers=self.headers,
            json=data
        )

        # Check the status code
        self.assertEqual(response.status_code, 200)

    # This deletes an item group based on an id
    def test_06_delete_item_group_id(self):
        # Send the request
        response = self.client.delete(
            url=(self.url + "/item_groups/1"), headers=self.headers
        )

        # Check the status code
        self.assertEqual(response.status_code, 200)


# to run the file: python -m unittest test_item_groups.py
# # git checkout . -f
