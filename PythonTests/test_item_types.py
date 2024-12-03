import unittest
# from unittest.mock import patch, mock_open
# from models.item_types import ItemTypes
import httpx


# class TestItemTypes(unittest.TestCase):
#     def setUp(self) -> None:
#         self.mock_data = [
#             {"id": 0, "name": "Laptop", "description": "",
#              "created_at": "2001-11-02 23:02:40",
#              "updated_at": "2008-07-01 04:09:17"},
#             {"id": 1, "name": "Desktop", "description": "",
#              "created_at": "1993-07-28 13:43:32",
#              "updated_at": "2022-05-12 08:54:35"}
#         ]
#         self.itemTypes = ItemTypes("../data/", is_debug=True)
#         self.itemTypes.data = self.mock_data

#     def test_loaded(self):
#         self.assertGreater(len(self.itemTypes.get_item_types()), 0)

#     def test_add_item_type(self):
#         new_item = {"id": 2, "name": "Tablet", "description": ""}
#         self.itemTypes.add_item_type(new_item)
#         self.assertEqual(len(self.itemTypes.get_item_types()), 3)
#         self.assertIsNotNone(self.itemTypes.get_item_type(2))

#     def test_update_item_type(self):
#         updated_item = {"id": 0, "name": "Gaming Laptop", "description": ""}
#         self.itemTypes.update_item_type(0, updated_item)
#         self.assertEqual(self.itemTypes.get_item_type(0)["name"],
#                          "Gaming Laptop")

#     def test_remove_item_type(self):
#         self.itemTypes.remove_item_type(1)
#         self.assertIsNone(self.itemTypes.get_item_type(1))
#         self.assertEqual(len(self.itemTypes.get_item_types()), 1)

#     @patch(
#         "builtins.open",
#         new_callable=mock_open,
#         read_data='[{"id": 0, "name": "Laptop"}]'
#     )
#     def test_load(self, mock_file):
#         self.itemTypes.load(is_debug=False)
#         self.assertEqual(len(self.itemTypes.get_item_types()), 1)
#         self.assertEqual(self.itemTypes.get_item_type(0)["name"], "Laptop")

#     @patch("builtins.open", new_callable=mock_open)
#     def test_save(self, mock_file):
#         self.itemTypes.save()
#         handle = mock_file()
#         written_data = ''.join(call.args[0] for call
#                                in handle.write.call_args_list)
#         expected_data = (
#             (
#                 '[{"id": 0, "name": "Laptop", "description": "", '
#                 '"created_at": "2001-11-02 23:02:40", '
#                 '"updated_at": "2008-07-01 04:09:17"}, '
#                 '{"id": 1, "name": "Desktop", "description": "", '
#                 '"created_at": "1993-07-28 13:43:32", '
#                 '"updated_at": "2022-05-12 08:54:35"}]'
#             )
#         )
#         self.assertEqual(written_data, expected_data)


# if __name__ == "__main__":
#     unittest.main()

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
        self.url = "http://localhost:3000/api/v1"
        self.headers = httpx.Headers({'API_KEY': 'a1b2c3d4e5'})

    def test_02_get_item_type_id(self):
        # Send the request
        response = self.client.get(
            url=(self.url + "/item_types/8"), headers=self.headers
        )
        # Check the status code
        self.assertEqual(response.status_code, 200)

        # Check that the response is a dictionary
        # (representative of a single item type object)
        self.assertEqual(type(response.json()), dict)

        # Check that the item type object has the correct properties
        self.assertTrue(checkItemType(response.json()))

    def test_03_get_item_types(self):
        # Send the request
        response = self.client.get(
            url=(self.url + "/item_types"), headers=self.headers
        )

        # Check the status code
        self.assertEqual(response.status_code, 200)

        # Check that the response is a list
        # (representative of a list of item types)
        self.assertEqual(type(response.json()), list)

        # If the list contains something, check the first object in the list
        if len(response.json()) > 0:
            # Check that each object in the list is a dictionary
            self.assertEqual(type(response.json()[0]), dict)

            # Check that each item type object has the correct properties
            self.assertTrue(
                all(
                    checkItemType(item_type)
                    for item_type in response.json()
                )
            )
    # Updates an item type based on the given item type id

    def test_05_put_item_type_id(self):
        data = {
            "id": 4,
            "name": "Updated Item Type",
            "description": "An updated item type",
            "created_at": "2023-10-01 00:00:00",
            "updated_at": "2023-10-01 00:00:00",
        }

        # Send the request
        response = self.client.put(
            url=(self.url + "/item_types/4"),
            headers=self.headers,
            json=data
        )

        # Check the status code
        self.assertEqual(response.status_code, 200)

    # This deletes an item type based on an id
    def test_06_delete_item_type_id(self):
        # Send the request
        response = self.client.delete(
            url=(self.url + "/item_types/3"), headers=self.headers
        )

        # Check the status code
        self.assertEqual(response.status_code, 200)


# to run the file: python -m unittest test_item_types.py
