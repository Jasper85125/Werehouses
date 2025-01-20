import unittest
import httpx


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
        self.versions = ["http://localhost:5001/api/v1",
                         "http://localhost:5002/api/v2"]
        self.headers = httpx.Headers({'Api-Key': 'AdminKey'})

    def test_01_post_item_line(self):
        for version in self.versions:
            with self.subTest(version=version):
                data = {
                    "name": "Tech Gadgets",
                    "description": "cooler gadgets",
                    "created_at": "2022-08-18T07:05:25"
                }

                response = self.client.post(
                    url=(version + "/itemlines"),
                    headers=self.headers,
                    json=data
                )

                # Check the status code
                self.assertEqual(response.status_code, 201)

    def test_02_get_item_line_id(self):
        for version in self.versions:
            with self.subTest(version=version):
                response = self.client.get(
                    url=(version + "/itemlines/0"), headers=self.headers
                )
                # Check the status code
                self.assertEqual(response.status_code, 200)

                # Check that the response is a dictionary
                self.assertEqual(type(response.json()), dict)

                # Check that the item line object has the correct properties
                self.assertTrue(checkItemLine(response.json()))

    def test_03_get_item_lines(self):
        for version in self.versions:
            with self.subTest(version=version):
                # Send the request
                response = self.client.get(
                    url=(version + "/itemlines"),
                    headers=self.headers
                )

                # Check the status code
                self.assertEqual(response.status_code, 200)

                # Check that the response is a list
                self.assertEqual(type(response.json()), list)

                # If the list contains something,
                if len(response.json()) > 0:
                    # Check that each object in the list is a dictionary
                    self.assertEqual(type(response.json()[0]), dict)

                    # Check that each item line object has the
                    # correct properties
                    self.assertTrue(
                        all(
                            checkItemLine(item_line)
                            for item_line in response.json()
                        )
                    )

    # Overwrites an item line based on the given item line id
    def test_04_put_item_line_id(self):
        for version in self.versions:
            with self.subTest(version=version):

                response = self.client.get(
                    url=(version + "/itemlines"),
                    headers=self.headers
                )
                items_line = response.json()
                last_items_line_id = items_line[-1]['id']

                data = {
                    "id": last_items_line_id,
                    "name": "Tech Gadgets",
                    "description": "cool gadgets",
                    "created_at": "2022-08-18T07:05:25",
                    "updated_at": "2023-05-15T15:44:28"
                }

                # Send the request to update the last item line
                response = self.client.put(
                    url=(version + f"/itemlines/{last_items_line_id}"),
                    headers=self.headers,
                    json=data
                )

                # Check the status code
                self.assertEqual(response.status_code, 200)

    # This deletes an item line based on an id
    def test_05_delete_item_line_id(self):
        for version in self.versions:
            with self.subTest(version=version):
                response = self.client.get(
                    url=(version + "/itemlines"),
                    headers=self.headers
                )
                items_line = response.json()
                last_items_line_id = items_line[-1]['id']
                print(last_items_line_id)

                # Send the request to delete the last item line
                response = self.client.delete(
                    url=(version + f"/itemlines/{last_items_line_id}"),
                    headers=self.headers
                )

                # Check the status code
                self.assertEqual(response.status_code, 200)

    def test_06_create_in_v1_get_and_delete_in_v2(self):
        # Create in v1
        data = {
            "name": "Tech Gadgets",
            "description": "cooler gadgets",
        }
        response = self.client.post(
            url="http://localhost:5001/api/v1/itemlines",
            headers=self.headers,
            json=data
        )
        self.assertEqual(response.status_code, 201)
        created_item_line = response.json()
        created_item_line_id = created_item_line['id']

        # Get in v2
        response = self.client.get(
            url=(
                (
                    f"http://localhost:5002/api/v2/itemlines/"
                    f"{created_item_line_id}"
                )
            ),
            headers=self.headers
        )
        self.assertEqual(response.status_code, 200)
        self.assertEqual(type(response.json()), dict)
        self.assertTrue(checkItemLine(response.json()))

        self.assertEqual(response.json()['name'], data['name'])
        self.assertEqual(response.json()['description'], data['description'])

        # Delete in v2
        response = self.client.delete(
            url=(
                (
                    f"http://localhost:5002/api/v2/itemlines/"
                    f"{created_item_line_id}"
                )
            ),
            headers=self.headers
        )
        self.assertEqual(response.status_code, 200)

# to run the file: python -m unittest test_item_lines.py
