import unittest
import httpx


class TestClass(unittest.TestCase):
    def setUp(self):
        self.client = httpx.Client()
        self.versions = ["http://localhost:5001/api/v1",
                         "http://localhost:5002/api/v2"]
        self.headers = httpx.Headers({'Api-Key': 'AdminKey'})

    def test_04_post_item(self):
        data = {
            "uid": "",
            "item_line": 3,
            "item_group": 3,
            "item_type": 3,
            "supplier_id": 3,
            "name": "Test Item",
            "description": "This is a test item",
            "code": "ITEM001",
            "upc_code": "123456789012",
            "model_number": "MODEL001",
            "commodity_code": "COMM001",
            "short_description": "Short description"
        }
        for url in self.versions:
            with self.subTest(url=url):
                response = self.client.post(
                    url=(url + "/items"), headers=self.headers, json=data
                )
                self.assertEqual(
                    response.status_code, 201,
                    msg=f"Failed to create item: {response.content}"
                )

    def test_03_get_items(self):
        for url in ["http://localhost:5001/api/v1"]:
            with self.subTest(url=url):
                response = self.client.get(
                    url=(url + "/items"), headers=self.headers
                )
                self.assertEqual(response.status_code, 200)
                self.assertEqual(type(response.json()), list)

    def test_03_get_items_v2(self):
        for url in ["http://localhost:5002/api/v2"]:
            with self.subTest(url=url):
                response = self.client.get(
                    url=(url + "/items"), headers=self.headers
                )
                self.assertEqual(response.status_code, 200)
                self.assertEqual(type(response.json()), dict)

    def test_02_get_item_id(self):
        for url in self.versions:
            with self.subTest(url=url):
                response = self.client.get(
                    url=(url + "/items"), headers=self.headers
                )
                self.assertEqual(response.status_code, 200)
                items = response.json()
                if items:
                    response = self.client.get(
                        url=(url + "/items/P000001"),
                        headers=self.headers
                    )
                    self.assertEqual(response.status_code, 200)
                    self.assertEqual(type(response.json()), dict)
                else:
                    self.fail("No items found in the response")

    def test_05_put_item_id(self):
        data = {
            "uid": "P000001",
            "item_line": 1,
            "item_group": 1,
            "item_type": 1,
            "supplier_id": 1,
            "name": "Updated Test Item",
            "description": "This is an updated test item",
            "code": "ITEM003",
            "upc_code": "123456789013",
            "model_number": "MODEL003",
            "commodity_code": "COMM003",
            "short_description": "Updated short description"
        }
        for url in self.versions:
            with self.subTest(url=url):
                response = self.client.get(
                    url=(url + "/items"), headers=self.headers
                )
                self.assertEqual(response.status_code, 200)
                items = response.json()
                if items:
                    last_item_uid = items[-1]['uid']
                    response = self.client.put(
                        url=(url + f"/items/{last_item_uid}"),
                        headers=self.headers, json=data
                    )
                    self.assertEqual(response.status_code, 200)
                else:
                    self.fail("No items found in the response")

    def test_06_delete_item_id(self):
        for url in self.versions:
            with self.subTest(url=url):
                response = self.client.get(
                    url=(url + "/items"), headers=self.headers
                )
                self.assertEqual(response.status_code, 200)
                items = response.json()
                if items:
                    last_item_uid = items[-1]['uid']
                    response = self.client.delete(
                        url=(url + f"/items/{last_item_uid}"),
                        headers=self.headers
                    )
                    self.assertEqual(response.status_code, 200)
                else:
                    self.fail("No items found in the response")


# to run the file: python -m unittest test_items.py
