from models.locations import Locations
import unittest


class TestLocations(unittest.TestCase):
    def setUp(self) -> None:
        self.locations = Locations("../data/")

    def test_loaded(self):
        self.assertGreater(len(self.locations.get_locations()), 0)


if __name__ == "__main__":
    unittest.main()