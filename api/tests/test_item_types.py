from models.item_types import ItemTypes
import unittest


class TestItemTypes(unittest.TestCase):
    def setUp(self) -> None:
        self.itemTypes = ItemTypes("../data/")

    def test_loaded(self):
        self.assertGreater(len(self.itemTypes.get_item_types()), 0)


if __name__ == "__main__":
    unittest.main()