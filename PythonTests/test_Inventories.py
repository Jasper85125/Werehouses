import httpx
import unittest


class TestClass(unittest.TestCase):
    def setUp(self):
        self.client = httpx.Client()
        self.versions = ["http://localhost:5001/api/v1",
                         "http://localhost:5002/api/v2"]
        self.headers = {'Api-Key': 'AdminKey'}

    def test_01_post_inventory(self):
        data = {
            "item_id": "ITEM123",
            "description": "Test description",
            "item_reference": "Test reference",
            "locations": [],
            "total_on_hand": 10,
            "total_expected": 5,
            "total_ordered": 3,
            "total_allocated": 2,
            "total_available": 8,
            "created_at": "2023-01-01T00:00:00Z",
            "updated_at": "2023-01-01T00:00:00Z"
        }
        for version in self.versions:
            with self.subTest(version=version):
                response = self.client.post(url=(version + "/inventories"),
                                            headers=self.headers, json=data)
                self.assertEqual(response.status_code, 201)

    def test_02_get_inventories(self):
        for version in self.versions:
            with self.subTest(version=version):
                response = self.client.get(url=(version + "/inventories"),
                                           headers=self.headers)
                self.assertEqual(response.status_code, 200)

    def test_03_get_inventory_id(self):
        for version in self.versions:
            with self.subTest(version=version):
                response = self.client.get(
                    url=(version + "/inventories"), headers=self.headers)
                self.assertEqual(
                    response.status_code, 200,
                    msg=f"Failed to get inventories: {response.content}"
                )
                inventories = response.json()
                last_inventory_id = inventories[-1]["id"] if inventories else 1

                response = self.client.get(url=(
                    version + f"/inventories/{last_inventory_id}"),
                                           headers=self.headers)
                self.assertEqual(response.status_code, 200)

    def test_04_put_inventory_id(self):
        data = {
            "id": 1,
            "item_id": "ITEM123",
            "description": "Updated description",
            "item_reference": "Updated reference",
            "locations": [],
            "total_on_hand": 20,
            "total_expected": 10,
            "total_ordered": 5,
            "total_allocated": 3,
            "total_available": 15,
            "created_at": "2023-01-01T00:00:00Z",
            "updated_at": "2023-01-01T00:00:00Z"
        }
        for version in self.versions:
            with self.subTest(version=version):
                response = self.client.get(
                    url=(version + "/inventories"), headers=self.headers)
                self.assertEqual(
                    response.status_code, 200,
                    msg=f"Failed to get inventories: {response.content}"
                )
                inventories = response.json()
                last_inventory_id = inventories[-1]["id"] if inventories else 1

                response = self.client.put(url=(
                    version + f"/inventories/{last_inventory_id}"),
                                           headers=self.headers, json=data)
                self.assertEqual(response.status_code, 200)

    def test_05_delete_inventory_id(self):
        for version in self.versions:
            with self.subTest(version=version):
                response = self.client.get(
                    url=(version + "/inventories"), headers=self.headers)
                self.assertEqual(
                    response.status_code, 200,
                    msg=f"Failed to get inventories: {response.content}"
                )
                inventories = response.json()
                last_inventory_id = inventories[-1]["id"] if inventories else 1

                response = self.client.delete(url=(
                    version + f"/inventories/{last_inventory_id}"),
                                              headers=self.headers)
                self.assertEqual(response.status_code, 200)

    def test_06_createV1_getV2_test(self):
        url1 = "http://localhost:5001/api/v1"

        data = {
            "item_id": "ITEM123",
            "description": "Updated description",
            "item_reference": "Updated reference",
            "locations": [],
            "total_on_hand": 20,
            "total_expected": 10,
            "total_ordered": 5,
            "total_allocated": 3,
            "total_available": 15,
        }
        response = self.client.post(
            url=(url1 + "/inventories"),
            headers=self.headers, json=data)

        self.assertEqual(
            response.status_code, 201,
            msg=f"Failed to create client: {response.content}"
        )
        created_inventory = response.json()
        created_inventory_id = created_inventory['id']

        url2 = "http://localhost:5002/api/v2"
        response = self.client.get(url=(
            url2 + f"/inventories/{created_inventory_id}"),
                                    headers=self.headers)
        self.assertEqual(response.status_code, 200)

        response_data = response.json()
        self.assertEqual(response_data["item_id"], data["item_id"])
        self.assertEqual(response_data["description"], data["description"])
        self.assertEqual(response_data["item_reference"],
                         data["item_reference"])
        self.assertEqual(response_data["locations"], data["locations"])
        self.assertEqual(response_data["total_on_hand"], data["total_on_hand"])
        self.assertEqual(response_data["total_expected"],
                         data["total_expected"])
        self.assertEqual(response_data["total_ordered"], data["total_ordered"])
        self.assertEqual(response_data["total_allocated"],
                         data["total_allocated"])
        self.assertEqual(response_data["total_available"],
                         data["total_available"])

        response = self.client.delete(url=(
            url2 + f"/inventories/{created_inventory_id}"),
                                    headers=self.headers)
        self.assertEqual(response.status_code, 200)
