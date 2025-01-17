import unittest
import httpx


class TestClass(unittest.TestCase):
    def setUp(self):
        self.client = httpx.Client()
        self.versions = ["http://localhost:5001/api/v1",
                         "http://localhost:5002/api/v2"]
        self.headers = {'Api-Key': 'AdminKey'}

    def test_01_post_shipment(self):
        data = {
            "order_id": 1,
            "source_id": 1,
            "order_date": "2023-01-01T00:00:00Z",
            "request_date": "2023-01-01T00:00:00Z",
            "shipment_date": "2023-01-01T00:00:00Z",
            "shipment_type": "TypeA",
            "shipment_status": "Pending",
            "notes": "Updated test shipment",
            "carrier_code": "CARRIER123",
            "carrier_description": "Updated Carrier Description",
            "service_code": "SERVICE123",
            "payment_type": "Prepaid",
            "transfer_mode": "Air",
            "total_package_count": 3,
            "total_package_weight": 10.5,
            "created_at": "2023-01-01T00:00:00Z",
            "updated_at": "2023-01-01T00:00:00Z",
            "items": [
                {
                    "item_id": "P000002",
                    "amount": 2
                },
                {
                    "item_id": "P000004",
                    "amount": 1
                },
                {
                    "item_id": "P000006",
                    "amount": 5
                }
            ]
        }

        for version in self.versions:
            with self.subTest(version=version):
                response = self.client.post(
                    url=(version + "/shipments"),
                    headers=self.headers, json=data)
                self.assertEqual(response.status_code, 201)

    def test_02_get_shipments_v1(self):
        for version in ["http://localhost:5001/api/v1"]:
            with self.subTest(version=version):
                response = self.client.get(
                    url=(version + "/shipments"), headers=self.headers)
                self.assertIn(response.status_code, [200, 204])

    def test_03_get_shipments_v2(self):
        for version in ["http://localhost:5002/api/v2"]:
            with self.subTest(version=version):
                response = self.client.get(
                    url=(version + "/shipments"), headers=self.headers)
                self.assertIn(response.status_code, [200, 204])

    def test_04_get_shipments_by_id(self):
        for version in self.versions:
            with self.subTest(version=version):
                response = self.client.get(
                    url=(version + "/shipments/1"), headers=self.headers)
                self.assertEqual(response.status_code, 200)

    def test_05_get_shipments_by_id_items(self):
        for version in self.versions:
            with self.subTest(version=version):
                response = self.client.get(
                    url=(version + "/shipments/1/items"), headers=self.headers)
                self.assertEqual(response.status_code, 200)

    def test_06_put_shipment_by_id_v1(self):
        version = "http://localhost:5001/api/v1"
        with self.subTest(version=version):
            response = self.client.get(
                url=(version + "/shipments"), headers=self.headers)
            self.assertEqual(
                response.status_code, 200,
                msg=f"Failed to get shipments: {response.content}"
            )
            shipments = response.json()
            last_shipment_id = shipments[-1]["id"] if shipments else 1

            data = {
                "id": last_shipment_id,
                "order_id": 1,
                "source_id": 1,
                "order_date": "2023-01-01T00:00:00Z",
                "request_date": "2023-01-01T00:00:00Z",
                "shipment_date": "2023-01-01T00:00:00Z",
                "shipment_type": "TypeA",
                "shipment_status": "Pending",
                "notes": "Updated test shipment",
                "carrier_code": "CARRIER123",
                "carrier_description": "Updated Carrier Description",
                "service_code": "SERVICE123",
                "payment_type": "Prepaid",
                "transfer_mode": "Air",
                "total_package_count": 3,
                "total_package_weight": 10.5,
                "created_at": "2023-01-01T00:00:00Z",
                "updated_at": "2023-01-01T00:00:00Z",
                "items": [
                    {
                        "item_id": "P000002",
                        "amount": 2
                    },
                    {
                        "item_id": "P000004",
                        "amount": 1
                    },
                    {
                        "item_id": "P000006",
                        "amount": 5
                    }
                ]
            }

            response = self.client.put(
                url=(version + f"/shipments/{last_shipment_id}"),
                headers=self.headers, json=data
            )
            self.assertEqual(response.status_code, 200)
            self.assertEqual(type(response.json()), dict)

    def test_07_put_shipment_by_id_v2(self):
        version = "http://localhost:5002/api/v2"
        with self.subTest(version=version):
            response = self.client.get(
                url=(version + "/shipments?page=0"), headers=self.headers)
            self.assertEqual(
                response.status_code, 200,
                msg=f"Failed to get shipments: {response.content}"
            )
            shipments = response.json()
            last_shipment_id = (
                shipments['data'][-1]["id"] if shipments['data'] else 1
            )

            data = {
                "id": last_shipment_id,
                "order_id": 1,
                "source_id": 1,
                "order_date": "2023-01-01T00:00:00Z",
                "request_date": "2023-01-01T00:00:00Z",
                "shipment_date": "2023-01-01T00:00:00Z",
                "shipment_type": "TypeA",
                "shipment_status": "Pending",
                "notes": "Updated test shipment",
                "carrier_code": "CARRIER123",
                "carrier_description": "Updated Carrier Description",
                "service_code": "SERVICE123",
                "payment_type": "Prepaid",
                "transfer_mode": "Air",
                "total_package_count": 3,
                "total_package_weight": 10.5,
                "created_at": "2023-01-01T00:00:00Z",
                "updated_at": "2023-01-01T00:00:00Z",
                "items": [
                    {
                        "item_id": "P000002",
                        "amount": 2
                    },
                    {
                        "item_id": "P000004",
                        "amount": 1
                    },
                    {
                        "item_id": "P000006",
                        "amount": 5
                    }
                ]
            }

            response = self.client.put(
                url=(version + f"/shipments/{last_shipment_id}"),
                headers=self.headers, json=data
            )
            self.assertEqual(response.status_code, 200)
            self.assertEqual(type(response.json()), dict)

    def test_08_delete_shipment_by_id_v1(self):
        version = "http://localhost:5001/api/v1"
        with self.subTest(version=version):
            response = self.client.get(
                url=(version + "/shipments"), headers=self.headers)
            self.assertEqual(
                response.status_code, 200,
                msg=f"Failed to get shipments: {response.content}"
            )
            shipments = response.json()
            last_shipment_id = shipments[-1]["id"] if shipments else 1
            response = self.client.delete(
                url=(version + f"/shipments/{last_shipment_id}"),
                headers=self.headers
            )
            self.assertEqual(response.status_code, 200)

    def test_09_delete_shipment_by_id_v2(self):
        version = "http://localhost:5002/api/v2"
        with self.subTest(version=version):
            response = self.client.get(
                url=(version + "/shipments?page=0"), headers=self.headers)
            self.assertEqual(
                response.status_code, 200,
                msg=f"Failed to get shipments: {response.content}"
            )
            shipments = response.json()
            last_shipment_id = (
                shipments['data'][-1]["id"] if shipments['data'] else 1
            )
            response = self.client.delete(
                url=(version + f"/shipments/{last_shipment_id}"),
                headers=self.headers
            )
            self.assertEqual(response.status_code, 200)
            
    def test_10_create_in_v1_get_and_delete_in_v2(self):
        # Create shipment in v1
        version_v1 = "http://localhost:5001/api/v1"
        data = {
            "order_id": 1,
            "source_id": 1,
            "order_date": "2023-01-01T00:00:00Z",
            "request_date": "2023-01-01T00:00:00Z",
            "shipment_date": "2023-01-01T00:00:00Z",
            "shipment_type": "TypeA",
            "shipment_status": "Pending",
            "notes": "Test shipment for cross-version",
            "carrier_code": "CARRIER123",
            "carrier_description": "Carrier Description",
            "service_code": "SERVICE123",
            "payment_type": "Prepaid",
            "transfer_mode": "Air",
            "total_package_count": 3,
            "total_package_weight": 10.5,
            "items": [
                {
                    "item_id": "P000002",
                    "amount": 2
                },
                {
                    "item_id": "P000004",
                    "amount": 1
                },
                {
                    "item_id": "P000006",
                    "amount": 5
                }
            ]
        }
        response = self.client.post(
            url=(version_v1 + "/shipments"),
            headers=self.headers, json=data
        )
        self.assertEqual(response.status_code, 201)
        shipment_id = response.json()["id"]

        # Get shipment in v2
        version_v2 = "http://localhost:5002/api/v2"
        response = self.client.get(
            url=(version_v2 + f"/shipments/{shipment_id}"),
            headers=self.headers
        )
        self.assertEqual(response.status_code, 200)

        # Check all data in the get response
        shipment_data = response.json()
        self.assertEqual(shipment_data["order_id"], data["order_id"])
        self.assertEqual(shipment_data["source_id"], data["source_id"])
        self.assertEqual(shipment_data["order_date"], data["order_date"])
        self.assertEqual(shipment_data["request_date"], data["request_date"])
        self.assertEqual(shipment_data["shipment_date"], data["shipment_date"])
        self.assertEqual(shipment_data["shipment_type"], data["shipment_type"])
        self.assertEqual(shipment_data["shipment_status"],
                         data["shipment_status"])
        self.assertEqual(shipment_data["notes"], data["notes"])
        self.assertEqual(shipment_data["carrier_code"], data["carrier_code"])
        self.assertEqual(shipment_data["carrier_description"],
                         data["carrier_description"])
        self.assertEqual(shipment_data["service_code"], data["service_code"])
        self.assertEqual(shipment_data["payment_type"], data["payment_type"])
        self.assertEqual(shipment_data["transfer_mode"], data["transfer_mode"])
        self.assertEqual(shipment_data["total_package_count"],
                         data["total_package_count"])
        self.assertEqual(shipment_data["total_package_weight"],
                         data["total_package_weight"])
        self.assertEqual(shipment_data["items"], data["items"])

        # Delete shipment in v2
        response = self.client.delete(
            url=(version_v2 + f"/shipments/{shipment_id}"),
            headers=self.headers
        )
        self.assertEqual(response.status_code, 200)