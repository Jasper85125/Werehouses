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

    def test_01_get_clients(self):
        for version in ["http://localhost:5001/api/v1",
                        "http://localhost:5002/api/v2"]:
            with self.subTest(version=version):
                self.url = f"{version}"

                # Stuur de request
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

    def test_02_get_client_id(self):
        for version in ["http://localhost:5001/api/v1",
                        "http://localhost:5002/api/v2"]:
            with self.subTest(version=version):
                self.url = f"{version}"

                # Stuur de request
                response = self.client.get(
                    url=(self.url + "/clients/1"), headers=self.headers)

                # Check de status code
                # Check dat de response een
                # dictionary is (representatief voor een
                # enkel client object)

                # Check dat de response
                # een dictionary is (representatief voor een
                # enkel client object)
                self.assertEqual(response.status_code, 200,
                                 msg=f"Response content: {response.content}")
                self.assertTrue(response.content,
                                msg="Response content is empty")
                self.assertEqual(type(response.json()), dict)

                # Check dat het client object de juiste properties heeft
                self.assertTrue(checkClient(response.json()))

    # deze voegt een nieuwe warehouse object
    def test_04_post_client(self):
        for version in ["http://localhost:5001/api/v1",
                        "http://localhost:5002/api/v2"]:
            with self.subTest(version=version):
                self.url = f"{version}"

                data = {
                    "id": 99999,
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

    # Overschrijft een warehouse op basis van de opgegeven warehouse-id
    def test_05_put_client_id(self):
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
                last_client_id = clients[-1]["id"] if clients else 99999

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
    def test_06_delete_client_id(self):
        for version in ["http://localhost:5001/api/v1",
                        "http://localhost:5002/api/v2"]:
            with self.subTest(version=version):
                self.url = f"{version}"

                # Stuur de request
                response = self.client.delete(
                    url=(self.url + "/clients/1"), headers=self.headers)

                # Check de status code
                self.assertEqual(response.status_code, 200)
