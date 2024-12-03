# from models.suppliers import Suppliers
import unittest
import requests


# class TestSuppliers(unittest.TestCase):
#     def setUp(self) -> None:
#         self.suppliers = Suppliers("../data/")

#     def test_loaded(self):
#         self.assertGreater(len(self.suppliers.get_suppliers()), 0)

class TestClass(unittest.TestCase):
    def setUp(self):
        self.url = "http://localhost:5125/api/v1"
        self.headers = {'API_KEY': 'a1b2c3d4e5'}

    def test_get_suppliers(self):
        response = requests.get(
            url=(self.url + "/suppliers"), headers=self.headers
        )

        self.assertEqual(response.status_code, 200)
        response = requests.get(
            url=(self.url + "/suppliers/1"), headers=self.headers
        )

    def test_get_supplier_id(self):
        response = requests.get(
            url=(self.url + "/suppliers/1"), headers=self.headers
        )
        response = requests.get(
            url=(self.url + "/suppliers/1/items"), headers=self.headers
        )
        self.assertEqual(response.status_code, 200)


def test_get_items_from_supplier(self):
    data = {
        "id": 678098,
        "code": "SUP",
        "name": "FRANKY",
        "address": "SUNNY",
        "address_extra": None,
        "city": None,
        "zip_code": None,
        "province": "East Blue",
        "country": "Water Severn",
        "contact_name": "Iceberg",
        "phonenumber": None,
        "reference": "FRANKY-SUP",
        "created_at": None,
        "updated_at": None
    }
    response = requests.post(
        url=(self.url + "/suppliers"), headers=self.headers, json=data
    )
    self.assertEqual(response.status_code, 200)

    data = {
        "id": 12345,
        "code": "SUP",
        "name": "FRANKY",
        "address": "SUNNY",
        "address_extra": None,
        "city": None,
        "zip_code": None,
        "province": "East Blue",
        "country": "Water Severn",
        "contact_name": "Iceberg",
        "phonenumber": None,
        "reference": "FRANKY-SUP",
        "created_at": None,
        "updated_at": None
    }
    response = requests.put(
        url=(self.url + "/suppliers/2"), headers=self.headers, json=data
    )
    self.assertEqual(response.status_code, 200)

    response = requests.delete(
        url=(self.url + "/suppliers/3"), headers=self.headers
    )
    self.assertEqual(response.status_code, 200)

    def test_delete_supplier_id(self):
        response = requests.delete(
            url=(self.url + "/suppliers/3"), headers=self.headers
        )

        self.assertEqual(response.status_code, 200)


if __name__ == "__main__":
    unittest.main()
