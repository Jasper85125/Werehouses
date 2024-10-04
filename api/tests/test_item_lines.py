from models.item_lines import ItemLines
import unittest


class TestItemLines(unittest.TestCase):
    def setUp(self) -> None:
        self.itemLines = ItemLines("../data/")

    def test_loaded(self):
        self.assertGreater(len(self.itemLines.get_item_lines()), 0)


if __name__ == "__main__":
    unittest.main()