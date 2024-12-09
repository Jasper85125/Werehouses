import unittest
import httpx


def checkTransfer(transfer):
    required_keys = [
        "id", "reference", "transfer_from", "transfer_to", "transfer_status",
        "created_at", "updated_at", "items"
    ]
    return all(key in transfer for key in required_keys)


class TestClass(unittest.TestCase):
    def setUp(self):
        self.client = httpx.Client()
        self.versions = [
            "http://localhost:5001/api/v1",
            "http://localhost:5002/api/v2"
        ]
        self.headers = httpx.Headers({'API_KEY': 'a1b2c3d4e5'})

    def test_01_get_transfers(self):
        for version in self.versions:
            with self.subTest(version=version):
                response = self.client.get(
                    url=(version + "/transfers"), headers=self.headers)
                self.assertEqual(
                    response.status_code, 200,
                    msg=f"Response content: {response.content}"
                )
                self.assertEqual(type(response.json()), list)
                if response.json():
                    self.assertEqual(type(response.json()[0]), dict)

    def test_02_get_transfer_id(self):
        for version in self.versions:
            with self.subTest(version=version):
                response = self.client.get(
                    url=(version + "/transfers/1"), headers=self.headers)
                self.assertEqual(
                    response.status_code, 200,
                    msg=f"Response content: {response.content}"
                )
                self.assertEqual(type(response.json()), dict)
                self.assertTrue(checkTransfer(response.json()))

    def test_03_get_items_in_transfer(self):
        for version in self.versions:
            with self.subTest(version=version):
                response = self.client.get(
                    url=version + "/transfers/1/items", headers=self.headers)
                self.assertEqual(
                    response.status_code, 200,
                    msg=f"Response content: {response.content}"
                )
                self.assertEqual(type(response.json()), list)
                if response.json():
                    self.assertEqual(type(response.json()[0]), dict)

    def test_04_post_transfer(self):
        for version in self.versions:
            with self.subTest(version=version):
                data = {
                    "id": 99999,
                    "reference": "Test Reference",
                    "transfer_from": 1,
                    "transfer_to": 2,
                    "transfer_status": "Pending",
                    "created_at": "2023-01-01T00:00:00Z",
                    "updated_at": "2023-01-01T00:00:00Z",
                    "items": [],
                    "transfer": "Sample Transfer"
                }
                response = self.client.post(
                    url=(version + "/transfers"),
                    headers=self.headers, json=data)
                self.assertEqual(
                    response.status_code, 201,
                    msg=f"Failed to create transfer: {response.content}"
                )

    def test_05_put_transfer_id(self):
        for version in self.versions:
            with self.subTest(version=version):
                response = self.client.get(
                    url=(version + "/transfers"), headers=self.headers)
                self.assertEqual(
                    response.status_code, 200,
                    msg=f"Failed to get transfers: {response.content}"
                )
                transfers = response.json()
                last_transfer_id = transfers[-1]["id"] if transfers else 99999

                if not transfers:
                    data = {
                        "id": last_transfer_id,
                        "reference": "Test Reference",
                        "transfer_from": 1,
                        "transfer_to": 2,
                        "transfer_status": "Pending",
                        "created_at": "2023-01-01T00:00:00Z",
                        "updated_at": "2023-01-01T00:00:00Z",
                        "items": [],
                        "transfer": "Sample Transfer"
                    }
                    response = self.client.post(
                        url=(version + "/transfers"),
                        headers=self.headers, json=data)
                    self.assertEqual(
                        response.status_code, 201,
                        msg=f"Failed to create transfer: {response.content}"
                    )

                response = self.client.get(
                    url=(version + f"/transfers/{last_transfer_id}"),
                    headers=self.headers)
                self.assertEqual(
                    response.status_code, 200,
                    msg=f"Transfer not found: {response.content}"
                )

                data = {
                    "id": last_transfer_id,
                    "reference": "Updated Reference",
                    "transfer_from": 1,
                    "transfer_to": 2,
                    "transfer_status": "Completed",
                    "created_at": "2023-01-01T00:00:00Z",
                    "updated_at": "2023-01-02T00:00:00Z",
                    "items": []
                }
                response = self.client.put(
                    url=(version + f"/transfers/{last_transfer_id}"),
                    headers=self.headers, json=data)
                if response.status_code == 500:
                    self.fail(f"Server error: {response.content}")
                else:
                    self.assertEqual(
                        response.status_code, 200,
                        msg=f"Response content: {response.content}"
                    )

    def test_06_delete_transfer_id(self):
        for version in self.versions:
            with self.subTest(version=version):
                response = self.client.delete(
                    url=(version + "/transfers/15"), headers=self.headers)
                self.assertEqual(response.status_code, 200)
