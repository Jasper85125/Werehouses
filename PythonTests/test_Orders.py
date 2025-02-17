import unittest
import httpx


class TestOrdersAPI(unittest.TestCase):
    def setUp(self):
        self.client = httpx.Client()
        self.headers = {'Api-Key': 'AdminKey'}
        self.versions = ["http://localhost:5001/api/v1",
                         "http://localhost:5002/api/v2"]

    def test_01_post_order(self):
        data = {
                "id": 1,
                "source_id": 33,
                "order_date": "2019-04-03T11:33:15Z",
                "request_date": "2019-04-07T11:33:15Z",
                "reference": "ORD00001",
                "reference_extra": "Bedreven arm straffen bureau.",
                "order_status": "Delivered",
                "notes": "Voedsel vijf vork heel.",
                "shipping_notes": "Buurman betalen plaats bewolkt.",
                "picking_notes": "Volgorde scherp aardappel op leren.",
                "warehouse_id": 18,
                "ship_to": 4562,
                "bill_to": 7863,
                "shipment_id": 1,
                "total_amount": 9905.13,
                "total_discount": 150.77,
                "total_tax": 372.72,
                "total_surcharge": 77.6,
                "created_at": "2019-04-03T11:33:15Z",
                "updated_at": "2019-04-05T07:33:15Z",
                "items": [
                    {
                        "item_id": "P000002",
                        "amount": 2
                    },
                    {
                        "item_id": "P000004",
                        "amount": 1
                    }
                ]
        }
        for version in self.versions:
            response = self.client.post(
                url=(version + "/orders"), headers=self.headers, json=data)

            self.assertIn(response.status_code, [201, 405])

    def test_02_get_orders_v1(self):
        response = self.client.get(
            url=(self.versions[0] + "/orders"), headers=self.headers)

        self.assertEqual(response.status_code, 200)
        self.assertEqual(type(response.json()), list)

    def test_03_get_orders_v2(self):
        response = self.client.get(
            url=(self.versions[1] + "/orders"), headers=self.headers)

        self.assertEqual(response.status_code, 200)
        self.assertEqual(type(response.json()), dict)

    def test_04_get_order_id_v1(self):
        for url in ["http://localhost:5001/api/v1"]:
            with self.subTest(url=url):
                response = self.client.get(
                    url=(url + "/orders"), headers=self.headers)
                self.assertEqual(
                    response.status_code, 200,
                    msg=f"Failed to get orders: {response.content}"
                )
                orders = response.json()
                last_order_id = orders[-1]["id"] if orders else 1

                response = self.client.get(
                    url=(url + f"/orders/{last_order_id}"),
                    headers=self.headers
                )

                self.assertEqual(response.status_code, 200)
                self.assertEqual(type(response.json()), dict)

    def test_04_get_order_id_v2(self):
        for url in ["http://localhost:5002/api/v2"]:
            with self.subTest(url=url):
                response = self.client.get(
                    url=(url + "/orders?page=0"), headers=self.headers)
                self.assertEqual(
                    response.status_code, 200,
                    msg=f"Failed to get orders: {response.content}"
                )
                orders = response.json()
                last_order_id = (
                    next(reversed(orders.values()))[-1]["id"] if orders else 1
                )

                response = self.client.get(
                    url=(url + f"/orders/{last_order_id}"),
                    headers=self.headers
                )

                self.assertEqual(response.status_code, 200)
                self.assertEqual(type(response.json()), dict)

    def test_05_get_orders_by_id_items_v2(self):
        for url in ["http://localhost:5002/api/v2"]:
            with self.subTest(url=url):
                response = self.client.get(
                    url=(url + "/orders?page=0"), headers=self.headers)
                self.assertEqual(
                    response.status_code, 200,
                    msg=f"Failed to get orders: {response.content}"
                )
                orders = response.json()
                last_order_id = (
                    next(reversed(orders.values()))[-1]["id"] if orders else 1
                )

                response = self.client.get(
                    url=(url + f"/orders/{last_order_id}/items"),
                    headers=self.headers
                )

            self.assertEqual(response.status_code, 200)

    def test_05_get_orders_by_id_items_v1(self):
        for url in ["http://localhost:5001/api/v1"]:
            with self.subTest(url=url):
                response = self.client.get(
                    url=(url + "/orders"), headers=self.headers)
                self.assertEqual(
                    response.status_code, 200,
                    msg=f"Failed to get orders: {response.content}"
                )
                orders = response.json()
                last_order_id = orders[-1]["id"] if orders else 1

                response = self.client.get(
                    url=(url + f"/orders/{last_order_id}/items"),
                    headers=self.headers
                )

            self.assertEqual(response.status_code, 200)

    def test_06_get_orders_by_id_shipments(self):
        for url in self.versions:
            response = self.client.get(
                url=(url + "/orders/1/shipments"),
                headers=self.headers)

        self.assertEqual(response.status_code, 200)

    def test_07_put_order_id(self):
        data = {
                "id": 1,
                "source_id": 35,
                "order_date": "2019-04-03T11:33:15Z",
                "request_date": "2019-04-07T11:33:15Z",
                "reference": "ORD00001",
                "reference_extra": "Bedreven arm straffen bureau.",
                "order_status": "Delivered",
                "notes": "Voedsel vijf vork heel.",
                "shipping_notes": "Buurman betalen plaats bewolkt.",
                "picking_notes": "Volgorde scherp aardappel op leren.",
                "warehouse_id": 18,
                "ship_to": 4562,
                "bill_to": 7863,
                "shipment_id": 1,
                "total_amount": 9905.13,
                "total_discount": 150.77,
                "total_tax": 372.72,
                "total_surcharge": 77.6,
                "created_at": "2019-04-03T11:33:15Z",
                "updated_at": "2019-04-05T07:33:15Z",
                "items": [
                    {
                        "item_id": "P000002",
                        "amount": 2
                    },
                    {
                        "item_id": "P000004",
                        "amount": 1
                    }
                ]
        }
        for version in self.versions:
            with self.subTest(version=version):
                # Get the last client ID
                response = self.client.get(
                    url=(version + "/orders"), headers=self.headers)
                self.assertEqual(
                    response.status_code, 200,
                    msg=f"Failed to get clients: {response.content}"
                )
                orders = response.json()

                if version == 'http://localhost:5001/api/v1':
                    last_order_id = orders[-1]["id"] if orders \
                        else 1
                else:
                    response = self.client.get(
                        url=(version + "/orders?page=0"),
                        headers=self.headers)
                    orders = response.json()
                    last_order_id = orders['data'][-1]["id"] \
                        if orders else 1

                response = self.client.put(
                    url=(version + f"/orders/{last_order_id}"),
                    headers=self.headers, json=data
                )
                self.assertEqual(response.status_code, 200)

    def test_08_delete_order_id(self):
        # Get the last order ID
        for version in self.versions:
            with self.subTest(version=version):
                response = self.client.get(
                    url=(version + "/orders"), headers=self.headers)
                self.assertEqual(
                    response.status_code, 200,
                    msg=f"Failed to get clients: {response.content}"
                )
                orders = response.json()

                if version == 'http://localhost:5001/api/v1':
                    last_order_id = orders[-1]["id"] if orders \
                        else 1
                else:
                    response = self.client.get(
                        url=(version + "/orders?page=0"),
                        headers=self.headers)
                    orders = response.json()
                    last_order_id = orders['data'][-1]["id"] \
                        if orders else 1
                response = self.client.delete(
                    url=(version + f"/orders/{last_order_id}"),
                    headers=self.headers)

            self.assertEqual(response.status_code, 200)

    def test_09_create_in_v1_get_and_delete_in_v2(self):
        # Create order in v1
        data = {
                "source_id": 33,
                "order_date": "2019-04-03T11:33:15Z",
                "request_date": "2019-04-07T11:33:15Z",
                "reference": "ORD00001",
                "reference_extra": "Bedreven arm straffen bureau.",
                "order_status": "Delivered",
                "notes": "Voedsel vijf vork heel.",
                "shipping_notes": "Buurman betalen plaats bewolkt.",
                "picking_notes": "Volgorde scherp aardappel op leren.",
                "warehouse_id": 18,
                "ship_to": 4562,
                "bill_to": 7863,
                "shipment_id": 1,
                "total_amount": 9905.13,
                "total_discount": 150.77,
                "total_tax": 372.72,
                "total_surcharge": 77.6,
                "items": [
                    {
                        "item_id": "P000002",
                        "amount": 2
                    },
                    {
                        "item_id": "P000004",
                        "amount": 1
                    }
                ]
        }
        response = self.client.post(
            url=(self.versions[0] + "/orders"), headers=self.headers,
            json=data)
        self.assertIn(response.status_code, [201, 405])
        created_order = response.json()
        created_order_id = created_order['id']

        # Get the order in v2 using created_order_id
        response = self.client.get(
            url=(self.versions[1] + f"/orders/{created_order_id}"),
            headers=self.headers)
        self.assertEqual(response.status_code, 200)
        self.assertEqual(type(response.json()), dict)

        # Check all the data in the get response
        get_order = response.json()
        for key in data:
            if key != "items":
                self.assertEqual(get_order[key], data[key])
            else:
                for item in data["items"]:
                    self.assertIn(item, get_order["items"])

        # Delete the order in v2 using created_order_id
        response = self.client.delete(
            url=(self.versions[1] + f"/orders/{created_order_id}"),
            headers=self.headers)
        self.assertEqual(response.status_code, 200)
