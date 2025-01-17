import unittest
import httpx


class TestClass(unittest.TestCase):
    def setUp(self):
        self.client = httpx.Client()
        self.versions = ["http://localhost:5001/api/v1",
                         "http://localhost:5002/api/v2"]
        self.headers = {'Api-Key': 'AdminKey'}

    def test_01_create_location(self):
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

    def test_02_get_locations(self):
        for url in self.versions:
            with self.subTest(url=url):
                response = self.client.get(
                    url=(url + "/locations"), headers=self.headers
                )
                self.assertEqual(response.status_code, 200)

    def test_03_get_location_id(self):
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

    def test_04_update_location(self):
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

    def test_05_delete_location_id(self):
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

    def test_06_create_in_v1_get_and_delete_in_v2(self):
        # Create location in v1
        data = {
            "id": 34534,
            "warehouse_id": 21,
            "code": "B.D.1",
            "name": "Cross Version Location",
        }
        response = self.client.post(
            url="http://localhost:5001/api/v1/locations",
            headers=self.headers, json=data
        )
        self.assertEqual(response.status_code, 201)
        created_location = response.json()
        created_location_id = created_location['id']

        # Get location in v2
        response = self.client.get(
            url="http://localhost:5002/api/v2/locations", headers=self.headers
        )
        self.assertEqual(
            response.status_code, 200,
            msg=f"Failed to get locations: {response.content}"
        )
        locations = response.json()
        created_location = next(
            (loc for loc in locations if loc["id"] == created_location_id),
            None
        )
        self.assertIsNotNone(
            created_location, "Created location not found in v2"
        )

        # Check all data in the get
        self.assertEqual(created_location["id"], data["id"])
        self.assertEqual(created_location["warehouse_id"],
                         data["warehouse_id"])
        self.assertEqual(created_location["code"], data["code"])
        self.assertEqual(created_location["name"], data["name"])

        # Delete location in v2
        response = self.client.delete(
            url=(
                f"http://localhost:5002/api/v2/locations/"
                f"{created_location_id}"
            ),
            headers=self.headers
        )
        self.assertEqual(response.status_code, 200)
