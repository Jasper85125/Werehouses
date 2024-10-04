from models.inventories import Inventories
import unittest


class TestInventories(unittest.TestCase):
    def setUp(self) -> None:
        self.inventories = Inventories("../data/")

    def test_loaded(self):
        self.assertGreater(len(self.inventories.get_inventories()), 0)


if __name__ == "__main__":
    unittest.main()