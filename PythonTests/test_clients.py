import httpx
import unittest


def checkClient(client):
    if len(client) != 12:
        return False

    # als de client niet die property heeft, return False
    if client.get("id") == None:
        return False
    if client.get("name") == None:
        return False
    if client.get("address") == None:
        return False
    if client.get("zip_code") == None:
        return False
    if client.get("city") == None:
        return False
    if client.get("province") == None:
        return False
    if client.get("country") == None:
        return False
    if client.get("contact_phone") == None:
        return False
    if client.get("contact_email") == None:
        return False
    if client.get("created_at") == None:
        return False
    if client.get("updated_at") == None:
        return False

    # het heeft elke property dus return true
    return True


class TestClass(unittest.TestCase):
    def setUp(self):
        self.client = httpx.Client()
        self.url = "http://localhost:3000/api/v1"
        self.headers = httpx.Headers({'API_KEY': 'a1b2c3d4e5'})

    def test_01_get_clients(self):

        # Stuur de request
        response = self.client.get(
            url=(self.url + "/clients"), headers=self.headers)

        # Check de status code
        self.assertEqual(response.status_code, 200)

        # Check dat de response een list is
        self.assertEqual(type(response.json()), list)

        # Als de list iets bevat (want een list van 0 objects is inprincipe "legaal")
        if (len(response.json()) > 0):
            # Check of de object in de list ook echt een "object" (eigenlijk overal een dictionary) is,
            # dus niet dat het een list van ints, strings etc. zijn
            self.assertEqual(type(response.json()[0]), dict)

    def test_02_get_client_id(self):
        # Stuur de request
        response = self.client.get(
            url=(self.url + "/clients/1"), headers=self.headers)

        # Check de status code
        self.assertEqual(response.status_code, 200)

        # Check dat de response een dictionary is (representatief voor een enkel client object)
        self.assertEqual(type(response.json()), dict)

        # Check dat het client object de juiste properties heeft
        self.assertTrue(checkClient(response.json()))

    # deze voegt een nieuwe warehouse object
    def test_04_post_client(self):
        data = {
            "id": 99999,
            "name": None,
            "address": None,
            "zip_code": None,
            "city": None,
            "province": None,
            "country": None,
            "contact_phone": None,
            "contact_email": None,
            "created_at": None,
            "updated_at": None
        }

        # Stuur de request
        response = self.client.post(
            url=(self.url + "/clients"), headers=self.headers, json=data)

        # Check de status code
        self.assertEqual(response.status_code, 201)

    # Overschrijft een warehouse op basis van de opgegeven warehouse-id

    def test_05_put_client_id(self):
        data = {
            "id": 99999,
            "code": "AAAAAAA",
            "name": None,
            "address": None,
            "zip": None,
            "city": None,
            "province": None,
            "country": None,
            "contact": None,
            "created_at": None,
            "updated_at": None
        }

        # Stuur de request
        response = self.client.put(
            url=(self.url + "/clients/99999"), headers=self.headers, json=data)

        # Check de status code
        self.assertEqual(response.status_code, 200)

        # deze delete een warehouse op basis van een id

    def test_06_delete_client_id(self):
        # Stuur de request
        response = self.client.delete(
            url=(self.url + "/clients/99999"), headers=self.headers)

        # Check de status code
        self.assertEqual(response.status_code, 200)


# to run the file: python -m unittest test_clients.py
# # git checkout . -f