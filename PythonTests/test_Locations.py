import unittest
import httpx


class TestClass(unittest.TestCase):
    def setUp(self):
        self.client = httpx.Client()
        self.versions = ["http://localhost:5001/api/v1",
                         "http://localhost:5002/api/v2"]
        self.headers = {'Api-Key': 'AdminKey'}

    def test_create_location(self):
        data = {
            "id": 69696,
            "warehouse_id": 20,
            "code": "A.D.0",
            "name": "Test Location",
            "created_at": "2023-01-01T00:00:00Z",
        }
        for url in self.versions:
            with self.subTest(url=url):
                response = self.client.post(
                    url=(url + "/locations"), headers=self.headers, json=data
                )
                self.assertEqual(response.status_code, 201)

    def test_get_locations(self):
        for url in self.versions:
            with self.subTest(url=url):
                response = self.client.get(
                    url=(url + "/locations"), headers=self.headers
                )
                self.assertEqual(response.status_code, 200)

    def test_get_location_id(self):
        for url in self.versions:
            with self.subTest(url=url):
                response = self.client.get(
                    url=(url + "/locations"), headers=self.headers)
                self.assertEqual(
                    response.status_code, 200,
                    msg=f"Failed to get locations: {response.content}"
                )
                locations = response.json()
                last_location_id = locations[-1]["id"] if locations else 1

                response = self.client.get(
                    url=(url + f"/locations/{last_location_id}"),
                    headers=self.headers
                )
                self.assertEqual(response.status_code, 200)

    def test_update_location(self):
        data = {
            "id": 5,
            "warehouse_id": 20,
            "code": "A.D.0",
            "name": "Updated Location",
            "created_at": "2023-01-01T00:00:00Z",
        }
        for url in self.versions:
            with self.subTest(url=url):
                response = self.client.get(
                    url=(url + "/locations"), headers=self.headers)
                self.assertEqual(
                    response.status_code, 200,
                    msg=f"Failed to get locations: {response.content}"
                )
                locations = response.json()
                last_location_id = locations[-1]["id"] if locations else 1

                response = self.client.put(
                    url=(url + f"/locations/{last_location_id}"),
                    headers=self.headers, json=data
                )
                self.assertEqual(response.status_code, 200)

    def test_delete_location_id(self):
        for url in self.versions:
            with self.subTest(url=url):
                response = self.client.get(
                    url=(url + "/locations"), headers=self.headers)
                self.assertEqual(
                    response.status_code, 200,
                    msg=f"Failed to get locations: {response.content}"
                )
                locations = response.json()
                last_location_id = locations[-1]["id"] if locations else 1

                response = self.client.delete(
                    url=(url + f"/locations/{last_location_id}"),
                    headers=self.headers
                )
                self.assertEqual(response.status_code, 200)


if __name__ == "__main__":
    unittest.main()
