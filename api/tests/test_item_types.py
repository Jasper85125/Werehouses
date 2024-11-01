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
    
    def test_get_items_by_item_type_id(self):
        response = self.client.get(url=(self.url + "/item_types/1/items"), headers=self.headers)

        self.assertEqual(response.status_code, 200)
    
    def test_get_wrong_path(self):
        response = self.client.get(url=(self.url + "/item_types/1/error"), headers=self.headers)

        self.assertEqual(response.status_code, 404)


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
