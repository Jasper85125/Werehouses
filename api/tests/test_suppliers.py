from models.suppliers import Suppliers
import unittest


class TestSuppliers(unittest.TestCase):
    def setUp(self) -> None:
        self.suppliers = Suppliers("../data/")

    def test_loaded(self):
        self.assertGreater(len(self.suppliers.get_suppliers()), 0)


if __name__ == "__main__":
    unittest.main()
