import unittest
import httpx


def checkWarehouse(warehouse):
    if warehouse.get("id") is None:
        return False
    if warehouse.get("code") is None:
        return False
    if warehouse.get("name") is None:
        return False
    if warehouse.get("address") is None:
        return False
    if warehouse.get("zip") is None:
        return False
    if warehouse.get("city") is None:
        return False
    if warehouse.get("province") is None:
        return False
    if warehouse.get("country") is None:
        return False
    if warehouse.get("contact") is None:
        return False
    if warehouse.get("created_at") is None:
        return False
    if warehouse.get("updated_at") is None:
        return False

    return True


class TestClass(unittest.TestCase):
    def setUp(self):
        self.warehouse = httpx.Client()
        self.versions = [
            "http://localhost:5001/api/v1",
            "http://localhost:5002/api/v2"
        ]
        self.headers = {'Api-Key': 'AdminKey'}

    def test_01_post_warehouse(self):
        data = {
            "code": "WH99999",
            "name": "Warehouse 99999",
            "address": "1234 Test St",
            "zip": "12345",
            "city": "Test City",
            "province": "Test Province",
            "country": "Test Country",
            "contact": {
                "name": "John Doe",
                "phone": "123-456-7890",
                "email": "johndoe@example.com"
            },
            "created_at": "2023-01-01T00:00:00Z",
            "updated_at": "2023-01-01T00:00:00Z"
        }
        for version in self.versions:
            with self.subTest(version=version):
                response = self.warehouse.post(
                    url=(version + "/warehouses"),
                    headers=self.headers, json=data
                )
                # Stuur de request
                response = self.client.post(
                    url=(self.url + "/clients"),
                    headers=self.headers, json=data)

                # Check de status code
                self.assertEqual(
                    response.status_code, 201,
                    msg=f"Failed to create client: {response.content}"
                )

    def test_02_get_warehouses_v1(self):
        for version in ["http://localhost:5001/api/v1"]:
            with self.subTest(version=version):
                response = self.warehouse.get(
                    url=(version + "/warehouses"), headers=self.headers
                )
                self.assertEqual(response.status_code, 200)
                self.assertEqual(type(response.json()), list)

    def test_03_get_warehouses_v2(self):
        for version in ["http://localhost:5002/api/v2"]:
            with self.subTest(version=version):
                response = self.warehouse.get(
                    url=(version + "/warehouses"), headers=self.headers
                )
                self.assertEqual(response.status_code, 200)
                self.assertEqual(type(response.json()), dict)

    def test_04_get_warehouse_id(self):
        for version in self.versions:
            with self.subTest(version=version):
                response = self.warehouse.get(
                    url=(version + "/warehouses/1"), headers=self.headers
                )
                self.assertEqual(response.status_code, 200)

    def test_05_put_warehouse_id(self):
        data = {
            "id": 2,
            "code": "AAAAAAA",
            "name": "Updated Warehouse",
            "address": "Updated Address",
            "zip": "54321",
            "city": "Updated City",
            "province": "Updated Province",
            "country": "Updated Country",
            "contact": {
                "name": "Jane Doe",
                "phone": "123-456-7890",
                "email": "janedoe@example.com"
            },
            "created_at": "2023-01-01T00:00:00Z",
            "updated_at": "2023-01-01T00:00:00Z"
        }
        for version in self.versions:
            with self.subTest(version=version):
                response = self.warehouse.put(
                    url=(version + "/warehouses/2"),
                    headers=self.headers, json=data
                )
                self.assertEqual(response.status_code, 200)

    def test_06_delete_warehouse_id(self):
        for version in self.versions:
            with self.subTest(version=version):
                response = self.warehouse.delete(
                    url=(version + "/warehouses/3"), headers=self.headers
                )
                self.assertEqual(response.status_code, 200)
