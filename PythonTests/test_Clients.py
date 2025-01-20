import httpx
import unittest


def checkClient(client):
    if len(client) != 12:
        return False

    # als de client niet die property heeft, return False
    if client.get("id") is None:
        return False
    if client.get("name") is None:
        return False
    if client.get("address") is None:
        return False
    if client.get("zip_code") is None:
        return False
    if client.get("city") is None:
        return False
    if client.get("province") is None:
        return False
    if client.get("country") is None:
        return False
    if client.get("contact_phone") is None:
        return False
    if client.get("contact_email") is None:
        return False
    if client.get("created_at") is None:
        return False
    if client.get("updated_at") is None:
        return False

    # het heeft elke property dus return true
    return True


class TestClass(unittest.TestCase):
    def setUp(self):
        self.client = httpx.Client()
        self.headers = httpx.Headers({'Api-Key': 'AdminKey'})

    # deze voegt een nieuwe warehouse object
    def test_01_post_client(self):
        for version in ["http://localhost:5001/api/v1",
                        "http://localhost:5002/api/v2"]:
            with self.subTest(version=version):
                self.url = f"{version}"

                data = {
                    "name": "Test Client",
                    "address": "123 Test Street",
                    "zip_code": "12345",
                    "city": "Test City",
                    "province": "Test Province",
                    "country": "Test Country",
                    "contact_phone": "123-456-7890",
                    "contact_email": "test@example.com",
                    "created_at": "2023-01-01T00:00:00Z",
                    "updated_at": "2023-01-01T00:00:00Z"
                }

                # Stuur de request
                response = self.client.post(
                    url=(self.url + "/clients"),
                    headers=self.headers, json=data)

                # Check de status code
                self.assertEqual(
                    response.status_code, 201,
                    msg=f"Failed to create client: {response.content}"
                )

    def test_02_get_clients(self):
        for version in ["http://localhost:5001/api/v1",
                        "http://localhost:5002/api/v2"]:
            with self.subTest(version=version):
                self.url = f"{version}"

                response = self.client.get(
                    url=(self.url + "/clients"), headers=self.headers)

                # Check de status code
                self.assertEqual(
                    response.status_code, 200,
                    msg=f"Response content: {response.content}"
                )

                # Check dat de response een list is
                self.assertEqual(type(response.json()), list)

                # Als de list iets bevat (want een list van 0
                # objects is inprincipe "legaal")
                # Check of de object in de list ook
                # echt een "object" (eigenlijk overal een dictionary) is,
                # Check of de object in de list ook echt een "object" is,
                # (eigenlijk overal een dictionary),
                # dus niet dat het een list van ints, strings etc. zijn
                self.assertEqual(type(response.json()[0]), dict)

    def test_03_get_client_id(self):
        for version in ["http://localhost:5001/api/v1",
                        "http://localhost:5002/api/v2"]:
            with self.subTest(version=version):
                self.url = f"{version}"

                # Get the last client ID
                response = self.client.get(
                    url=(self.url + "/clients"), headers=self.headers)
                self.assertEqual(
                    response.status_code, 200,
                    msg=f"Failed to get clients: {response.content}"
                )
                clients = response.json()
                last_client_id = clients[-1]["id"] if clients else 1

                # Stuur de request
                response = self.client.get(
                    url=(self.url + f"/clients/{last_client_id}"),
                    headers=self.headers)
                self.assertEqual(response.status_code, 200,
                                 msg=f"Response content: {response.content}")
                self.assertTrue(response.content,
                                msg="Response content is empty")
                self.assertEqual(type(response.json()), dict)

                # Check dat het client object de juiste properties heeft
                self.assertTrue(checkClient(response.json()))

    # Overschrijft een warehouse op basis van de opgegeven warehouse-id
    def test_04_put_client_id(self):
        for version in ["http://localhost:5001/api/v1",
                        "http://localhost:5002/api/v2"]:
            with self.subTest(version=version):
                self.url = f"{version}"

                # Get the last client ID
                response = self.client.get(
                    url=(self.url + "/clients"), headers=self.headers)
                self.assertEqual(
                    response.status_code, 200,
                    msg=f"Failed to get clients: {response.content}"
                )
                clients = response.json()
                last_client_id = clients[-1]["id"] if clients else 1

                # Create the client if no clients exist
                if not clients:
                    data = {
                        "id": last_client_id,
                        "name": "Test Client",
                        "address": "123 Test Street",
                        "zip_code": "12345",
                        "city": "Test City",
                        "province": "Test Province",
                        "country": "Test Country",
                        "contact_phone": "123-456-7890",
                        "contact_email": "test@example.com",
                        "created_at": "2023-01-01T00:00:00Z",
                        "updated_at": "2023-01-01T00:00:00Z"
                    }
                    response = self.client.post(
                        url=(self.url + "/clients"),
                        headers=self.headers, json=data)
                    self.assertEqual(
                        response.status_code, 201,
                        msg=f"Failed to create client: {response.content}"
                    )

                # Ensure the client exists before updating
                response = self.client.get(
                    url=(self.url + f"/clients/{last_client_id}"),
                    headers=self.headers)
                self.assertEqual(
                    response.status_code, 200,
                    msg=f"Client not found: {response.content}"
                )

                # Update the client
                data = {
                    "id": last_client_id,
                    "name": "Updated Test Client",
                    "address": "123 Updated Street",
                    "zip_code": "54321",
                    "city": "Updated City",
                    "province": "Updated Province",
                    "country": "Updated Country",
                    "contact_phone": "098-765-4321",
                    "contact_email": "updated@example.com",
                    "created_at": "2023-01-01T00:00:00Z",
                    "updated_at": "2023-01-02T00:00:00Z"
                }

                # Stuur de request
                response = self.client.put(
                    url=(self.url + f"/clients/{last_client_id}"),
                    headers=self.headers, json=data
                )
                if response.status_code == 500:
                    self.fail(f"Server error: {response.content}")
                else:
                    self.assertEqual(
                        response.status_code, 200,
                        msg=f"Response content: {response.content}"
                    )

    # deze delete een warehouse op basis van een id
    def test_05_delete_client_id(self):
        for version in ["http://localhost:5001/api/v1",
                        "http://localhost:5002/api/v2"]:
            with self.subTest(version=version):
                self.url = f"{version}"

                # Get the last client ID
                response = self.client.get(
                    url=(self.url + "/clients"), headers=self.headers)
                self.assertEqual(
                    response.status_code, 200,
                    msg=f"Failed to get clients: {response.content}"
                )
                clients = response.json()
                last_client_id = clients[-1]["id"] if clients else 1

                # Stuur de request
                response = self.client.delete(
                    url=(self.url + f"/clients/{last_client_id}"),
                    headers=self.headers)

                # Check de status code
                self.assertEqual(response.status_code, 200)

    def test_06_createV1_getV2_test(self):
        url1 = "http://localhost:5001/api/v1"

        data = {
            "name": "Test Client",
            "address": "123 Test Street",
            "zip_code": "12345",
            "city": "Test City",
            "province": "Test Province",
            "country": "Test Country",
            "contact_phone": "123-456-7890",
            "contact_email": "test@example.com",
            "created_at": "2023-01-01T00:00:00Z",
            "updated_at": "2023-01-01T00:00:00Z"
        }
        response = self.client.post(
            url=(url1 + "/clients"),
            headers=self.headers, json=data)

        self.assertEqual(
            response.status_code, 201,
            msg=f"Failed to create client: {response.content}"
        )
        created_clients = response.json()
        created_client_id = created_clients['id']

        url2 = "http://localhost:5002/api/v2"
        response = self.client.get(
            url=(url2 + f"/clients/{created_client_id}"),
            headers=self.headers)
        self.assertEqual(response.status_code, 200)

        response_data = response.json()
        self.assertEqual(response_data["name"], data["name"])
        self.assertEqual(response_data["address"], data["address"])
        self.assertEqual(response_data["zip_code"], data["zip_code"])
        self.assertEqual(response_data["city"], data["city"])
        self.assertEqual(response_data["province"], data["province"])
        self.assertEqual(response_data["country"], data["country"])
        self.assertEqual(response_data["contact_phone"], data["contact_phone"])
        self.assertEqual(response_data["contact_email"], data["contact_email"])

        response = self.client.delete(
            url=(url2 + f"/clients/{created_client_id}"),
            headers=self.headers)

        # Check de status code
        self.assertEqual(response.status_code, 200)
