import unittest
import httpx


def checkSupplier(supplier):
    required_properties = ["id", "code", "name", "address", "province",
                           "country", "contact_name", "reference"]
    for prop in required_properties:
        if supplier.get(prop) is None:
            return False
    return True


class TestClass(unittest.TestCase):
    def setUp(self):
        self.client = httpx.Client()
        self.headers = {'Api-Key': 'AdminKey'}
        self.versions = ["http://localhost:5001/api/v1",
                         "http://localhost:5002/api/v2"]

    def test_01_post_suppliers(self):
        data = {
            "code": "12345",
            "name": "Test Supplier",
            "address": "123 Test St",
            "province": "Test Province",
            "country": "Test Country",
            "contact_name": "Test Contact",
            "reference": "Test Reference"
        }
        for version in self.versions:
            with self.subTest(version=version):
                response = self.client.post(
                    url=(version + "/suppliers"),
                    headers=self.headers, json=data
                )
                self.assertEqual(response.status_code, 201)

    def test_02_get_suppliers(self):
        for version in self.versions:
            with self.subTest(version=version):
                response = self.client.get(
                    url=(version + "/suppliers"), headers=self.headers
                )
                self.assertEqual(response.status_code, 200)

    def test_03_get_supplier_id(self):
        for version in self.versions:
            with self.subTest(version=version):
                response = self.client.get(
                    url=(version + "/suppliers"), headers=self.headers)
                self.assertEqual(
                    response.status_code, 200,
                    msg=f"Failed to get clients: {response.content}"
                )
                clients = response.json()
                last_supplier_id = clients[-1]["id"] if clients else 1
                response = self.client.get(
                    url=(version + f"/suppliers/{last_supplier_id}"),
                    headers=self.headers
                )
                self.assertEqual(response.status_code, 200)

    def test_04_get_supplier_id_items(self):
        for version in self.versions:
            with self.subTest(version=version):
                response = self.client.get(
                    url=(version + "/suppliers"), headers=self.headers)
                self.assertEqual(
                    response.status_code, 200,
                    msg=f"Failed to get clients: {response.content}"
                )
                clients = response.json()
                last_supplier_id = clients[-1]["id"] if clients else 1
                response = self.client.get(
                    url=(version + f"/suppliers/{last_supplier_id}/items"),
                    headers=self.headers
                )
                self.assertEqual(response.status_code, 200)

    def test_05_put_supplier_id(self):
        data = {
            "code": "123456",
            "name": "Test Supplier",
            "address": "123 Test St",
            "province": "Test Province",
            "country": "Test Country",
            "contact_name": "Test Contact",
            "reference": "Test Reference"
        }
        for version in self.versions:
            with self.subTest(version=version):
                response = self.client.get(
                    url=(version + "/suppliers"), headers=self.headers)
                self.assertEqual(
                    response.status_code, 200,
                    msg=f"Failed to get clients: {response.content}"
                )
                clients = response.json()
                last_supplier_id = clients[-1]["id"] if clients else 1
                response = self.client.put(
                    url=(version + f"/suppliers/{last_supplier_id}"),
                    headers=self.headers, json=data
                )
                self.assertEqual(response.status_code, 200)

    def test_06_delete_supplier_id(self):
        for version in self.versions:
            with self.subTest(version=version):
                response = self.client.get(
                    url=(version + "/suppliers"), headers=self.headers)
                self.assertEqual(
                    response.status_code, 200,
                    msg=f"Failed to get clients: {response.content}"
                )
                clients = response.json()
                last_supplier_id = clients[-1]["id"] if clients else 1
                response = self.client.delete(
                    url=(version + f"/suppliers/{last_supplier_id}"),
                    headers=self.headers
                )
                self.assertEqual(response.status_code, 200)
