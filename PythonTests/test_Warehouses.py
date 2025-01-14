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
                # Check de status code
                self.assertEqual(response.status_code, 201)

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
                warehouses = response.json()
                last_warehouse_id = warehouses[-1]["id"] if warehouses else 1
                self.assertEqual(response.status_code, 200)
                self.assertEqual(type(response.json()), dict)
                # Stuur de request
                response = self.client.get(
                    url=(version + f"/warehouses/{last_warehouse_id}"),
                    headers=self.headers)
                self.assertEqual(response.status_code, 200,
                                 msg=f"Response content: {response.content}")
                self.assertEqual(type(response.json()), dict)

                # Check dat het client object de juiste properties heeft
                self.assertTrue(checkWarehouse(response.json()))

    def test_04_get_warehouse_id(self):
        for version in self.versions:
            with self.subTest(version=version):
                response = self.warehouse.get(
                    url=(version + "/warehouses/1"), headers=self.headers
                )
                self.assertEqual(response.status_code, 200)
                warehouses = response.json()
                last_warehouse_id = warehouses[-1]["id"] if warehouses else 1
                self.assertEqual(response.status_code, 200)
                self.assertEqual(type(response.json()), dict)
                # Stuur de request
                response = self.client.get(
                    url=(version + f"/warehouses/{last_warehouse_id}"),
                    headers=self.headers)
                self.assertEqual(response.status_code, 200,
                                 msg=f"Response content: {response.content}")
                self.assertEqual(type(response.json()), dict)

                # Check dat het client object de juiste properties heeft
                self.assertTrue(checkWarehouse(response.json()))

    def test_05_put_warehouse_id(self):
        for version in self.versions:
            with self.subTest(version=version):
                self.url = f"{version}"
                response = self.warehouse.get(
                    url=(version+"/warehouses/{last_warehouse_id}"), headers=self.headers)
                self.assertEqual(response.status_code, 200)
                warehouses = response.json()
                last_warehouse_id = warehouses[-1]["id"] if warehouses else 1
                if not warehouses:
                    data = {
                        "id": last_warehouse_id,
                        "code": "AAAAAAA",
                        "name": "Warehouse",
                        "address": "1234 Test St",
                        "zip": "12345",
                        "city": "Test City",
                        "province": "Test Province",
                        "country": "Test Country",
                        "contact": {
                            "name": "John Doe",
                            "phone": "123-456-7890",
                            "email": "test@example.com"
                        },
                        "created_at": "2023-01-01T00:00:00Z",
                        "updated_at": "2023-01-01T00:00:00Z"
                    }
                    response = self.warehouse.post(
                        url=(self.url + "/warehouses"),
                        headers=self.headers, json=data
                    )
                    self.assertEqual(response.status_code, 201)
                response = self.warehouse.get(
                    url=(self.url + "/warehouses/3"), headers=self.headers)
                self.assertEqual(response.status_code, 200)

                data = {
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
                        "email": "test@example.com"
                    },
                    "created_at": "2023-01-01T00:00:00Z",
                    "updated_at": "2023-01-01T00:00:00Z"
                }
                response = self.warehouse.put(
                    url=(self.url + "/warehouses/{last_warehouse_id}"),
                    headers=self.headers, json=data
                )
                if response.status_code == 500:
                    self.fail(f"server error: {response.content}")
                else:
                    self.assertEqual(response.status_code, 200,
                                     msg="Response content:" +
                                     f"{response.content}")
        # data = {
        #     "code": "AAAAAAA",
        #     "name": "Updated Warehouse",
        #     "address": "Updated Address",
        #     "zip": "54321",
        #     "city": "Updated City",
        #     "province": "Updated Province",
        #     "country": "Updated Country",
        #     "contact": {
        #         "name": "Jane Doe",
        #         "phone": "123-456-7890",
        #         "email": "janedoe@example.com"
        #     },
        #     "created_at": "2023-01-01T00:00:00Z",
        #     "updated_at": "2023-01-01T00:00:00Z"
        # }

    def test_06_delete_warehouse_id(self):
        for version in self.versions:
            with self.subTest(version=version):
                response = self.warehouse.get(
                    url=(self.url + "/warehouses"), headers=self.headers)
                self.assertEqual(response.status_code, 200,
                                 msg=f"Response content: {response.content}")
                warehouses = response.json()
                last_warehouse_id = warehouses[-1]["id"] if warehouses else 1
                response = self.warehouse.delete(
                    url=(self.url + f"/warehouses/{last_warehouse_id}"),
                    headers=self.headers)
                self.assertEqual(response.status_code, 200)
