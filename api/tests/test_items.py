from models.items import Items
import unittest


class TestItems(unittest.TestCase):
    def setUp(self) -> None:
        self.items = Items("../data/")

    def test_loaded(self):
        self.assertGreater(len(self.items.get_items()), 0)


if __name__ == "__main__":
    unittest.main()