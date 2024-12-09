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
        self.headers = {'API_KEY': 'a1b2c3d4e5'}
        self.versions = ["http://localhost:5001/api/v1",
                         "http://localhost:5002/api/v2"]

    def test_get_suppliers(self):
        for version in self.versions:
            with self.subTest(version=version):
                response = self.client.get(
                    url=(version + "/suppliers"), headers=self.headers
                )
                self.assertEqual(response.status_code, 200)
                response = self.client.get(
                    url=(version + "/suppliers/1"), headers=self.headers
                )
                self.assertEqual(response.status_code, 200)

    def test_get_supplier_id(self):
        for version in self.versions:
            with self.subTest(version=version):
                response = self.client.get(
                    url=(version + "/suppliers/1"), headers=self.headers
                )
                self.assertEqual(response.status_code, 200)
                response = self.client.get(
                    url=(version + "/suppliers/1/items"), headers=self.headers
                )
                self.assertEqual(response.status_code, 200)

    def test_post_suppliers(self):
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

    def test_put_supplier_id(self):
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
                response = self.client.put(
                    url=(version + "/suppliers/2"),
                    headers=self.headers, json=data
                )
                self.assertEqual(response.status_code, 200)

    def test_delete_supplier_id(self):
        for version in self.versions:
            with self.subTest(version=version):
                response = self.client.delete(
                    url=(version + "/suppliers/3"), headers=self.headers
                )
                self.assertEqual(response.status_code, 200)
