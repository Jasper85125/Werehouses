import httpx
import unittest

# class TestWarehouses(unittest.TestCase):
#     def setUp(self) -> None:
#         self.warehouses = Warehouses("../data/")

#     def test_loaded(self):
#         self.assertGreater(len(self.warehouses.get_warehouses()), 0)


class TestClass(unittest.TestCase):
    def setUp(self):
        self.client = httpx.Client()
        self.url = "http://localhost:5125/api/v2"
        self.headers = {'API_KEY': 'a1b2c3d4e5'}

    def test_get_inventories(self):

        response = self.client.get(
            url=(self.url + "/inventories"), headers=self.headers)
        self.assertEqual(response.status_code, 200)

    def test_get_inventory_id(self):
        response = self.client.get(
            url=(self.url + "/inventories/1"), headers=self.headers)

        self.assertEqual(response.status_code, 200)

    def test_post_inventory(self):
        data = {
            "id": 99999,
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

        response = self.client.post(
            url=(self.url + "/inventories"), headers=self.headers, json=data)

        self.assertEqual(response.status_code, 201)

    def test_put_inventory_id(self):
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

        response = self.client.put(
            url=(self.url + "/inventories/1"),
            headers=self.headers,
            json=data
        )

        self.assertEqual(response.status_code, 200)

    def test_delete_inventory_id(self):
        response = self.client.delete(
            url=(self.url + "/inventories/5"), headers=self.headers)

        self.assertEqual(response.status_code, 200)
