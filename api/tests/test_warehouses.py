from models.warehouses import Warehouses
import unittest


class TestWarehouses(unittest.TestCase):
    def setUp(self) -> None:
        self.warehouses = Warehouses("../data/")

    def test_loaded(self):
        self.assertGreater(len(self.warehouses.get_warehouses()), 0)


if __name__ == "__main__":
    unittest.main()