from models.item_groups import ItemGroups
import unittest


class TestItemGroups(unittest.TestCase):
    def setUp(self) -> None:
        self.itemGroups = ItemGroups("../data/")

    def test_loaded(self):
        self.assertGreater(len(self.itemGroups.get_item_groups()), 0)


if __name__ == "__main__":
    unittest.main()