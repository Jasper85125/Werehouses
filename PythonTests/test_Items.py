import unittest
import httpx


class TestClass(unittest.TestCase):
    def setUp(self):
        self.client = httpx.Client()
        self.versions = ["http://localhost:5001/api/v1",
                         "http://localhost:5002/api/v2"]
        self.headers = httpx.Headers({'Api-Key': 'AdminKey'})

    def test_01_post_item(self):
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

    def test_02_get_items(self):
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

    def test_04_get_item_id_v1(self):
        for url in ["http://localhost:5001/api/v1"]:
            with self.subTest(url=url):
                response = self.client.get(
                    url=(url + "/items"), headers=self.headers)
                self.assertEqual(
                    response.status_code, 200,
                    msg=f"Failed to get items: {response.content}"
                )
                items = response.json()
                last_item_id = items[-1]["uid"] if items else 1

                response = self.client.get(
                    url=(url + f"/items/{last_item_id}"),
                    headers=self.headers
                )

                self.assertEqual(response.status_code, 200)
                self.assertEqual(type(response.json()), dict)

    def test_04_get_item_id_v2(self):
        for url in ["http://localhost:5002/api/v2"]:
            with self.subTest(url=url):
                response = self.client.get(
                    url=(url + "/items?page=0"), headers=self.headers)
                self.assertEqual(
                    response.status_code, 200,
                    msg=f"Failed to get items: {response.content}"
                )
                items = response.json()
                last_item_id = (
                    next(reversed(items.values()))[-1]["uid"] if items else 1
                )
                print(last_item_id)

                response = self.client.get(
                    url=(url + f"/items/{last_item_id}"),
                    headers=self.headers
                )

                self.assertEqual(response.status_code, 200)
                self.assertEqual(type(response.json()), dict)

    def test_05_put_item_id_v1(self):
        data = {
            "uid": "P000003",
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
        for url in ["http://localhost:5001/api/v1"]:
            with self.subTest(url=url):
                response = self.client.get(
                    url=(url + "/items"), headers=self.headers)
                self.assertEqual(
                    response.status_code, 200,
                    msg=f"Failed to get items: {response.content}"
                )
                items = response.json()
                last_item_id = items[-1]["uid"] if items else 1

                response = self.client.put(
                    url=(url + f"/items/{last_item_id}"),
                    headers=self.headers, json=data
                )

                self.assertEqual(response.status_code, 200)

    def test_05_put_item_id_v2(self):
        data = {
            "uid": "P000003",
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
        for url in ["http://localhost:5002/api/v2"]:
            with self.subTest(url=url):
                response = self.client.get(
                    url=(url + "/items?page=0"), headers=self.headers)
                self.assertEqual(
                    response.status_code, 200,
                    msg=f"Failed to get items: {response.content}"
                )
                items = response.json()
                last_item_id = (
                    next(reversed(items.values()))[-1]["uid"] if items else 1
                )
                response = self.client.put(
                    url=(url + f"/items/{last_item_id}"),
                    headers=self.headers, json=data
                )

                self.assertEqual(response.status_code, 200)

    def test_06_delete_item_id_v1(self):
        for url in ["http://localhost:5001/api/v1"]:
            with self.subTest(url=url):
                response = self.client.get(
                    url=(url + "/items"), headers=self.headers)
                self.assertEqual(
                    response.status_code, 200,
                    msg=f"Failed to get items: {response.content}"
                )
                items = response.json()
                last_item_id = items[-1]["uid"] if items else 1

                response = self.client.delete(
                    url=(url + f"/items/{last_item_id}"),
                    headers=self.headers
                )
                self.assertEqual(response.status_code, 200)

    def test_06_delete_item_id_v2(self):
        for url in ["http://localhost:5002/api/v2"]:
            with self.subTest(url=url):
                response = self.client.get(
                    url=(url + "/items?page=0"), headers=self.headers)
                self.assertEqual(
                    response.status_code, 200,
                    msg=f"Failed to get items: {response.content}"
                )
                items = response.json()
                last_item_id = (
                    next(reversed(items.values()))[-1]["uid"] if items else 1
                )
                response = self.client.delete(
                    url=(url + f"/items/{last_item_id}"),
                    headers=self.headers
                )
                self.assertEqual(response.status_code, 200)

# to run the file: python -m unittest test_items.py
